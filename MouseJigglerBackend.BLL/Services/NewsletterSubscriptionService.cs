using MouseJigglerBackend.Core.DTOs;
using MouseJigglerBackend.Core.Entities;
using MouseJigglerBackend.Core.Interfaces;

namespace MouseJigglerBackend.BLL.Services;

public class NewsletterSubscriptionService : INewsletterSubscriptionService
{
    private readonly INewsletterSubscriptionRepository _newsletterRepository;

    public NewsletterSubscriptionService(INewsletterSubscriptionRepository newsletterRepository) {
        _newsletterRepository = newsletterRepository;
    }

    public async Task<NewsletterSubscriptionDto> SubscribeAsync(NewsletterSubscriptionRequestDto request) {
        var existingSubscription = await _newsletterRepository.GetByEmailAsync(request.Email);
        
        if (existingSubscription != null) {
            if (!existingSubscription.IsActive) {
                // Reactivate existing subscription
                existingSubscription.IsActive = true;
                existingSubscription.UnsubscribedAt = null;
                existingSubscription.FirstName = request.FirstName;
                existingSubscription.LastName = request.LastName;
                existingSubscription.UserId = request.UserId;
                await _newsletterRepository.UpdateAsync(existingSubscription);
            }
            return MapToDto(existingSubscription);
        }

        // Create new subscription
        var subscription = new NewsletterSubscription {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            SubscribedAt = DateTime.UtcNow,
            IsActive = true,
            UserId = request.UserId
        };

        var createdSubscription = await _newsletterRepository.AddAsync(subscription);
        return MapToDto(createdSubscription);
    }

    public async Task<bool> UnsubscribeAsync(string email) {
        var subscription = await _newsletterRepository.GetByEmailAsync(email);
        if (subscription == null || !subscription.IsActive) {
            return false;
        }

        subscription.IsActive = false;
        subscription.UnsubscribedAt = DateTime.UtcNow;
        await _newsletterRepository.UpdateAsync(subscription);
        return true;
    }

    public async Task<bool> IsSubscribedAsync(string email) {
        return await _newsletterRepository.IsSubscribedAsync(email);
    }

    public async Task<IEnumerable<NewsletterSubscriptionDto>> GetActiveSubscriptionsAsync() {
        var subscriptions = await _newsletterRepository.GetActiveSubscriptionsAsync();
        return subscriptions.Select(MapToDto);
    }

    private static NewsletterSubscriptionDto MapToDto(NewsletterSubscription subscription) {
        return new NewsletterSubscriptionDto {
            Id = subscription.Id,
            Email = subscription.Email,
            FirstName = subscription.FirstName,
            LastName = subscription.LastName,
            SubscribedAt = subscription.SubscribedAt,
            IsActive = subscription.IsActive,
            UnsubscribedAt = subscription.UnsubscribedAt,
            UserId = subscription.UserId
        };
    }
}
