using MouseJigglerBackend.Core.Entities;

namespace MouseJigglerBackend.Core.Interfaces;

public interface ISubscriptionRepository : IRepository<Subscription>
{
    Task<Subscription?> GetByUserIdAsync(int userId);
    Task<Subscription?> GetActiveByUserIdAsync(int userId);
    Task<bool> HasActiveSubscriptionAsync(int userId);
}
