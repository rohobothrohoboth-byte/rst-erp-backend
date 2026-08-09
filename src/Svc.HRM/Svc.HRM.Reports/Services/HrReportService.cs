using System.Diagnostics;
using System.Text.Json;
using Svc.HRM.Reports.Models.DTOs;

namespace Svc.HRM.Reports.Services;

/// <summary>
/// Calls upstreams through HTTP Gateway (same paths Postman uses successfully).
/// Hard-caps each domain so the API always returns quickly.
/// </summary>
public class HrReportService(
    IHttpClientFactory httpClientFactory,
    ILogger<HrReportService> logger) : IHrReportService
{
    private const string ClientName = "gateway";
    private static readonly TimeSpan UpstreamTimeout = TimeSpan.FromSeconds(4);
    private static readonly TimeSpan DomainBudget = TimeSpan.FromSeconds(6);

    public Task<HrReportEnvelope> GetEmployeeReportAsync(CancellationToken ct = default) =>
        // Single proven Postman path: /hrm/profile/v1/Employee/stats
        WithBudget(
            token => FetchAsync("employees", "hrm/profile/v1/Employee/stats", token),
            "employees", ct);

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
            logger.LogWarning("HR report domain {Domain} hard-stopped after {Budget}s [{Build}]",
                domain, DomainBudget.TotalSeconds, ReportsBuild.Id);
            return Envelope(domain, false, $"Report timed out after {DomainBudget.TotalSeconds:0}s [{ReportsBuild.Id}]");
        }

        try { return await work; }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "HR report domain {Domain} failed [{Build}]", domain, ReportsBuild.Id);
            return Envelope(domain, false, $"{ex.Message} [{ReportsBuild.Id}]");
        }
    }

    private async Task<HrReportEnvelope> FetchAsync(string domain, string gatewayPath, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var client = httpClientFactory.CreateClient(ClientName);
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
                "[{Build}] HR report {Domain} GET {Base}{Path} => {Status} in {Elapsed}ms",
                ReportsBuild.Id, domain, client.BaseAddress, gatewayPath, (int)response.StatusCode, sw.ElapsedMilliseconds);

            return Envelope(domain, response.IsSuccessStatusCode,
                response.IsSuccessStatusCode ? "OK" : $"Upstream {(int)response.StatusCode}: {Truncate(body)}",
                data);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("[{Build}] HR report {Domain} canceled via {Path} after {Elapsed}ms",
                ReportsBuild.Id, domain, gatewayPath, sw.ElapsedMilliseconds);
            return Envelope(domain, false, $"Upstream timeout ({gatewayPath})");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "[{Build}] HR report fetch failed for {Domain} via {Path}",
                ReportsBuild.Id, domain, gatewayPath);
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
