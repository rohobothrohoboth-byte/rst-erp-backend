namespace Svc.Auth.Models.Entities;

public class Role : BaseEntity
{
    //public string Name { get; set; } = default!;


    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    // Navigation properties
    public List<Permission> Permissions { get; set; } = null;
    public List<User> Users { get; set; } = null;
}