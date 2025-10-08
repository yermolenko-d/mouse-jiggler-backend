using MouseJigglerBackend.Core.DTOs;
using MouseJigglerBackend.Core.Entities;
using MouseJigglerBackend.Core.Interfaces;

namespace MouseJigglerBackend.BLL.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public SubscriptionService(ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<SubscriptionDto?> GetUserSubscriptionAsync(int userId)
    {
        var subscription = await _subscriptionRepository.GetByUserIdAsync(userId);
        return subscription != null ? MapToSubscriptionDto(subscription) : null;
    }

    public async Task<bool> HasActiveSubscriptionAsync(int userId)
    {
        return await _subscriptionRepository.HasActiveSubscriptionAsync(userId);
    }

    public async Task<SubscriptionDto> CreateSubscriptionAsync(int userId, string planName)
    {
        var subscription = new Subscription
        {
            UserId = userId,
            PlanName = planName,
            Status = SubscriptionStatus.Active,
            StartDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var createdSubscription = await _subscriptionRepository.AddAsync(subscription);
        return MapToSubscriptionDto(createdSubscription);
    }

    public async Task<bool> UpdateSubscriptionStatusAsync(int userId, string status)
    {
        try
        {
            var subscription = await _subscriptionRepository.GetByUserIdAsync(userId);
            if (subscription == null) return false;

            if (Enum.TryParse<SubscriptionStatus>(status, true, out var subscriptionStatus))
            {
                subscription.Status = subscriptionStatus;
                subscription.UpdatedAt = DateTime.UtcNow;
                await _subscriptionRepository.UpdateAsync(subscription);
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    private static SubscriptionDto MapToSubscriptionDto(Subscription subscription)
    {
        return new SubscriptionDto
        {
            PlanName = subscription.PlanName,
            Status = subscription.Status.ToString().ToLower(),
            StartDate = subscription.StartDate,
            EndDate = subscription.EndDate
        };
    }
}
