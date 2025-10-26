using Microsoft.EntityFrameworkCore;
using MouseJigglerBackend.Core.Entities;
using MouseJigglerBackend.Core.Interfaces;
using MouseJigglerBackend.DAL.Data;

namespace MouseJigglerBackend.DAL.Repositories;

public class ActivationKeyRepository : Repository<ActivationKey>, IActivationKeyRepository
{
    public ActivationKeyRepository(ApplicationDbContext context) : base(context) {
    }

    public async Task<ActivationKey?> GetByKeyAsync(string key) {
        return await _dbSet
            .FirstOrDefaultAsync(ak => ak.Key == key && ak.IsActive);
    }

    public async Task<bool> IsKeyValidAsync(string key) {
        return await _dbSet
            .AnyAsync(ak => ak.Key == key && 
                           ak.IsActive && 
                           ak.ActivatedAt.HasValue &&
                           (ak.ExpiresAt == null || ak.ExpiresAt > DateTime.UtcNow));
    }

    public async Task<ActivationKey?> GetByKeyWithUserAsync(string key) {
        return await _dbSet
            .Include(ak => ak.User)
            .ThenInclude(u => u.Subscription)
            .FirstOrDefaultAsync(ak => ak.Key == key && ak.IsActive);
    }
}
