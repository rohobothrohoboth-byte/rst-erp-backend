using System.Text.Json;
using Svc.HRM.Reports.Models.DTOs;

namespace Svc.HRM.Reports.Services;

public class HrReportService(
    IHttpClientFactory httpClientFactory,
    ILogger<HrReportService> logger) : IHrReportService
{
    public Task<HrReportEnvelope> GetEmployeeReportAsync(CancellationToken ct = default) =>
        FetchAsync("profile", "employees", "api/hrm/profile/v1/Employee/stats", ct);

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
        try
        {
            var client = httpClientFactory.CreateClient("recruit");
            var offersTask = client.GetAsync("api/hrm/recruit/v1/JobOffer/All", ct);
            var reqsTask = client.GetAsync("api/hrm/recruit/v1/JobReq/AllJobReq", ct);
            var interviewsTask = client.GetAsync("api/hrm/recruit/v1/Interview/All", ct);
            await Task.WhenAll(offersTask, reqsTask, interviewsTask);

            async Task<object?> Read(HttpResponseMessage res)
            {
                var body = await res.Content.ReadAsStringAsync(ct);
                if (string.IsNullOrWhiteSpace(body)) return null;
                try { return JsonSerializer.Deserialize<JsonElement>(body); }
                catch { return body; }
            }

            var offers = await offersTask;
            var reqs = await reqsTask;
            var interviews = await interviewsTask;
            var ok = offers.IsSuccessStatusCode || reqs.IsSuccessStatusCode || interviews.IsSuccessStatusCode;

            return new HrReportEnvelope
            {
                Domain = "recruitment",
                UpstreamSuccess = ok,
                Message = ok ? "OK" : "Recruitment upstream endpoints unavailable",
                Data = new
                {
                    offers = await Read(offers),
                    requisitions = await Read(reqs),
                    interviews = await Read(interviews)
                }
            };
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "HR recruitment report failed");
            return new HrReportEnvelope { Domain = "recruitment", UpstreamSuccess = false, Message = ex.Message };
        }
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

    private async Task<HrReportEnvelope> FetchAsync(string clientName, string domain, string path, CancellationToken ct)
    {
        try
        {
            var client = httpClientFactory.CreateClient(clientName);
            using var response = await client.GetAsync(path, ct);
            var body = await response.Content.ReadAsStringAsync(ct);
            object? data = null;
            if (!string.IsNullOrWhiteSpace(body))
            {
                try { data = JsonSerializer.Deserialize<JsonElement>(body); }
                catch { data = body; }
            }

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
        catch (Exception ex)
        {
            logger.LogWarning(ex, "HR report fetch failed for {Domain} via {Path}", domain, path);
            return new HrReportEnvelope
            {
                Domain = domain,
                UpstreamSuccess = false,
                Message = ex.Message
            };
        }
    }

    private static string Truncate(string? value) =>
        string.IsNullOrEmpty(value) ? string.Empty : value.Length <= 300 ? value : value[..300];
}
