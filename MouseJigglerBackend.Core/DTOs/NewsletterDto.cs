using System.ComponentModel.DataAnnotations;

namespace MouseJigglerBackend.Core.DTOs;

public class NewsletterSubscriptionDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime SubscribedAt { get; set; }
    public bool IsActive { get; set; }
    public DateTime? UnsubscribedAt { get; set; }
    public int? UserId { get; set; }
}

public class NewsletterSubscriptionRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int? UserId { get; set; }
}