using MouseJigglerBackend.Core.Entities;

namespace MouseJigglerBackend.Core.Interfaces;

public interface IActivationKeyRepository : IRepository<ActivationKey>
{
    Task<ActivationKey?> GetByKeyAsync(string key);
    Task<bool> IsKeyValidAsync(string key);
    Task<ActivationKey?> GetByKeyWithUserAsync(string key);
}
