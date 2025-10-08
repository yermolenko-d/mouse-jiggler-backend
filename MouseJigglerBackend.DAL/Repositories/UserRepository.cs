using Microsoft.EntityFrameworkCore;
using MouseJigglerBackend.Core.Entities;
using MouseJigglerBackend.Core.Interfaces;
using MouseJigglerBackend.DAL.Data;

namespace MouseJigglerBackend.DAL.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
    }

    public async Task<User?> GetByIdWithSubscriptionAsync(int id)
    {
        return await _dbSet
            .Include(u => u.Subscription)
            .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
    }
}
