namespace Svc.HRM.Performance.Models.Entities;

public class Goal : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public string Status { get; set; } = "Active";
    public int Progress { get; set; }
    public Guid? KpiId { get; set; }
}
