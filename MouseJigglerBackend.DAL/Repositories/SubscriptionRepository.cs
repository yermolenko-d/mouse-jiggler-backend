using Microsoft.EntityFrameworkCore;
using MouseJigglerBackend.Core.Entities;
using MouseJigglerBackend.Core.Interfaces;
using MouseJigglerBackend.DAL.Data;

namespace MouseJigglerBackend.DAL.Repositories;

public class SubscriptionRepository : Repository<Subscription>, ISubscriptionRepository
{
    public SubscriptionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Subscription?> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.UserId == userId && s.IsActive);
    }

    public async Task<Subscription?> GetActiveByUserIdAsync(int userId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.UserId == userId && 
                                     s.IsActive && 
                                     s.Status == SubscriptionStatus.Active &&
                                     (s.EndDate == null || s.EndDate > DateTime.UtcNow));
    }

    public async Task<bool> HasActiveSubscriptionAsync(int userId)
    {
        return await _dbSet
            .AnyAsync(s => s.UserId == userId && 
                          s.IsActive && 
                          s.Status == SubscriptionStatus.Active &&
                          (s.EndDate == null || s.EndDate > DateTime.UtcNow));
    }
}
