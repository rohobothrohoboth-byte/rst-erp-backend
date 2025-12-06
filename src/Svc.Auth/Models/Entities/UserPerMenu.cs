namespace Svc.Auth.Models.Entities;

public class UserPerMenu : BaseEntity
{
    public string UserId { get; set; } = default!;
    public Guid PerMenuId { get; set; }

    //******************************************//

    public AppUser User { get; set; } = null;
    public PerMenu PerMenu { get; set; } = null;
}