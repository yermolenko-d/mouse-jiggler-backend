using MouseJigglerBackend.Core.DTOs;

namespace MouseJigglerBackend.Core.Interfaces;

public interface INewsletterSubscriptionService
{
    Task<NewsletterSubscriptionDto> SubscribeAsync(NewsletterSubscriptionRequestDto request);
    Task<bool> UnsubscribeAsync(string email);
    Task<bool> IsSubscribedAsync(string email);
    Task<IEnumerable<NewsletterSubscriptionDto>> GetActiveSubscriptionsAsync();
}
