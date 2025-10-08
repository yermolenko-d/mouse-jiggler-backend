namespace MouseJigglerBackend.Core.DTOs;

public class ActivationKeyValidationResponse
{
    public bool Valid { get; set; }
    public bool SubscriptionActive { get; set; }
    public string SubscriptionStatus { get; set; } = string.Empty;
    public UserDto? User { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Error { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public SubscriptionDto? Subscription { get; set; }
}

public class SubscriptionDto
{
    public string PlanName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
