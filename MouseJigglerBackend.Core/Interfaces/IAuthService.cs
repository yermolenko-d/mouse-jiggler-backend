using MouseJigglerBackend.Core.DTOs;

namespace MouseJigglerBackend.Core.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto> RefreshTokenAsync(TokenRefreshRequestDto request);
    Task<AuthResponseDto> ForgotPasswordAsync(PasswordResetRequestDto request);
    Task<AuthResponseDto> ResetPasswordAsync(PasswordResetConfirmDto request);
    Task<bool> ValidateTokenAsync(string token);
    Task LogoutAsync(string token);
}
