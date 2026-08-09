using System.Text.Json;
using System.Text.Json.Serialization;

namespace Svc.HRM.Payroll.Services;

public interface IAttendanceClient
{
    Task<EmployeeAttendanceDto> GetEmployeeAttendanceAsync(Guid employeeId, DateTime startDate, DateTime endDate, CancellationToken ct = default);
}

public class AttendanceClient : IAttendanceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AttendanceClient> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public AttendanceClient(HttpClient httpClient, ILogger<AttendanceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<EmployeeAttendanceDto> GetEmployeeAttendanceAsync(
        Guid employeeId, DateTime startDate, DateTime endDate, CancellationToken ct = default)
    {
        var empty = new EmployeeAttendanceDto
        {
            EmployeeId = employeeId,
            StartDate = startDate,
            EndDate = endDate
        };

        try
        {
            // Prefer summary endpoint (matches AttendanceSummaryDto shape)
            var summaryUrl =
                $"/api/v1/attendance/employee/{employeeId}/summary?from={startDate:yyyy-MM-dd}&to={endDate:yyyy-MM-dd}";
            var summaryResponse = await _httpClient.GetAsync(summaryUrl, ct);
            if (summaryResponse.IsSuccessStatusCode)
            {
                var summaryJson = await summaryResponse.Content.ReadAsStringAsync(ct);
                var summary = DeserializeData<AttendanceSummaryResponse>(summaryJson)
                              ?? JsonSerializer.Deserialize<AttendanceSummaryResponse>(summaryJson, JsonOptions);

                if (summary != null)
                {
                    return new EmployeeAttendanceDto
                    {
                        EmployeeId = employeeId,
                        StartDate = startDate,
                        EndDate = endDate,
                        TotalDays = summary.TotalDays,
                        PresentDays = summary.PresentDays,
                        AbsentDays = summary.AbsentDays,
                        LeaveDays = summary.LeaveDays,
                        HolidayDays = summary.HolidayDays,
                        OvertimeHours = summary.TotalOvertimeHours
                    };
                }
            }

            // Fallback: period records → aggregate
            var periodUrl =
                $"/api/v1/attendance/employee/{employeeId}/period?start={startDate:yyyy-MM-dd}&end={endDate:yyyy-MM-dd}";
            var periodResponse = await _httpClient.GetAsync(periodUrl, ct);
            if (!periodResponse.IsSuccessStatusCode)
                return empty;

            var periodJson = await periodResponse.Content.ReadAsStringAsync(ct);
            var records = DeserializeData<List<PeriodRecord>>(periodJson)
                          ?? JsonSerializer.Deserialize<List<PeriodRecord>>(periodJson, JsonOptions)
                          ?? [];

            return new EmployeeAttendanceDto
            {
                EmployeeId = employeeId,
                StartDate = startDate,
                EndDate = endDate,
                TotalDays = records.Count,
                PresentDays = records.Count(r => IsStatus(r.Status, "Present", "Late")),
                AbsentDays = records.Count(r => IsStatus(r.Status, "Absent")),
                LeaveDays = records.Count(r => IsStatus(r.Status, "Leave")),
                HolidayDays = records.Count(r => IsStatus(r.Status, "Holiday")),
                OvertimeHours = records.Sum(r => r.OvertimeHours),
                DailyRecords = records.Select(r => new DailyAttendanceDto
                {
                    Date = r.Date,
                    CheckIn = r.CheckIn,
                    CheckOut = r.CheckOut,
                    Status = r.Status,
                    HoursWorked = r.HoursWorked,
                    OvertimeHours = r.OvertimeHours
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching attendance for employee {Id}", employeeId);
            return empty;
        }
    }

    private static bool IsStatus(string? status, params string[] names) =>
        names.Any(n => string.Equals(status, n, StringComparison.OrdinalIgnoreCase));

    private static T? DeserializeData<T>(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        if (root.TryGetProperty("data", out var data) || root.TryGetProperty("Data", out data))
            return JsonSerializer.Deserialize<T>(data.GetRawText(), JsonOptions);
        return default;
    }

    private class AttendanceSummaryResponse
    {
        public int TotalDays { get; set; }
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
        public int LeaveDays { get; set; }
        public int HolidayDays { get; set; }
        public double TotalOvertimeHours { get; set; }
    }

    private class PeriodRecord
    {
        public DateTime Date { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public string Status { get; set; } = default!;
        public double HoursWorked { get; set; }
        public double OvertimeHours { get; set; }
    }
}
