using System.Text.Json;

namespace Svc.HRM.Payroll.Services;

public interface ILeavePayrollClient
{
    Task<decimal> GetUnpaidLeaveDaysAsync(Guid employeeId, DateTime from, DateTime to, CancellationToken ct = default);
}

public class LeavePayrollClient : ILeavePayrollClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<LeavePayrollClient> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public LeavePayrollClient(HttpClient httpClient, ILogger<LeavePayrollClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<decimal> GetUnpaidLeaveDaysAsync(
        Guid employeeId, DateTime from, DateTime to, CancellationToken ct = default)
    {
        try
        {
            var url =
                $"/api/hrm/leave/v1/Integration/UnpaidDays/{employeeId}?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}";
            var response = await _httpClient.GetAsync(url, ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Leave UnpaidDays returned {Status} for {EmployeeId}", response.StatusCode, employeeId);
                return 0;
            }

            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            var data = root.TryGetProperty("data", out var d) || root.TryGetProperty("Data", out d)
                ? d
                : root;

            if (data.TryGetProperty("unpaidDays", out var days) || data.TryGetProperty("UnpaidDays", out days))
                return days.GetDecimal();

            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching unpaid leave days for employee {Id}", employeeId);
            return 0;
        }
    }
}
