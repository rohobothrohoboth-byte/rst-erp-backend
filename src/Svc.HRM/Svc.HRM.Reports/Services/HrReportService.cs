using System.Diagnostics;
using System.Text.Json;
using Svc.HRM.Reports.Models.DTOs;

namespace Svc.HRM.Reports.Services;

/// <summary>
/// Aggregates HR reports via the HTTP Gateway (same paths as UI/Postman).
/// Uses Task.WhenAny hard budgets so the API always returns even if HttpClient ignore cancel.
/// </summary>
public class HrReportService(
    IHttpClientFactory httpClientFactory,
    ILogger<HrReportService> logger) : IHrReportService
{
    private const string GatewayClient = "gateway";
    private static readonly TimeSpan UpstreamTimeout = TimeSpan.FromSeconds(4);
    private static readonly TimeSpan DomainBudget = TimeSpan.FromSeconds(6);

    public Task<HrReportEnvelope> GetEmployeeReportAsync(CancellationToken ct = default) =>
        WithBudget(async token =>
        {
            var experience = FetchAsync("employees", "hrm/profile/v1/EmpExp/AllEmpExp", token);
            var education = FetchAsync("employees", "hrm/profile/v1/EmpEdu/AllEmpEdu", token);
            await Task.WhenAll(experience, education);

            var exp = await experience;
            var edu = await education;
            var ok = exp.UpstreamSuccess || edu.UpstreamSuccess;
            return new HrReportEnvelope
            {
                Domain = "employees",
                UpstreamSuccess = ok,
                Message = ok ? "OK" : string.Join(" | ", new[] { exp.Message, edu.Message }.Where(m => !string.IsNullOrWhiteSpace(m))),
                Data = new { experience = exp.Data, education = edu.Data }
            };
        }, "employees", ct);

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

    public Task<HrReportEnvelope> GetRecruitmentReportAsync(CancellationToken ct = default) =>
        // Single lightweight call first — fan-out of 3× All* endpoints was too slow.
        WithBudget(
            token => FetchAsync("recruitment", "hrm/recruit/v1/JobReq/AllJobReq", token),
            "recruitment", ct);

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

    /// <summary>
    /// Absolute wall-clock budget. Does not await abandoned HTTP calls after timeout.
    /// </summary>
    private async Task<HrReportEnvelope> WithBudget(
        Func<CancellationToken, Task<HrReportEnvelope>> action,
        string domain,
        CancellationToken ct)
    {
        using var budgetCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        var work = action(budgetCts.Token);
        var winner = await Task.WhenAny(work, Task.Delay(DomainBudget, CancellationToken.None));

        if (winner != work)
        {
            budgetCts.Cancel();
            logger.LogWarning("HR report domain {Domain} hard-stopped after {Budget}s", domain, DomainBudget.TotalSeconds);
            // Do not await `work` — it may hang; abandon it.
            return new HrReportEnvelope
            {
                Domain = domain,
                UpstreamSuccess = false,
                Message = $"Report timed out after {DomainBudget.TotalSeconds:0}s (gateway upstream slow/down)"
            };
        }

        try
        {
            return await work;
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
            var body = await response.Content.ReadAsStringAsync(timeoutCts.Token);

            object? data = null;
            if (!string.IsNullOrWhiteSpace(body))
            {
                try { data = JsonSerializer.Deserialize<JsonElement>(body); }
                catch { data = body; }
            }

            logger.LogInformation(
                "HR report {Domain} GET {Base}{Path} => {Status} in {Elapsed}ms",
                domain, client.BaseAddress, gatewayPath, (int)response.StatusCode, sw.ElapsedMilliseconds);

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
        catch (OperationCanceledException)
        {
            logger.LogWarning("HR report {Domain} canceled/timed out via {Path} after {Elapsed}ms", domain, gatewayPath, sw.ElapsedMilliseconds);
            return new HrReportEnvelope
            {
                Domain = domain,
                UpstreamSuccess = false,
                Message = $"Upstream timeout ({gatewayPath})"
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
