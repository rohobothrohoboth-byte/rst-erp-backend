namespace Svc.HRM.Performance.Models.DTOs;

public class PerformanceReviewDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid? ReviewerId { get; set; }
    public Guid? TemplateId { get; set; }
    public string Title { get; set; } = default!;
    public string Status { get; set; } = default!;
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
}

public class PerformanceReviewCreateDto
{
    public Guid EmployeeId { get; set; }
    public Guid? ReviewerId { get; set; }
    public Guid? TemplateId { get; set; }
    public string Title { get; set; } = default!;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public string? Summary { get; set; }
}

public class PerformanceReviewUpdateDto
{
    public string? Title { get; set; }
    public Guid? ReviewerId { get; set; }
    public string? OverallRating { get; set; }
    public decimal? Score { get; set; }
    public string? Summary { get; set; }
    public string? EmployeeComments { get; set; }
    public string? ManagerComments { get; set; }
}

public class ReviewDecisionDto
{
    public Guid? ReviewerId { get; set; }
    public string? Comments { get; set; }
    public string? OverallRating { get; set; }
    public decimal? Score { get; set; }
    public string? RejectionReason { get; set; }
}
