using MouseJigglerBackend.Core.Entities;

namespace MouseJigglerBackend.Core.Interfaces;

public interface INewsletterSubscriptionRepository : IRepository<NewsletterSubscription>
{
    Task<NewsletterSubscription?> GetByEmailAsync(string email);
    Task<bool> IsSubscribedAsync(string email);
    Task<IEnumerable<NewsletterSubscription>> GetActiveSubscriptionsAsync();
}
