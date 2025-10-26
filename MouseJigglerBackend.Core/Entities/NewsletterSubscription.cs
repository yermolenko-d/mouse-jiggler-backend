namespace MouseJigglerBackend.Core.Entities;

public class NewsletterSubscription
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime SubscribedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? UnsubscribedAt { get; set; }
    
    // Optional: Add user reference for registered users
    public int? UserId { get; set; }
    public virtual User? User { get; set; }
}
