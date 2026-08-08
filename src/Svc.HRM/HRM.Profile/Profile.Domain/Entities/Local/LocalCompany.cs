namespace Profile.Domain.Entities.Local;

public class LocalCompany
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAm { get; set; } = string.Empty;
    public string? TaxId { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? LogoUrl { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime SyncedAt { get; set; } = DateTime.UtcNow;
}

public class LocalBranch
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
    public bool IsDeleted { get; set; }
    public DateTime SyncedAt { get; set; } = DateTime.UtcNow;
}

public class LocalDepartment
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAm { get; set; } = string.Empty;
    public string DeptStat { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime SyncedAt { get; set; } = DateTime.UtcNow;
}

public class LocalPosition
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAm { get; set; } = string.Empty;
    public int NoOfPosition { get; set; }
    public string IsVacant { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }
    public Guid? JobGradeId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime SyncedAt { get; set; } = DateTime.UtcNow;
}

public class LocalJobGrade
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double StartSalary { get; set; }
    public double MaxSalary { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime SyncedAt { get; set; } = DateTime.UtcNow;
}

public class LocalJgStep
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Salary { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string SalaryPayFreq { get; set; } = string.Empty;
    public Guid JobGradeId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime SyncedAt { get; set; } = DateTime.UtcNow;
}