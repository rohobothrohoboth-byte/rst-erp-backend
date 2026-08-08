// Svc.Auth.Models.Dtos/UserOrgDto.cs (NEW)

namespace Svc.Auth.Models.Dtos;

public class UserOrgDetailDto
{
    public string UserId { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }

    // Organization Info
    public Guid? EmployeeId { get; set; }
    public Guid? BranchId { get; set; }
    public string? BranchName { get; set; }
    public string? BranchCode { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public Guid? PositionId { get; set; }
    public string? PositionName { get; set; }
    public string? JobGradeName { get; set; }

    // Employee Details
    public string? EmploymentType { get; set; }
    public string? EmploymentNature { get; set; }
    public string? WorkArrangement { get; set; }
    public string? EmpState { get; set; }
    public DateTime? EmploymentDate { get; set; }

    // Position Permissions
    public List<string> PositionPermissions { get; set; } = new();
}

public class EmployeeOrgDto
{
    public Guid EmployeeId { get; set; }
    public string? EmployeeCode { get; set; }
    public string? EmploymentType { get; set; }
    public string? EmploymentNature { get; set; }
    public string? WorkArrangement { get; set; }
    public string? EmpState { get; set; }
    public DateTime? EmploymentDate { get; set; }

    public Guid? PositionId { get; set; }
    public string? PositionName { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public Guid? BranchId { get; set; }
    public string? BranchName { get; set; }
    public string? BranchCode { get; set; }
    public Guid? JobGradeId { get; set; }
    public string? JobGradeName { get; set; }
}


public class UserWithOrgDto
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public Guid? BranchId { get; set; }
    public string? BranchName { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public Guid? PositionId { get; set; }
    public string? PositionName { get; set; }
}


