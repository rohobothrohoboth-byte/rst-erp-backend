namespace Svc.HRM.Performance.Models.Entities;

public class LocalPerformanceReview
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid EmployeeId { get; set; }
    public Guid? ReviewerId { get; set; }
    public Guid? TemplateId { get; set; }
    public string Title { get; set; } = default!;
    public string Status { get; set; } = "Draft";
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? OverallRating { get; set; }
    public decimal? Score { get; set; }
    public string? Summary { get; set; }
    public string? EmployeeComments { get; set; }
    public string? ManagerComments { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime DateAdd { get; set; } = DateTime.UtcNow;
    public DateTime? DateMod { get; set; }
    public bool IsDeleted { get; set; }
}
