using Microsoft.AspNetCore.Identity;

namespace Svc.Identity.Entities;

public class User : IdentityUser<Guid>
{
    public Guid? EmployeeId { get; set; }
    //public string LastName { get; set; }
    //public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    //public ICollection<UserPermission> Permissions { get; set; } = new List<UserPermission>();
}
