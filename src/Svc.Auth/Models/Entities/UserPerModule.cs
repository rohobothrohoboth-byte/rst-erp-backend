namespace Svc.Auth.Models.Entities;

public class UserPerModule : BaseEntity
{
    public string UserId { get; set; } = default!;
    public Guid PerModuleId { get; set; }

    //******************************************//

    public AppUser User { get; set; } = null!;
    public PerModule PerModule { get; set; } = null!;
}