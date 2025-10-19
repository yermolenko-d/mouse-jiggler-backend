using Microsoft.Extensions.Logging;
using MouseJigglerBackend.Core.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace MouseJigglerBackend.BLL.Services;

public class PasswordService : IPasswordService
{
    private readonly ILogger<PasswordService> _logger;
    private const int SaltSize = 16; // 128 bits
    private const int HashSize = 32; // 256 bits
    private const int Iterations = 100000; // PBKDF2 iterations

    public PasswordService(ILogger<PasswordService> logger)
    {
        _logger = logger;
    }

    public string HashPassword(string password)
    {
        try
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password cannot be null or empty", nameof(password));
            }

            // Generate a random salt
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);

            // Hash the password with the salt using PBKDF2
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(HashSize);

            // Combine salt and hash
            var hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            // Convert to base64 string
            var result = Convert.ToBase64String(hashBytes);
            
            _logger.LogDebug("Password hashed successfully");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error hashing password");
            throw;
        }
    }

    public bool VerifyPassword(string password, string hash)
    {
        try
        {
            if (string.IsNullOrEmpty(password))
            {
                _logger.LogWarning("Password verification failed: password is null or empty");
                return false;
            }

            if (string.IsNullOrEmpty(hash))
            {
                _logger.LogWarning("Password verification failed: hash is null or empty");
                return false;
            }

            // Convert hash from base64
            var hashBytes = Convert.FromBase64String(hash);

            if (hashBytes.Length != SaltSize + HashSize)
            {
                _logger.LogWarning("Password verification failed: invalid hash length");
                return false;
            }

            // Extract salt
            var salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            // Hash the provided password with the extracted salt
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var testHash = pbkdf2.GetBytes(HashSize);

            // Compare hashes using constant-time comparison
            var result = true;
            for (int i = 0; i < HashSize; i++)
            {
                if (hashBytes[SaltSize + i] != testHash[i])
                {
                    result = false;
                }
            }
            
            _logger.LogDebug("Password verification completed: {Result}", result ? "success" : "failed");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying password");
            return false;
        }
    }

    public string GenerateRandomPassword(int length = 12)
    {
        try
        {
            if (length < 8)
            {
                throw new ArgumentException("Password length must be at least 8 characters", nameof(length));
            }

            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@#$%^&*";
            
            using var rng = RandomNumberGenerator.Create();
            var chars = new char[length];
            var randomBytes = new byte[length];

            rng.GetBytes(randomBytes);

            for (int i = 0; i < length; i++)
            {
                chars[i] = validChars[randomBytes[i] % validChars.Length];
            }

            var password = new string(chars);
            _logger.LogDebug("Random password generated successfully");
            return password;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating random password");
            throw;
        }
    }
}
