namespace MouseJigglerBackend.Core.Entities;

public class Subscription
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public SubscriptionStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string? StripeSubscriptionId { get; set; }
    public string? StripeCustomerId { get; set; }
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
}

public enum SubscriptionStatus
{
    Inactive = 0,
    Active = 1,
    Cancelled = 2,
    Expired = 3,
    Suspended = 4
}
