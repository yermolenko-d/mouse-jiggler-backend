using Microsoft.EntityFrameworkCore;
using MouseJigglerBackend.Core.Entities;
using MouseJigglerBackend.Core.Interfaces;
using MouseJigglerBackend.DAL.Data;

namespace MouseJigglerBackend.DAL.Repositories;

public class NewsletterSubscriptionRepository : Repository<NewsletterSubscription>, INewsletterSubscriptionRepository
{
    public NewsletterSubscriptionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<NewsletterSubscription?> GetByEmailAsync(string email) {
        return await _context.NewsletterSubscriptions
            .FirstOrDefaultAsync(ns => ns.Email == email);
    }

    public async Task<bool> IsSubscribedAsync(string email) {
        return await _context.NewsletterSubscriptions
            .AnyAsync(ns => ns.Email == email && ns.IsActive);
    }

    public async Task<IEnumerable<NewsletterSubscription>> GetActiveSubscriptionsAsync() {
        return await _context.NewsletterSubscriptions
            .Where(ns => ns.IsActive)
            .ToListAsync();
    }
}