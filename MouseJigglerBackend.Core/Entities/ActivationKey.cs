namespace MouseJigglerBackend.Core.Entities;

public class ActivationKey
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ActivatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
}
