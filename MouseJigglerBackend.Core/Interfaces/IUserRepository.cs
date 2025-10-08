using MouseJigglerBackend.Core.Entities;

namespace MouseJigglerBackend.Core.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdWithSubscriptionAsync(int id);
}
