namespace Svc.Auth.Models.Entities;

public class User : BaseEntity
{
    //public Guid? EmployeeId { get; set; }
    //public string Username { get; set; } = default!;
    //public string PasswordHash { get; set; } = default!;

    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;

    // Navigation properties
    public List<Role> Roles { get; set; } = null;
    public List<Permission> Permissions { get; set; } = null;
    public List<RefreshToken> RefreshTokens { get; set; } = null;
}