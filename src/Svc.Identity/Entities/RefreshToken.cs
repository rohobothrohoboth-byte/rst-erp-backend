namespace Svc.Identity.Entities;

public class RefreshToken : BaseEntity
{
    public string Token { get; set; } = default!;
    public DateTime ExpiryDate { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime? RevokeDate { get; set; }
    public Guid UserId { get; set; }

    //******************************************//

    public User User { get; set; } = null!;
}