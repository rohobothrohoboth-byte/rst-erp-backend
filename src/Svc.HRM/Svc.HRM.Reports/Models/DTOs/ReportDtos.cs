namespace Svc.HRM.Reports.Models.DTOs;

public static class ReportsBuild
{
    /// <summary>Bump whenever shipping a Reports behavior change so Postman can prove the binary.</summary>
    public const string Id = "reports-20260809-gateway-v5";
}

public class HrReportEnvelope
{
    public string Build { get; set; } = ReportsBuild.Id;
    public string Domain { get; set; } = default!;
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public bool UpstreamSuccess { get; set; }
    public string? Message { get; set; }
    public object? Data { get; set; }
}

public class HrReportsSummaryDto
{
    public string Build { get; set; } = ReportsBuild.Id;
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public HrReportEnvelope Employees { get; set; } = default!;
    public HrReportEnvelope Attendance { get; set; } = default!;
    public HrReportEnvelope Leave { get; set; } = default!;
    public HrReportEnvelope Payroll { get; set; } = default!;
    public HrReportEnvelope Recruitment { get; set; } = default!;
}
