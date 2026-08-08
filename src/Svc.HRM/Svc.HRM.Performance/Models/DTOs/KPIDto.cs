namespace Svc.HRM.Performance.Models.DTOs;

public class KPIDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string MetricUnit { get; set; } = default!;
    public decimal TargetValue { get; set; }
    public decimal ActualValue { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public Guid? GoalId { get; set; }
    public string? Notes { get; set; }
    public decimal AchievementPercent => TargetValue == 0 ? 0 : Math.Round(ActualValue / TargetValue * 100, 2);
}

public class KPICreateDto
{
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
}

public class KPIUpdateDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? MetricUnit { get; set; }
    public decimal? TargetValue { get; set; }
    public decimal? ActualValue { get; set; }
    public DateTime? PeriodEnd { get; set; }
    public string? Notes { get; set; }
}
