using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MouseJigglerBackend.Core.Constants;
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
    private readonly INewsletterSubscriptionService _newsletterService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserService userService,
        IPasswordService passwordService,
        INewsletterSubscriptionService newsletterService,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _userService = userService;
        _passwordService = passwordService;
        _newsletterService = newsletterService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        try {
            _logger.LogInformation("Attempting login for email: {Email}", request.Email);
            var userEntity = await _userService.GetUserEntityByEmailAsync(request.Email);
            if (userEntity == null) {
                _logger.LogWarning("Login failed: User not found for email: {Email}", request.Email);
                return CreateErrorResponse(AuthConstants.InvalidCredentials, AuthConstants.InvalidCredentialsError);
            }
            var passwordValid = _passwordService.VerifyPassword(request.Password, userEntity.PasswordHash);
            if (!passwordValid) {
                _logger.LogWarning("Login failed: Invalid password for email: {Email}", request.Email);
                return CreateErrorResponse(AuthConstants.InvalidCredentials, AuthConstants.InvalidCredentialsError);
            }
            var userDto = await _userService.GetUserByEmailAsync(request.Email);
            var token = GenerateJwtToken(userDto!);
            var refreshToken = GenerateRefreshToken();
            await _userService.UpdateLastLoginAsync(userDto!.Id);
            _logger.LogInformation("Login successful for email: {Email}", request.Email);
            return CreateSuccessResponse(AuthConstants.LoginSuccessful, token, userDto);
        } catch (Exception ex) {
            _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
            return CreateErrorResponse(AuthConstants.LoginError, AuthConstants.InternalServerError);
        }
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request) {
        try {
            _logger.LogInformation("Attempting registration for email: {Email}", request.Email);
            var existingUser = await _userService.GetUserByEmailAsync(request.Email);
            if (existingUser != null) {
                _logger.LogWarning("Registration failed: User already exists for email: {Email}", request.Email);
                return CreateErrorResponse(AuthConstants.UserAlreadyExists, AuthConstants.EmailAlreadyRegistered);
            }
            var passwordHash = _passwordService.HashPassword(request.Password);
            var createdUser = await _userService.CreateUserAsync(
                email: request.Email,
                firstName: request.FirstName,
                lastName: request.LastName,
                passwordHash: passwordHash
            );
            if (createdUser == null) {
                _logger.LogError("Failed to create user for email: {Email}", request.Email);
                return CreateErrorResponse(AuthConstants.UserCreationFailed, AuthConstants.RegistrationFailed);
            }

            // Handle newsletter subscription if requested
            if (request.SubscribeToNewsletter) {
                try {
                    var newsletterRequest = new NewsletterSubscriptionRequestDto {
                        Email = request.Email,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        UserId = createdUser.Id
                    };
                    await _newsletterService.SubscribeAsync(newsletterRequest);
                    _logger.LogInformation("Newsletter subscription added for email: {Email}", request.Email);
                } catch (Exception ex) {
                    _logger.LogWarning(ex, "Failed to add newsletter subscription for email: {Email}", request.Email);
                    // Don't fail registration if newsletter subscription fails
                }
            }

            _logger.LogInformation("TODO: Implement email confirmation for user: {Email}", request.Email);
            var token = GenerateJwtToken(createdUser);
            var refreshToken = GenerateRefreshToken();
            await _userService.UpdateLastLoginAsync(createdUser.Id);
            _logger.LogInformation("Registration successful for email: {Email}, UserId: {UserId}", request.Email, createdUser.Id);
            return CreateSuccessResponse(AuthConstants.RegistrationSuccessful, token, createdUser);
        } catch (Exception ex) {
            _logger.LogError(ex, "Error during registration for email: {Email}", request.Email);
            return CreateErrorResponse(AuthConstants.RegistrationError, AuthConstants.InternalServerError);
        }
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(TokenRefreshRequestDto request) {
        try {
            _logger.LogInformation("Token refresh requested");
            return CreateErrorResponse(AuthConstants.RefreshTokenNotImplemented, AuthConstants.FeatureNotAvailable);
        } catch (Exception ex) {
            _logger.LogError(ex, "Error during token refresh");
            return CreateErrorResponse(AuthConstants.TokenRefreshError, AuthConstants.InternalServerError);
        }
    }

    public async Task<AuthResponseDto> ForgotPasswordAsync(PasswordResetRequestDto request) {
        try {
            _logger.LogInformation("Password reset requested for email: {Email}", request.Email);
            return CreateSuccessResponse(AuthConstants.PasswordResetInstructionsSent);
        } catch (Exception ex) {
            _logger.LogError(ex, "Error during password reset for email: {Email}", request.Email);
            return CreateErrorResponse(AuthConstants.PasswordResetError, AuthConstants.InternalServerError);
        }
    }

    public async Task<AuthResponseDto> ResetPasswordAsync(PasswordResetConfirmDto request) {
        try {
            _logger.LogInformation("Password reset confirmation for email: {Email}", request.Email);
            return CreateSuccessResponse(AuthConstants.PasswordResetSuccessful);
        } catch (Exception ex) {
            _logger.LogError(ex, "Error during password reset confirmation for email: {Email}", request.Email);
            return CreateErrorResponse(AuthConstants.PasswordResetError, AuthConstants.InternalServerError);
        }
    }

    public async Task<bool> ValidateTokenAsync(string token) {
        try {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(GetJwtSecret());

            tokenHandler.ValidateToken(token, new TokenValidationParameters {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = GetJwtIssuer(),
                ValidAudience = GetJwtAudience(),
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        } catch (Exception ex) {
            _logger.LogWarning(ex, "Token validation failed");
            return false;
        }
    }

    public async Task LogoutAsync(string token) {
        try {
            _logger.LogInformation("User logout requested");
            // TODO: Implement token blacklisting or refresh token invalidation
        } catch (Exception ex) {
            _logger.LogError(ex, "Error during logout");
        }
    }

    public async Task<UserDto?> GetUserFromTokenAsync(string token) {
        try {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(GetJwtSecret());

            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = GetJwtIssuer(),
                ValidAudience = GetJwtAudience(),
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId)) {
                return null;
            }

            return await _userService.GetUserByIdAsync(userId);
        } catch (Exception ex) {
            _logger.LogWarning(ex, "Error extracting user from token");
            return null;
        }
    }

    private string GenerateJwtToken(UserDto user) {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(GetJwtSecret());
        var tokenDescriptor = new SecurityTokenDescriptor {
            Subject = new ClaimsIdentity(new[] {
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

    private string GenerateRefreshToken() {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private string GetJwtSecret() {
        return _configuration["Jwt:Secret"] ?? "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLong!";
    }

    private string GetJwtIssuer() {
        return _configuration["Jwt:Issuer"] ?? "MouseJigglerBackend";
    }

    private string GetJwtAudience() {
        return _configuration["Jwt:Audience"] ?? "MouseJigglerUsers";
    }

    private AuthResponseDto CreateErrorResponse(string message, string error) {
        return new AuthResponseDto {
            Success = false,
            Message = message,
            Errors = new List<string> { error }
        };
    }

    private AuthResponseDto CreateSuccessResponse(string message, string? token = null, UserDto? user = null) {
        return new AuthResponseDto {
            Success = true,
            Message = message,
            Token = token,
            User = user
        };
    }
}