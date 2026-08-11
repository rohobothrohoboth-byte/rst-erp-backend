namespace Svc.HRM.Performance.Models.Entities;

public class Kpi : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Metric { get; set; }
    public decimal? Target { get; set; }
    public decimal Weight { get; set; }
    public string? Category { get; set; }
}
