using System.Diagnostics;
using System.Text.Json;
using Svc.HRM.Reports.Models.DTOs;

namespace Svc.HRM.Reports.Services;

public class HrReportService(
    IHttpClientFactory httpClientFactory,
    ILogger<HrReportService> logger) : IHrReportService
{
    private static readonly TimeSpan UpstreamTimeout = TimeSpan.FromSeconds(12);

    public async Task<HrReportEnvelope> GetEmployeeReportAsync(CancellationToken ct = default)
    {
        // Prefer lightweight / known-good Profile endpoints. Employee/stats alone can be slow on cold cache.
        var stats = FetchAsync("profile", "employees", "api/hrm/profile/v1/Employee/stats", ct);
        var dashboard = FetchAsync("profile", "employees", "api/hrm/profile/v1/EmpListRepo/dashboard", ct);
        var experience = FetchAsync("profile", "employees", "api/hrm/profile/v1/EmpExp/AllEmpExp", ct);
        var education = FetchAsync("profile", "employees", "api/hrm/profile/v1/EmpEdu/AllEmpEdu", ct);
        await Task.WhenAll(stats, dashboard, experience, education);

        var parts = new[] { await stats, await dashboard, await experience, await education };
        var ok = parts.Any(p => p.UpstreamSuccess);
        return new HrReportEnvelope
        {
            Domain = "employees",
            UpstreamSuccess = ok,
            Message = ok
                ? "OK"
                : string.Join(" | ", parts.Select(p => p.Message).Where(m => !string.IsNullOrWhiteSpace(m)).Distinct()),
            Data = new
            {
                stats = (await stats).Data,
                dashboard = (await dashboard).Data,
                experience = (await experience).Data,
                education = (await education).Data,
                upstream = parts.Select(p => new { p.Message, p.UpstreamSuccess })
            }
        };
    }

    public Task<HrReportEnvelope> GetAttendanceReportAsync(int? year, int? month, DateTime? date, CancellationToken ct = default)
    {
        if (date.HasValue)
            return FetchAsync("attendance", "attendance", $"api/v1/attendance/report/daily?date={date:yyyy-MM-dd}", ct);

        var y = year ?? DateTime.UtcNow.Year;
        var m = month ?? DateTime.UtcNow.Month;
        return FetchAsync("attendance", "attendance", $"api/v1/attendance/report/monthly?year={y}&month={m}", ct);
    }

    public Task<HrReportEnvelope> GetLeaveReportAsync(CancellationToken ct = default) =>
        FetchAsync("leave", "leave", "api/hrm/leave/v1/dashboard/statistics", ct);

    public Task<HrReportEnvelope> GetPayrollReportAsync(CancellationToken ct = default) =>
        FetchAsync("payroll", "payroll", "api/v1/reports/payroll-summary", ct);

    public async Task<HrReportEnvelope> GetRecruitmentReportAsync(CancellationToken ct = default)
    {
        var offers = FetchAsync("recruit", "recruitment", "api/hrm/recruit/v1/JobOffer/All", ct);
        var reqs = FetchAsync("recruit", "recruitment", "api/hrm/recruit/v1/JobReq/AllJobReq", ct);
        var interviews = FetchAsync("recruit", "recruitment", "api/hrm/recruit/v1/Interview/All", ct);
        await Task.WhenAll(offers, reqs, interviews);

        var o = await offers;
        var r = await reqs;
        var i = await interviews;
        var ok = o.UpstreamSuccess || r.UpstreamSuccess || i.UpstreamSuccess;

        return new HrReportEnvelope
        {
            Domain = "recruitment",
            UpstreamSuccess = ok,
            Message = ok ? "OK" : "Recruitment upstream endpoints unavailable",
            Data = new
            {
                offers = o.Data,
                requisitions = r.Data,
                interviews = i.Data
            }
        };
    }

    public async Task<HrReportsSummaryDto> GetSummaryAsync(CancellationToken ct = default)
    {
        // Each domain isolates failures/timeouts so one dead service does not fail the hub.
        var employees = Safe(GetEmployeeReportAsync(ct), "employees");
        var attendance = Safe(GetAttendanceReportAsync(null, null, null, ct), "attendance");
        var leave = Safe(GetLeaveReportAsync(ct), "leave");
        var payroll = Safe(GetPayrollReportAsync(ct), "payroll");
        var recruit = Safe(GetRecruitmentReportAsync(ct), "recruitment");
        await Task.WhenAll(employees, attendance, leave, payroll, recruit);

        return new HrReportsSummaryDto
        {
            Employees = await employees,
            Attendance = await attendance,
            Leave = await leave,
            Payroll = await payroll,
            Recruitment = await recruit
        };
    }

    private async Task<HrReportEnvelope> Safe(Task<HrReportEnvelope> task, string domain)
    {
        try
        {
            return await task;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "HR report domain {Domain} failed", domain);
            return new HrReportEnvelope { Domain = domain, UpstreamSuccess = false, Message = ex.Message };
        }
    }

    private async Task<HrReportEnvelope> FetchAsync(string clientName, string domain, string path, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var client = httpClientFactory.CreateClient(clientName);
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            timeoutCts.CancelAfter(UpstreamTimeout);

            using var response = await client.GetAsync(path, timeoutCts.Token);
            var body = await response.Content.ReadAsStringAsync(ct);
            object? data = null;
            if (!string.IsNullOrWhiteSpace(body))
            {
                try { data = JsonSerializer.Deserialize<JsonElement>(body); }
                catch { data = body; }
            }

            logger.LogInformation(
                "HR report {Domain} GET {Path} => {Status} in {Elapsed}ms",
                domain, path, (int)response.StatusCode, sw.ElapsedMilliseconds);

            return new HrReportEnvelope
            {
                Domain = domain,
                UpstreamSuccess = response.IsSuccessStatusCode,
                Message = response.IsSuccessStatusCode
                    ? "OK"
                    : $"Upstream {(int)response.StatusCode}: {Truncate(body)}",
                Data = data
            };
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            logger.LogWarning("HR report {Domain} timed out after {Timeout}s via {Path}", domain, UpstreamTimeout.TotalSeconds, path);
            return new HrReportEnvelope
            {
                Domain = domain,
                UpstreamSuccess = false,
                Message = $"Upstream timeout after {UpstreamTimeout.TotalSeconds:0}s ({path})"
            };
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "HR report fetch failed for {Domain} via {Path}", domain, path);
            return new HrReportEnvelope
            {
                Domain = domain,
                UpstreamSuccess = false,
                Message = $"{ex.GetType().Name}: {ex.Message}"
            };
        }
    }

    private static string Truncate(string? value) =>
        string.IsNullOrEmpty(value) ? string.Empty : value.Length <= 300 ? value : value[..300];
}
