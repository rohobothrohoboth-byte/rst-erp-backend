namespace Svc.HRM.Performance.Models.Entities;

public class LocalKPI
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid EmployeeId { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string MetricUnit { get; set; } = "%";
    public decimal TargetValue { get; set; }
    public decimal ActualValue { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public Guid? GoalId { get; set; }
    public string? Notes { get; set; }
    public DateTime DateAdd { get; set; } = DateTime.UtcNow;
    public DateTime? DateMod { get; set; }
    public bool IsDeleted { get; set; }
}
