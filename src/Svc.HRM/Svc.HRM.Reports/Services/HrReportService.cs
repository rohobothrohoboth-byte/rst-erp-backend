using System.Diagnostics;
using System.Text.Json;
using Svc.HRM.Reports.Models.DTOs;

namespace Svc.HRM.Reports.Services;

/// <summary>
/// Aggregates HR reports by calling upstream APIs through the Gateway (same paths as the UI/Postman).
/// Each upstream call is hard-capped so the Reports API always returns quickly.
/// </summary>
public class HrReportService(
    IHttpClientFactory httpClientFactory,
    ILogger<HrReportService> logger) : IHrReportService
{
    private const string GatewayClient = "gateway";
    private static readonly TimeSpan UpstreamTimeout = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan DomainBudget = TimeSpan.FromSeconds(7);

    public async Task<HrReportEnvelope> GetEmployeeReportAsync(CancellationToken ct = default)
    {
        // Only proven/fast Profile endpoints (Postman-verified EmpExp). Skip heavy Employee/stats.
        return await WithBudget(async token =>
        {
            var experience = FetchAsync("employees", "hrm/profile/v1/EmpExp/AllEmpExp", token);
            var education = FetchAsync("employees", "hrm/profile/v1/EmpEdu/AllEmpEdu", token);
            var repo = FetchAsync("employees", "hrm/profile/v1/EmpListRepo/EmpDbRepo", token);
            await Task.WhenAll(experience, education, repo);

            var parts = new[] { await experience, await education, await repo };
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
                    experience = (await experience).Data,
                    education = (await education).Data,
                    repository = (await repo).Data,
                    upstream = parts.Select(p => new { p.Message, p.UpstreamSuccess })
                }
            };
        }, "employees", ct);
    }

    public Task<HrReportEnvelope> GetAttendanceReportAsync(int? year, int? month, DateTime? date, CancellationToken ct = default)
    {
        if (date.HasValue)
        {
            return WithBudget(
                token => FetchAsync("attendance", $"attendance/report/daily?date={date:yyyy-MM-dd}", token),
                "attendance", ct);
        }

        var y = year ?? DateTime.UtcNow.Year;
        var m = month ?? DateTime.UtcNow.Month;
        return WithBudget(
            token => FetchAsync("attendance", $"attendance/report/monthly?year={y}&month={m}", token),
            "attendance", ct);
    }

    public Task<HrReportEnvelope> GetLeaveReportAsync(CancellationToken ct = default) =>
        WithBudget(
            token => FetchAsync("leave", "hrm/leave/v1/dashboard/statistics", token),
            "leave", ct);

    public Task<HrReportEnvelope> GetPayrollReportAsync(CancellationToken ct = default) =>
        WithBudget(
            token => FetchAsync("payroll", "payroll/reports/payroll-summary", token),
            "payroll", ct);

    public async Task<HrReportEnvelope> GetRecruitmentReportAsync(CancellationToken ct = default)
    {
        return await WithBudget(async token =>
        {
            var offers = FetchAsync("recruitment", "hrm/recruit/v1/JobOffer/All", token);
            var reqs = FetchAsync("recruitment", "hrm/recruit/v1/JobReq/AllJobReq", token);
            var interviews = FetchAsync("recruitment", "hrm/recruit/v1/Interview/All", token);
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
                Data = new { offers = o.Data, requisitions = r.Data, interviews = i.Data }
            };
        }, "recruitment", ct);
    }

    public async Task<HrReportsSummaryDto> GetSummaryAsync(CancellationToken ct = default)
    {
        var employees = GetEmployeeReportAsync(ct);
        var attendance = GetAttendanceReportAsync(null, null, null, ct);
        var leave = GetLeaveReportAsync(ct);
        var payroll = GetPayrollReportAsync(ct);
        var recruit = GetRecruitmentReportAsync(ct);
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

    private async Task<HrReportEnvelope> WithBudget(
        Func<CancellationToken, Task<HrReportEnvelope>> action,
        string domain,
        CancellationToken ct)
    {
        using var budgetCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        budgetCts.CancelAfter(DomainBudget);
        try
        {
            return await action(budgetCts.Token);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            logger.LogWarning("HR report domain {Domain} exceeded {Budget}s budget", domain, DomainBudget.TotalSeconds);
            return new HrReportEnvelope
            {
                Domain = domain,
                UpstreamSuccess = false,
                Message = $"Report timed out after {DomainBudget.TotalSeconds:0}s"
            };
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "HR report domain {Domain} failed", domain);
            return new HrReportEnvelope { Domain = domain, UpstreamSuccess = false, Message = ex.Message };
        }
    }

    private async Task<HrReportEnvelope> FetchAsync(string domain, string gatewayPath, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var client = httpClientFactory.CreateClient(GatewayClient);
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            timeoutCts.CancelAfter(UpstreamTimeout);

            using var response = await client.GetAsync(gatewayPath, HttpCompletionOption.ResponseHeadersRead, timeoutCts.Token);
            await using var stream = await response.Content.ReadAsStreamAsync(timeoutCts.Token);
            using var reader = new StreamReader(stream);
            var body = await reader.ReadToEndAsync(timeoutCts.Token);

            object? data = null;
            if (!string.IsNullOrWhiteSpace(body))
            {
                try { data = JsonSerializer.Deserialize<JsonElement>(body); }
                catch { data = body; }
            }

            var baseUrl = client.BaseAddress?.ToString() ?? "";
            logger.LogInformation(
                "HR report {Domain} GET {Base}{Path} => {Status} in {Elapsed}ms",
                domain, baseUrl, gatewayPath, (int)response.StatusCode, sw.ElapsedMilliseconds);

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
            logger.LogWarning("HR report {Domain} timed out after {Timeout}s via {Path}", domain, UpstreamTimeout.TotalSeconds, gatewayPath);
            return new HrReportEnvelope
            {
                Domain = domain,
                UpstreamSuccess = false,
                Message = $"Upstream timeout after {UpstreamTimeout.TotalSeconds:0}s ({gatewayPath})"
            };
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "HR report fetch failed for {Domain} via {Path}", domain, gatewayPath);
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
