namespace Svc.Auth.Models.Dtos;

// Svc.Auth.Models.Dtos/RegDtos.cs
public class RegStep1
{
    public string? EmployeeId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }

    // ?? ADD THESE:
    public string? RoleId { get; set; }
    public List<Guid>? PerModules { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? PositionId { get; set; }
}

public class RegStep2
{
    public string? EmployeeId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
    public string? PhoneNumber { get; set; }

    // ?? ADD THESE:
    public string? UserId { get; set; }
    public List<Guid>? PerMenus { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? PositionId { get; set; }
}

public class RegStep3
{
    public string? EmployeeId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
    public string? PhoneNumber { get; set; }
    public List<string>? Roles { get; set; }

    // ?? ADD THESE:
    public string? UserId { get; set; }
    public List<Guid>? PerAccess { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? PositionId { get; set; }
}


// Svc.Auth/Models/Dtos/AppUserWithOrgDto.cs
public class AppUserWithOrgDto
{
    public string Id { get; set; } = string.Empty;  // ? Changed to string
    public string? EmployeeId { get; set; }          // ? Changed to string
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? PositionId { get; set; }
    public string? BranchName { get; set; }
    public string? DepartmentName { get; set; }
    public string? PositionName { get; set; }
}

public class RegRes
{
    public string UserId { get; set; } = default!; //User Id
}