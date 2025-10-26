using MouseJigglerBackend.Core.DTOs;
using MouseJigglerBackend.Core.Entities;

namespace MouseJigglerBackend.Core.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<User?> GetUserEntityByEmailAsync(string email);
    Task<UserDto> CreateUserAsync(string email, string firstName, string lastName, string passwordHash);
    Task<bool> UserExistsAsync(string email);
    Task<IEnumerable<UserDto>> GetUsersByEmailFilterAsync(IEnumerable<string> emailFilters);
    Task UpdateLastLoginAsync(int userId);
}
