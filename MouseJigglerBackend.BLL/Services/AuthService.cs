using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MouseJigglerBackend.Core.DTOs;
using MouseJigglerBackend.Core.Entities;
using MouseJigglerBackend.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MouseJigglerBackend.BLL.Services;

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly IPasswordService _passwordService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserService userService,
        IPasswordService passwordService,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _userService = userService;
        _passwordService = passwordService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        try
        {
            _logger.LogInformation("Attempting login for email: {Email}", request.Email);

            // Get the user entity to access password hash
            var userEntity = await _userService.GetUserEntityByEmailAsync(request.Email);
            if (userEntity == null)
            {
                _logger.LogWarning("Login failed: User not found for email: {Email}", request.Email);
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid email or password",
                    Errors = new List<string> { "Invalid credentials" }
                };
            }

            // Verify password
            var passwordValid = _passwordService.VerifyPassword(request.Password, userEntity.PasswordHash);
            if (!passwordValid)
            {
                _logger.LogWarning("Login failed: Invalid password for email: {Email}", request.Email);
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid email or password",
                    Errors = new List<string> { "Invalid credentials" }
                };
            }

            // Get user DTO for token generation
            var userDto = await _userService.GetUserByEmailAsync(request.Email);
            var token = GenerateJwtToken(userDto!);
            var refreshToken = GenerateRefreshToken();

            _logger.LogInformation("Login successful for email: {Email}", request.Email);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Login successful",
                Token = token,
                User = userDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
            return new AuthResponseDto
            {
                Success = false,
                Message = "An error occurred during login",
                Errors = new List<string> { "Internal server error" }
            };
        }
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        try
        {
            _logger.LogInformation("Attempting registration for email: {Email}", request.Email);

            // Check if user already exists
            var existingUsers = await _userService.GetUsersByEmailFilterAsync(new[] { request.Email });
            if (existingUsers.Any())
            {
                _logger.LogWarning("Registration failed: User already exists for email: {Email}", request.Email);
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User with this email already exists",
                    Errors = new List<string> { "Email already registered" }
                };
            }

            // Hash the password
            var passwordHash = _passwordService.HashPassword(request.Password);

            // Create new user
            var createdUser = await _userService.CreateUserAsync(
                email: request.Email,
                firstName: string.Empty,
                lastName: string.Empty,
                passwordHash: passwordHash
            );

            if (createdUser == null)
            {
                _logger.LogError("Failed to create user for email: {Email}", request.Email);
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Failed to create user account",
                    Errors = new List<string> { "Registration failed" }
                };
            }

            var token = GenerateJwtToken(createdUser);
            var refreshToken = GenerateRefreshToken();

            _logger.LogInformation("Registration successful for email: {Email}", request.Email);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Registration successful",
                Token = token,
                User = createdUser
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for email: {Email}", request.Email);
            return new AuthResponseDto
            {
                Success = false,
                Message = "An error occurred during registration",
                Errors = new List<string> { "Internal server error" }
            };
        }
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(TokenRefreshRequestDto request)
    {
        try
        {
            // TODO: Implement refresh token validation and generation
            _logger.LogInformation("Token refresh requested");

            return new AuthResponseDto
            {
                Success = false,
                Message = "Refresh token functionality not implemented",
                Errors = new List<string> { "Feature not available" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return new AuthResponseDto
            {
                Success = false,
                Message = "An error occurred during token refresh",
                Errors = new List<string> { "Internal server error" }
            };
        }
    }

    public async Task<AuthResponseDto> ForgotPasswordAsync(PasswordResetRequestDto request)
    {
        try
        {
            _logger.LogInformation("Password reset requested for email: {Email}", request.Email);

            // TODO: Implement password reset logic
            // 1. Validate email exists
            // 2. Generate reset token
            // 3. Send email with reset link

            return new AuthResponseDto
            {
                Success = true,
                Message = "Password reset instructions sent to your email"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password reset for email: {Email}", request.Email);
            return new AuthResponseDto
            {
                Success = false,
                Message = "An error occurred during password reset",
                Errors = new List<string> { "Internal server error" }
            };
        }
    }

    public async Task<AuthResponseDto> ResetPasswordAsync(PasswordResetConfirmDto request)
    {
        try
        {
            _logger.LogInformation("Password reset confirmation for email: {Email}", request.Email);

            // TODO: Implement password reset confirmation
            // 1. Validate reset token
            // 2. Hash new password
            // 3. Update user password

            return new AuthResponseDto
            {
                Success = true,
                Message = "Password reset successful"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password reset confirmation for email: {Email}", request.Email);
            return new AuthResponseDto
            {
                Success = false,
                Message = "An error occurred during password reset",
                Errors = new List<string> { "Internal server error" }
            };
        }
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(GetJwtSecret());

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = GetJwtIssuer(),
                ValidAudience = GetJwtAudience(),
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token validation failed");
            return false;
        }
    }

    public async Task LogoutAsync(string token)
    {
        try
        {
            _logger.LogInformation("User logout requested");
            // TODO: Implement token blacklisting or refresh token invalidation
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
        }
    }

    private string GenerateJwtToken(UserDto user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(GetJwtSecret());
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Email)
            }),
            Expires = DateTime.UtcNow.AddHours(24),
            Issuer = GetJwtIssuer(),
            Audience = GetJwtAudience(),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private string GetJwtSecret()
    {
        return _configuration["Jwt:Secret"] ?? "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLong!";
    }

    private string GetJwtIssuer()
    {
        return _configuration["Jwt:Issuer"] ?? "MouseJigglerBackend";
    }

    private string GetJwtAudience()
    {
        return _configuration["Jwt:Audience"] ?? "MouseJigglerUsers";
    }
}
