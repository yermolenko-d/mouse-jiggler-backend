using MouseJigglerBackend.Core.DTOs;

namespace MouseJigglerBackend.Core.Interfaces;

public interface ISubscriptionService
{
    Task<SubscriptionDto?> GetUserSubscriptionAsync(int userId);
    Task<bool> HasActiveSubscriptionAsync(int userId);
    Task<SubscriptionDto> CreateSubscriptionAsync(int userId, string planName);
    Task<bool> UpdateSubscriptionStatusAsync(int userId, string status);
}
