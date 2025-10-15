using MouseJigglerBackend.Core.DTOs;
using MouseJigglerBackend.Core.Entities;
using MouseJigglerBackend.Core.Interfaces;

namespace MouseJigglerBackend.BLL.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;

    public UserService(IUserRepository userRepository, ISubscriptionRepository subscriptionRepository)
    {
        _userRepository = userRepository;
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdWithSubscriptionAsync(id);
        return user != null ? MapToUserDto(user) : null;
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        return user != null ? MapToUserDto(user) : null;
    }

    public async Task<UserDto> CreateUserAsync(string email, string firstName, string lastName)
    {
        var user = new User
        {
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var createdUser = await _userRepository.AddAsync(user);
        return MapToUserDto(createdUser);
    }

    public async Task<bool> UserExistsAsync(string email)
    {
        return await _userRepository.ExistsAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<UserDto>> GetUsersByEmailFilterAsync(IEnumerable<string> emailFilters)
    {
        var users = await _userRepository.FindAsync(u => emailFilters.Any(filter => u.Email.Contains(filter)));
        return users.Select(MapToUserDto);
    }

    private static UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Subscription = user.Subscription != null ? new SubscriptionDto
            {
                PlanName = user.Subscription.PlanName,
                Status = user.Subscription.Status.ToString().ToLower(),
                StartDate = user.Subscription.StartDate,
                EndDate = user.Subscription.EndDate
            } : null
        };
    }
}
