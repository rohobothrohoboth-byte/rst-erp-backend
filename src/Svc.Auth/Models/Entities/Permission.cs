namespace Svc.Auth.Models.Entities;

public class Permission : BaseEntity
{
    //public string Name { get; set; } = default!;
    //public string Desc { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    // Navigation properties
    public List<Role> Roles { get; set; } = null;
    public List<User> Users { get; set; } = null;
}