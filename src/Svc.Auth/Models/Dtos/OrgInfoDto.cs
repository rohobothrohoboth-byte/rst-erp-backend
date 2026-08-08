// Svc.Auth.Models.Dtos/OrgInfoDto.cs (NEW)

namespace Svc.Auth.Models.Dtos;

public class UserOrgInfoDto
{
    public string BranchId { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string BranchCode { get; set; } = string.Empty;
    public string DepartmentId { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string PositionId { get; set; } = string.Empty;
    public string PositionName { get; set; } = string.Empty;
    public string JobGradeId { get; set; } = string.Empty;
    public string JobGradeName { get; set; } = string.Empty;
}

// For login response
public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string? EmployeeId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }

    // Org Info
    public string? BranchId { get; set; }
    public string? BranchName { get; set; }
    public string? BranchCode { get; set; }
    public string? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public string? PositionId { get; set; }
    public string? PositionName { get; set; }
    public string? JobGradeId { get; set; }
    public string? JobGradeName { get; set; }
}




public class OrgInfoDto
{
    public string? BranchName { get; set; }
    public string? DepartmentName { get; set; }
    public string? PositionName { get; set; }
}

 // DTOs for updating org fields
   public class UpdateBranchDto
   {
       public Guid BranchId { get; set; }
   }

   public class UpdateDepartmentDto
   {
       public Guid DepartmentId { get; set; }
   }

   public class UpdatePositionDto
   {
       public Guid PositionId { get; set; }
   }

// DTOs
public class UserOrgIdsDto
{
    public Guid? BranchId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? PositionId { get; set; }
}
// DTOs
public class BranchInfo
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}

public class DepartmentInfo
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
}

public class PositionInfo
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }
}