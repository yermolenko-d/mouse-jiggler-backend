using MouseJigglerBackend.Core.DTOs;
using MouseJigglerBackend.Core.Entities;
using MouseJigglerBackend.Core.Interfaces;

namespace MouseJigglerBackend.BLL.Services;

public class ActivationKeyService : IActivationKeyService
{
    private readonly IActivationKeyRepository _activationKeyRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;

    public ActivationKeyService(
        IActivationKeyRepository activationKeyRepository,
        IUserRepository userRepository,
        ISubscriptionRepository subscriptionRepository)
    {
        _activationKeyRepository = activationKeyRepository;
        _userRepository = userRepository;
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<ActivationKeyValidationResponse> ValidateActivationKeyAsync(ActivationKeyValidationRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.ActivationKey))
            {
                return new ActivationKeyValidationResponse
                {
                    Valid = false,
                    Error = "Activation key is required"
                };
            }

            var activationKey = await _activationKeyRepository.GetByKeyWithUserAsync(request.ActivationKey);
            
            if (activationKey == null)
            {
                return new ActivationKeyValidationResponse
                {
                    Valid = false,
                    Error = "Invalid activation key"
                };
            }

            // Check if key is activated
            if (!activationKey.ActivatedAt.HasValue)
            {
                return new ActivationKeyValidationResponse
                {
                    Valid = false,
                    Error = "Activation key has not been activated"
                };
            }

            // Check if key is expired
            if (activationKey.ExpiresAt.HasValue && activationKey.ExpiresAt < DateTime.UtcNow)
            {
                return new ActivationKeyValidationResponse
                {
                    Valid = false,
                    Error = "Activation key has expired"
                };
            }

            // Check user subscription
            var hasActiveSubscription = await _subscriptionRepository.HasActiveSubscriptionAsync(activationKey.UserId);
            
            if (!hasActiveSubscription)
            {
                return new ActivationKeyValidationResponse
                {
                    Valid = false,
                    SubscriptionActive = false,
                    SubscriptionStatus = "inactive",
                    Error = "Subscription is inactive or expired"
                };
            }

            // Get user with subscription details
            var user = await _userRepository.GetByIdWithSubscriptionAsync(activationKey.UserId);
            
            if (user == null)
            {
                return new ActivationKeyValidationResponse
                {
                    Valid = false,
                    Error = "User not found"
                };
            }

            return new ActivationKeyValidationResponse
            {
                Valid = true,
                SubscriptionActive = true,
                SubscriptionStatus = user.Subscription?.Status.ToString().ToLower() ?? "active",
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Subscription = user.Subscription != null ? new SubscriptionDto
                    {
                        PlanName = user.Subscription.PlanName,
                        Status = user.Subscription.Status.ToString().ToLower(),
                        StartDate = user.Subscription.StartDate,
                        EndDate = user.Subscription.EndDate
                    } : null
                },
                Message = "Activation successful"
            };
        }
        catch (Exception ex)
        {
            return new ActivationKeyValidationResponse
            {
                Valid = false,
                Error = $"An error occurred: {ex.Message}"
            };
        }
    }

    public async Task<bool> IsKeyValidAsync(string key)
    {
        return await _activationKeyRepository.IsKeyValidAsync(key);
    }

    public async Task<ActivationKeyValidationResponse> CreateActivationKeyAsync(int userId, string? notes = null)
    {
        try
        {
            var key = GenerateActivationKey();
            var activationKey = new ActivationKey
            {
                Key = key,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ActivatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddYears(1), // 1 year expiry
                IsActive = true,
                Notes = notes
            };

            await _activationKeyRepository.AddAsync(activationKey);

            return new ActivationKeyValidationResponse
            {
                Valid = true,
                Message = "Activation key created successfully"
            };
        }
        catch (Exception ex)
        {
            return new ActivationKeyValidationResponse
            {
                Valid = false,
                Error = $"Failed to create activation key: {ex.Message}"
            };
        }
    }

    private static string GenerateActivationKey()
    {
        // Generate a 16-character key in format XXXX-XXXX-XXXX-XXXX
        var random = new Random();
        var key = "";
        
        for (int i = 0; i < 4; i++)
        {
            if (i > 0) key += "-";
            key += random.Next(1000, 9999).ToString();
        }
        
        return key;
    }
}
