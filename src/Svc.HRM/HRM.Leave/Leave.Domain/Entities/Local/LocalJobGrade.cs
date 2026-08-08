// Svc.Auth/Models/Entities/Local/LocalJobGrade.cs
namespace Leave.Domain.Entities.Local;

public class LocalJobGrade : LocalBaseEntity
{
    public string Name { get; set; } = default!;
    public double StartSalary { get; set; }
    public double MaxSalary { get; set; }
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