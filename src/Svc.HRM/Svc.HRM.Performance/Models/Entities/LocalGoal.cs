namespace Svc.HRM.Performance.Models.Entities;

public class LocalGoal
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid EmployeeId { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string Status { get; set; } = "NotStarted";
    public DateTime StartDate { get; set; }
    public DateTime TargetDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public decimal ProgressPercent { get; set; }
    public string? Weight { get; set; }
    public Guid? ReviewId { get; set; }
    public string? Notes { get; set; }
    public DateTime DateAdd { get; set; } = DateTime.UtcNow;
    public DateTime? DateMod { get; set; }
    public bool IsDeleted { get; set; }
}
