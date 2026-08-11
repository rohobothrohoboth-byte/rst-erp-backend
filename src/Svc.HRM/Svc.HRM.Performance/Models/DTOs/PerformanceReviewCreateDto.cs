namespace Svc.HRM.Performance.Models.DTOs;

public class PerformanceReviewCreateDto
{
    public Guid EmployeeId { get; set; }
    public Guid ReviewerId { get; set; }
    public string Period { get; set; } = string.Empty;
    public decimal? OverallScore { get; set; }
    public string Status { get; set; } = "Draft";
    public string? Comments { get; set; }
    public DateTime? ReviewDate { get; set; }
}
