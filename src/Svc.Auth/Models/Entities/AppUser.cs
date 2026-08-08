using Microsoft.AspNetCore.Identity;

namespace Svc.Auth.Models.Entities;

public class AppUser : IdentityUser
{

        public Guid? EmployeeId { get; set; }
        public bool IsActive { get; set; } = true;

        // ?? NEW: Organizational Hierarchy Links
        public Guid? BranchId { get; set; }      // Which branch the user belongs to
        public Guid? DepartmentId { get; set; }  // Which department the user belongs to
        public Guid? PositionId { get; set; }    // Which position/role the user holds



        // Existing collections
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<UserPerModule> PerModule { get; set; } = new List<UserPerModule>();
        public ICollection<UserPerMenu> PerMenu { get; set; } = new List<UserPerMenu>();
        public ICollection<UserPerApi> PerApi { get; set; } = new List<UserPerApi>();
}