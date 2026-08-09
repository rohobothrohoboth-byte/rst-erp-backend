using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using Svc.HRM.Reports.Services;

namespace Svc.HRM.Reports.Controllers;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/reports")]
[RequestTimeout(10000)] // hard stop at 10s so Gateway/UI never wait 30s+
public class HrReportsController(IHrReportService reports) : ControllerBase
{
    [HttpGet("summary")]
    [RequestTimeout(12000)]
    public async Task<IActionResult> Summary(CancellationToken ct) =>
        Ok(await reports.GetSummaryAsync(ct));

    [HttpGet("employees")]
    public async Task<IActionResult> Employees(CancellationToken ct) =>
        Ok(await reports.GetEmployeeReportAsync(ct));

    [HttpGet("attendance")]
    public async Task<IActionResult> Attendance(
        [FromQuery] int? year,
        [FromQuery] int? month,
        [FromQuery] DateTime? date,
        CancellationToken ct) =>
        Ok(await reports.GetAttendanceReportAsync(year, month, date, ct));

    [HttpGet("leave")]
    public async Task<IActionResult> Leave(CancellationToken ct) =>
        Ok(await reports.GetLeaveReportAsync(ct));

    [HttpGet("payroll")]
    public async Task<IActionResult> Payroll(CancellationToken ct) =>
        Ok(await reports.GetPayrollReportAsync(ct));

    [HttpGet("recruitment")]
    public async Task<IActionResult> Recruitment(CancellationToken ct) =>
        Ok(await reports.GetRecruitmentReportAsync(ct));
}
