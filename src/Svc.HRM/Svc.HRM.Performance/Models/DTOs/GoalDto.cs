namespace Svc.HRM.Performance.Models.DTOs;

public class GoalDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string Status { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime TargetDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public decimal ProgressPercent { get; set; }
    public string? Weight { get; set; }
    public Guid? ReviewId { get; set; }
    public string? Notes { get; set; }
}

public class GoalCreateDto
{
    public Guid EmployeeId { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime TargetDate { get; set; }
    public string? Weight { get; set; }
    public Guid? ReviewId { get; set; }
    public string? Notes { get; set; }
}

public class GoalUpdateDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public DateTime? TargetDate { get; set; }
    public decimal? ProgressPercent { get; set; }
    public string? Weight { get; set; }
    public string? Notes { get; set; }
}
