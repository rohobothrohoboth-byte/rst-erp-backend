using Microsoft.AspNetCore.Identity;

namespace Svc.Auth.Models.Entities;

public class AppUser : IdentityUser
{
    public Guid? EmployeeId { get; set; }
    public bool IsActive { get; set; } = default!;
}