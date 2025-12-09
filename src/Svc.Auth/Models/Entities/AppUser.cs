using Microsoft.AspNetCore.Identity;

namespace Svc.Auth.Models.Entities;

public class AppUser : IdentityUser
{
    public Guid? EmployeeId { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<UserPerModule> PerModule { get; set; } = new List<UserPerModule>();
    public ICollection<UserPerMenu> PerMenu { get; set; } = new List<UserPerMenu>();
    public ICollection<UserPerApi> PerApi { get; set; } = new List<UserPerApi>();
}