using System.Diagnostics;
using System.Text.Json;
using Svc.HRM.Reports.Models.DTOs;

namespace Svc.HRM.Reports.Services;

/// <summary>
/// Calls HR microservices directly (NOT back through Gateway) to avoid
/// Gateway ↔ Reports re-entrancy deadlocks / long hangs.
/// </summary>
public class HrReportService(
    IHttpClientFactory httpClientFactory,
    ILogger<HrReportService> logger) : IHrReportService
{
    private static readonly TimeSpan UpstreamTimeout = TimeSpan.FromSeconds(4);
    private static readonly TimeSpan DomainBudget = TimeSpan.FromSeconds(6);

    public Task<HrReportEnvelope> GetEmployeeReportAsync(CancellationToken ct = default) =>
        WithBudget(async token =>
        {
            var experience = FetchAsync("profile", "employees", "api/hrm/profile/v1/EmpExp/AllEmpExp", token);
            var education = FetchAsync("profile", "employees", "api/hrm/profile/v1/EmpEdu/AllEmpEdu", token);
            await Task.WhenAll(experience, education);
            var exp = await experience;
            var edu = await education;
            var ok = exp.UpstreamSuccess || edu.UpstreamSuccess;
            return Envelope("employees", ok,
                ok ? "OK" : JoinMessages(exp.Message, edu.Message),
                new { experience = exp.Data, education = edu.Data });
        }, "employees", ct);

    public Task<HrReportEnvelope> GetAttendanceReportAsync(int? year, int? month, DateTime? date, CancellationToken ct = default)
    {
        if (date.HasValue)
        {
            return WithBudget(
                token => FetchAsync("attendance", "attendance", $"api/v1/attendance/report/daily?date={date:yyyy-MM-dd}", token),
                "attendance", ct);
        }

        var y = year ?? DateTime.UtcNow.Year;
        var m = month ?? DateTime.UtcNow.Month;
        return WithBudget(
            token => FetchAsync("attendance", "attendance", $"api/v1/attendance/report/monthly?year={y}&month={m}", token),
            "attendance", ct);
    }

    public Task<HrReportEnvelope> GetLeaveReportAsync(CancellationToken ct = default) =>
        WithBudget(
            token => FetchAsync("leave", "leave", "api/hrm/leave/v1/dashboard/statistics", token),
            "leave", ct);

    public Task<HrReportEnvelope> GetPayrollReportAsync(CancellationToken ct = default) =>
        WithBudget(
            token => FetchAsync("payroll", "payroll", "api/v1/reports/payroll-summary", token),
            "payroll", ct);

    public Task<HrReportEnvelope> GetRecruitmentReportAsync(CancellationToken ct = default) =>
        WithBudget(
            token => FetchAsync("recruit", "recruitment", "api/hrm/recruit/v1/JobReq/AllJobReq", token),
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
            return Envelope(domain, false, $"Report timed out after {DomainBudget.TotalSeconds:0}s (upstream slow/down)");
        }

        try { return await work; }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "HR report domain {Domain} failed", domain);
            return Envelope(domain, false, ex.Message);
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

            using var response = await client.GetAsync(path, HttpCompletionOption.ResponseHeadersRead, timeoutCts.Token);
            var body = await response.Content.ReadAsStringAsync(timeoutCts.Token);

            object? data = null;
            if (!string.IsNullOrWhiteSpace(body))
            {
                try { data = JsonSerializer.Deserialize<JsonElement>(body); }
                catch { data = body; }
            }

            logger.LogInformation(
                "HR report {Domain} GET {Base}{Path} => {Status} in {Elapsed}ms",
                domain, client.BaseAddress, path, (int)response.StatusCode, sw.ElapsedMilliseconds);

            return Envelope(domain, response.IsSuccessStatusCode,
                response.IsSuccessStatusCode ? "OK" : $"Upstream {(int)response.StatusCode}: {Truncate(body)}",
                data);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("HR report {Domain} canceled via {Path} after {Elapsed}ms", domain, path, sw.ElapsedMilliseconds);
            return Envelope(domain, false, $"Upstream timeout ({path})");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "HR report fetch failed for {Domain} via {Path}", domain, path);
            return Envelope(domain, false, $"{ex.GetType().Name}: {ex.Message}");
        }
    }

    private static HrReportEnvelope Envelope(string domain, bool ok, string message, object? data = null) => new()
    {
        Domain = domain,
        UpstreamSuccess = ok,
        Message = message,
        Data = data
    };

    private static string JoinMessages(params string?[] messages) =>
        string.Join(" | ", messages.Where(m => !string.IsNullOrWhiteSpace(m)).Distinct());

    private static string Truncate(string? value) =>
        string.IsNullOrEmpty(value) ? string.Empty : value.Length <= 300 ? value : value[..300];
}
