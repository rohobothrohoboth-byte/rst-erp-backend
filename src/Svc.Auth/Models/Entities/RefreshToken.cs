namespace Svc.Auth.Models.Entities;

public class RefreshToken : BaseEntity
{
    //public string Token { get; set; } = default!;
    //public Guid UserId { get; set; }
    //public DateTime ExpiresTime { get; set; }
    //public DateTime? RevokedTime { get; set; }
    ////public bool IsActive => RevokedAt == null && ExpiresAt > DateTimeOffset.UtcNow;

    ////******************************************//

    //public User User { get; set; } = null!;

    public Guid UserId { get; set; }
    public string Token { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation property
    public User? User { get; set; }
}