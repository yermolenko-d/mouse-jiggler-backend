using Microsoft.AspNetCore.Mvc;
using MouseJigglerBackend.Core.DTOs;
using MouseJigglerBackend.Core.Interfaces;

namespace MouseJigglerBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// User login endpoint
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>Authentication response with token</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    [ProducesResponseType(typeof(AuthResponseDto), 400)]
    [ProducesResponseType(typeof(AuthResponseDto), 401)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            var result = await _authService.LoginAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return Unauthorized(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in login endpoint");
            return StatusCode(500, new AuthResponseDto
            {
                Success = false,
                Message = "An internal server error occurred",
                Errors = new List<string> { "Internal server error" }
            });
        }
    }

    /// <summary>
    /// User registration endpoint
    /// </summary>
    /// <param name="request">Registration data</param>
    /// <returns>Authentication response with token</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    [ProducesResponseType(typeof(AuthResponseDto), 400)]
    [ProducesResponseType(typeof(AuthResponseDto), 409)]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            var result = await _authService.RegisterAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            if (result.Errors.Contains("Email already registered"))
            {
                return Conflict(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in register endpoint");
            return StatusCode(500, new AuthResponseDto
            {
                Success = false,
                Message = "An internal server error occurred",
                Errors = new List<string> { "Internal server error" }
            });
        }
    }

    /// <summary>
    /// Refresh authentication token
    /// </summary>
    /// <param name="request">Refresh token request</param>
    /// <returns>New authentication response with token</returns>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    [ProducesResponseType(typeof(AuthResponseDto), 400)]
    [ProducesResponseType(typeof(AuthResponseDto), 401)]
    public async Task<IActionResult> RefreshToken([FromBody] TokenRefreshRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            var result = await _authService.RefreshTokenAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return Unauthorized(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in refresh-token endpoint");
            return StatusCode(500, new AuthResponseDto
            {
                Success = false,
                Message = "An internal server error occurred",
                Errors = new List<string> { "Internal server error" }
            });
        }
    }

    /// <summary>
    /// Request password reset
    /// </summary>
    /// <param name="request">Password reset request</param>
    /// <returns>Password reset response</returns>
    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    [ProducesResponseType(typeof(AuthResponseDto), 400)]
    public async Task<IActionResult> ForgotPassword([FromBody] PasswordResetRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            var result = await _authService.ForgotPasswordAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in forgot-password endpoint");
            return StatusCode(500, new AuthResponseDto
            {
                Success = false,
                Message = "An internal server error occurred",
                Errors = new List<string> { "Internal server error" }
            });
        }
    }

    /// <summary>
    /// Confirm password reset
    /// </summary>
    /// <param name="request">Password reset confirmation</param>
    /// <returns>Password reset confirmation response</returns>
    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    [ProducesResponseType(typeof(AuthResponseDto), 400)]
    [ProducesResponseType(typeof(AuthResponseDto), 401)]
    public async Task<IActionResult> ResetPassword([FromBody] PasswordResetConfirmDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            var result = await _authService.ResetPasswordAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in reset-password endpoint");
            return StatusCode(500, new AuthResponseDto
            {
                Success = false,
                Message = "An internal server error occurred",
                Errors = new List<string> { "Internal server error" }
            });
        }
    }

    /// <summary>
    /// Validate authentication token
    /// </summary>
    /// <param name="token">JWT token to validate</param>
    /// <returns>Token validation result</returns>
    [HttpPost("validate-token")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 401)]
    public async Task<IActionResult> ValidateToken([FromBody] string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { valid = false, message = "Token is required" });
            }

            var isValid = await _authService.ValidateTokenAsync(token);

            if (isValid)
            {
                return Ok(new { valid = true, message = "Token is valid" });
            }

            return Unauthorized(new { valid = false, message = "Token is invalid or expired" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in validate-token endpoint");
            return StatusCode(500, new { valid = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// User logout
    /// </summary>
    /// <param name="token">JWT token to invalidate</param>
    /// <returns>Logout response</returns>
    [HttpPost("logout")]
    [ProducesResponseType(typeof(object), 200)]
    public async Task<IActionResult> Logout([FromBody] string token)
    {
        try
        {
            await _authService.LogoutAsync(token);
            return Ok(new { success = true, message = "Logout successful" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in logout endpoint");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get current authenticated user information
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>Current user information</returns>
    [HttpPost("current-user")]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> GetCurrentUser([FromBody] string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { success = false, message = "Token is required" });
            }

            var user = await _authService.GetUserFromTokenAsync(token);

            if (user == null)
            {
                return Unauthorized(new { success = false, message = "Invalid or expired token" });
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in current-user endpoint");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }
}
