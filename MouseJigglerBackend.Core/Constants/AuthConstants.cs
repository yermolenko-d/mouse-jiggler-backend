namespace MouseJigglerBackend.Core.Constants;

public static class AuthConstants
{
    // Error Messages
    public const string InvalidCredentials = "Invalid email or password";
    public const string InvalidCredentialsError = "Invalid credentials";
    public const string UserAlreadyExists = "User with this email already exists";
    public const string EmailAlreadyRegistered = "Email already registered";
    public const string UserCreationFailed = "Failed to create user account";
    public const string RegistrationFailed = "Registration failed";
    public const string RefreshTokenNotImplemented = "Refresh token functionality not implemented";
    public const string FeatureNotAvailable = "Feature not available";
    public const string PasswordResetInstructionsSent = "Password reset instructions sent to your email";
    public const string PasswordResetSuccessful = "Password reset successful";
    public const string InternalServerError = "Internal server error";
    
    // Success Messages
    public const string LoginSuccessful = "Login successful";
    public const string RegistrationSuccessful = "Registration successful. You are now logged in.";
    
    // Generic Error Messages
    public const string LoginError = "An error occurred during login";
    public const string RegistrationError = "An error occurred during registration";
    public const string TokenRefreshError = "An error occurred during token refresh";
    public const string PasswordResetError = "An error occurred during password reset";
}
