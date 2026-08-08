// Svc.Auth/Models/Entities/Local/LocalEmployee.cs



namespace Svc.HRM.Payroll.Models.Entities.Local;

public class LocalEmployee : LocalBaseEntity
{
    public string Code { get; set; } = default!;
    public string EmploymentType { get; set; } = default!;
    public string EmploymentNature { get; set; } = default!;
    public string WorkArrangement { get; set; } = default!;
    public string EmpState { get; set; } = default!;
    public DateTime EmploymentDate { get; set; } = DateTime.UtcNow;
    public Guid PersonId { get; set; }
    public Guid JobGradeId { get; set; }
    public Guid PositionId { get; set; }
    public Guid DepartmentId { get; set; }
    public string? AppUserId { get; set; }

    // Person details (denormalized for quick access)
    public string FirstName { get; set; } = default!;
    public string FirstNameAm { get; set; } = "";
    public string MiddleName { get; set; } = default!;
    public string MiddleNameAm { get; set; } = "";
    public string LastName { get; set; } = default!;
    public string LastNameAm { get; set; } = "";
    public string Gender { get; set; } = default!;
    public string Nationality { get; set; } = default!;
    public string? Email { get; set; }
    public string? Phone { get; set; }

    public LocalPosition Position { get; set; } = null!;
    public LocalDepartment Department { get; set; } = null!;
    public LocalJobGrade JobGrade { get; set; } = null!;
}