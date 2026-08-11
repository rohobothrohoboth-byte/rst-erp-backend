namespace Svc.HRM.Performance.Models.DTOs;

public class KpiCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Metric { get; set; }
    public decimal? Target { get; set; }
    public decimal Weight { get; set; }
    public string? Category { get; set; }
}
