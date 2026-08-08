namespace Svc.HRM.Attendance.Models.Entities.Local;

public class LocalPosition : LocalBaseEntity
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public int NoOfPosition { get; set; }
    public string IsVacant { get; set; } = default!;
    public Guid DepartmentId { get; set; }
    public Guid? JobGradeId { get; set; }

    public LocalDepartment Department { get; set; } = null!;
    public LocalJobGrade JobGrade { get; set; } = null!;
}