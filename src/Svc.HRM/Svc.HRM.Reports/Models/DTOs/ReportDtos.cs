namespace Svc.HRM.Reports.Models.DTOs;

public class HrReportEnvelope
{
    public string Domain { get; set; } = default!;
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public bool UpstreamSuccess { get; set; }
    public string? Message { get; set; }
    public object? Data { get; set; }
}

public class HrReportsSummaryDto
{
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public HrReportEnvelope Employees { get; set; } = default!;
    public HrReportEnvelope Attendance { get; set; } = default!;
    public HrReportEnvelope Leave { get; set; } = default!;
    public HrReportEnvelope Payroll { get; set; } = default!;
    public HrReportEnvelope Recruitment { get; set; } = default!;
}
