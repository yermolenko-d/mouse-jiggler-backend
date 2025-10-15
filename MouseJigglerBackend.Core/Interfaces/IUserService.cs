using MouseJigglerBackend.Core.DTOs;

namespace MouseJigglerBackend.Core.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<UserDto> CreateUserAsync(string email, string firstName, string lastName);
    Task<bool> UserExistsAsync(string email);
    Task<IEnumerable<UserDto>> GetUsersByEmailFilterAsync(IEnumerable<string> emailFilters);
}
