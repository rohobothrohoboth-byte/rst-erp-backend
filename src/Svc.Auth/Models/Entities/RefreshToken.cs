namespace Svc.Auth.Models.Entities;

public class RefreshToken : BaseEntity
{
    public string UserId { get; set; } = default!; 
    public string Token { get; set; } = default!; 
    public DateTime ExpiryDate { get; set; }
    public bool IsRevoked { get; set; } = false;
    public DateTime? RevokedDate { get; set; }

    //******************************************//

    public AppUser User { get; set; } = null!;
}