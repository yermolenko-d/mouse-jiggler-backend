using MouseJigglerBackend.Core.DTOs;

namespace MouseJigglerBackend.Core.Interfaces;

public interface IActivationKeyService
{
    Task<ActivationKeyValidationResponse> ValidateActivationKeyAsync(ActivationKeyValidationRequest request);
    Task<bool> IsKeyValidAsync(string key);
    Task<ActivationKeyValidationResponse> CreateActivationKeyAsync(int userId, string? notes = null);
}
