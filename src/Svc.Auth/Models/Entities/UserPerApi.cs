namespace Svc.Auth.Models.Entities;

public class UserPerApi : BaseEntity
{
    public string UserId { get; set; } = default!;
    public Guid PerApiId { get; set; }

    //******************************************//

    public AppUser User { get; set; } = null!;
    public PerApi PerApi { get; set; } = null!;
}