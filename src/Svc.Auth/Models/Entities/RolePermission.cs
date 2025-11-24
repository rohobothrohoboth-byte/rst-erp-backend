namespace Svc.Auth.Models.Entities;

public class RolePermission : BaseEntity
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }

    //******************************************//

    public Role Role { get; set; } = default!;
    public Permission Permission { get; set; } = default!;
}