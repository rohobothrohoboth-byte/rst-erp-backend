using Microsoft.AspNetCore.Identity;

namespace RST.Auth.API.Models
{
    public class RolePermission
    {
        public string RoleId { get; set; } = null!;
        public IdentityRole Role { get; set; } = null!;
        public Guid PermissionId { get; set; }
        public Permission Permission { get; set; } = null!;
    }
}
