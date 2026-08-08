namespace Svc.HRM.Payroll.Models.DTOs;

// ============ Core Module DTOs (Companies, Branches, Departments) ============

public class CoreApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public string[]? Errors { get; set; }
}

public class CompanyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAm { get; set; } = string.Empty;
    public string? TaxId { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? LogoUrl { get; set; }
}

public class BranchDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAm { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime OpenDate { get; set; }
    public string BranchType { get; set; } = string.Empty;
    public string BranchStat { get; set; } = string.Empty;
    public Guid CompId { get; set; }
}


public class DepartmentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAm { get; set; } = string.Empty;
    public string DeptStat { get; set; } = string.Empty;
    public Guid BranchId { get; set; }  // ? ADD THIS
    public string? Branch { get; set; }
    public string? BranchAm { get; set; }
}

// ============ Core HRMM DTOs (Positions, JobGrades) ============

public class HrmmApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public string[]? Errors { get; set; }
}

public class PositionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAm { get; set; } = string.Empty;
    public int NoOfPosition { get; set; }
    public string IsVacant { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }
    public Guid? JobGradeId { get; set; }
}

public class JobGradeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double StartSalary { get; set; }
    public double MaxSalary { get; set; }
}

// ============ HRM Pro DTOs (Employees, Persons) ============

public class HrmProApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public string[]? Errors { get; set; }
}

public class PersonDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string FirstNameAm { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string MiddleNameAm { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string LastNameAm { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
}

public class EmployeeDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;

    // IDs
    public Guid DepartmentId { get; set; }
    public Guid JobGradeId { get; set; }
    public Guid PositionId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid PersonId { get; set; }  // ? ADD THIS

    // ? PERSON FIELDS
    public string FirstName { get; set; } = string.Empty;
    public string FirstNameAm { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string MiddleNameAm { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string LastNameAm { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }

    // Display names
    public string? Position { get; set; }
    public string? Department { get; set; }
    public string? JobGrade { get; set; }
    public string? Branch { get; set; }

    // Employment fields
    public string? AppUserId { get; set; }
    public bool IsActive { get; set; }
    public string EmpState { get; set; } = string.Empty;
    public string EmploymentType { get; set; } = string.Empty;
    public string EmploymentNature { get; set; } = string.Empty;
    public string WorkArrangement { get; set; } = string.Empty;
    public DateTime EmploymentDate { get; set; }
}

// DTOs for Employee responses
public class EmployeeListDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public Guid? PositionId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? JobGradeId { get; set; }
    public string? AppUserId { get; set; }

    public string? PositionName { get; set; }
    public string? DepartmentName { get; set; }
    public string? JobGradeName { get; set; }
}

public class EmployeeDetailDto : EmployeeListDto
{
    public string? FirstNameAm { get; set; }
    public string? MiddleName { get; set; }
    public string? MiddleNameAm { get; set; }
    public string? LastNameAm { get; set; }
    public string? Phone { get; set; }
    public string? Gender { get; set; }
    public string? Nationality { get; set; }
    public string? EmploymentType { get; set; }
    public string? EmploymentNature { get; set; }
    public string? WorkArrangement { get; set; }
    public string? EmpState { get; set; }
    public DateTime? EmploymentDate { get; set; }
}

