namespace Svc.Auth.Models.Entities;

public class RefreshToken : BaseEntity
{
    public string UserId { get; set; } = default!;
    public string Token { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    //******************************************//

    public AppUser? User { get; set; }
}