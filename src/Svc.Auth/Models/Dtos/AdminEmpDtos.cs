namespace Svc.Auth.Models.Dtos;

public class AdminEmpListDto
{
    public string Id { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FirstNameAm { get; set; } = string.Empty;
    public string MiddleNameAm { get; set; } = string.Empty;
    public string LastNameAm { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Gender { get; set; }
    public string? EmpState { get; set; }
    public string? EmploymentType { get; set; }
    public string? EmploymentNature { get; set; }
    public string? WorkArrangement { get; set; }
    public Guid? PositionId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? JobGradeId { get; set; }
    public string? AppUserId { get; set; }
    public string? PositionName { get; set; }
    public string? DepartmentName { get; set; }
    public string? BranchName { get; set; }
    public string? JobGradeName { get; set; }
    public bool HasAccount { get; set; }
    public bool IsAccountActive { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public string RowVersion { get; set; } = string.Empty;

    // Computed properties for UI
    public string EmpFullName => $"{FirstName} {MiddleName} {LastName}".Trim();
    public string EmpFullNameAm => $"{FirstNameAm} {MiddleNameAm} {LastNameAm}".Trim();
    public string Branch => BranchName ?? "—";
    public string Department => DepartmentName ?? "—";
    public string Position => PositionName ?? "—";
    public string JobGrade => JobGradeName ?? "—";
    public string EmpType => EmploymentType ?? "";
    public string EmpNature => EmploymentNature ?? "";
    public string WorkArr => WorkArrangement ?? "";
    public string Photo => "";
}

public class AdminEmpDetailDto : AdminEmpListDto
{
    public string? Nationality { get; set; }
    public DateTime EmploymentDate { get; set; }
}