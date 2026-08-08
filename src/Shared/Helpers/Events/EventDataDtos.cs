namespace Shared.Helpers.Events;

// ============ CORE MODULE EVENTS ============

public class BranchEventData
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
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
}

public class DepartmentEventData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAm { get; set; } = string.Empty;
    public string DeptStat { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
}

public class CompanyEventData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAm { get; set; } = string.Empty;
    public string? TaxId { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? LogoUrl { get; set; }
    public bool IsDeleted { get; set; } = false;
}

// ============ CORE HRMM EVENTS ============

public class PositionEventData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAm { get; set; } = string.Empty;
    public int NoOfPosition { get; set; }
    public string IsVacant { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }
    public Guid? JobGradeId { get; set; }
    public bool IsDeleted { get; set; } = false;
}

public class JobGradeEventData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double StartSalary { get; set; }
    public double MaxSalary { get; set; }
    public bool IsDeleted { get; set; } = false;
}

// ============ HRM PRO EVENTS ============

public class EmployeeEventData
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string EmpState { get; set; } = string.Empty;
    public string EmploymentType { get; set; } = string.Empty;
    public string EmploymentNature { get; set; } = string.Empty;
    public string WorkArrangement { get; set; } = string.Empty;
    public DateTime EmploymentDate { get; set; }
    public Guid PersonId { get; set; }
    public Guid JobGradeId { get; set; }
    public Guid PositionId { get; set; }
    public Guid DepartmentId { get; set; }
    public string ?BranchId { get; set; }  // ✅ Added
    public Guid? AppUserId { get; set; }
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
    public bool IsActive { get; set; } = true;  // ✅ Added
    public bool IsDeleted { get; set; } = false;
}

public class PersonEventData
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
    public bool IsDeleted { get; set; } = false;
}
