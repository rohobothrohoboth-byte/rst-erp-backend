using System.Text.Json;
using System.Text.Json.Serialization;

namespace Svc.HRM.Attendance.Services;

public class ApprovedLeaveInfo
{
    public Guid RequestId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid LeaveTypeId { get; set; }
    public string LeaveTypeName { get; set; } = default!;
    public string LeaveCategory { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double DaysRequested { get; set; }
    public bool IsHalfDay { get; set; }
    public string Status { get; set; } = default!;
}

public interface IHrmLeaveClient
{
    Task<List<ApprovedLeaveInfo>> GetApprovedLeavesAsync(DateTime from, DateTime to, Guid? employeeId = null, CancellationToken ct = default);
    Task<bool> IsOnApprovedLeaveAsync(Guid employeeId, DateTime date, CancellationToken ct = default);
}

public class HrmLeaveClient : IHrmLeaveClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HrmLeaveClient> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public HrmLeaveClient(HttpClient httpClient, ILogger<HrmLeaveClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<ApprovedLeaveInfo>> GetApprovedLeavesAsync(
        DateTime from, DateTime to, Guid? employeeId = null, CancellationToken ct = default)
    {
        try
        {
            var url = $"/api/hrm/leave/v1/Integration/ApprovedLeaves?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}";
            if (employeeId.HasValue && employeeId != Guid.Empty)
                url += $"&employeeId={employeeId}";

            var response = await _httpClient.GetAsync(url, ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("HRM.Leave ApprovedLeaves returned {Status}", response.StatusCode);
                return [];
            }

            var json = await response.Content.ReadAsStringAsync(ct);
            var result = JsonSerializer.Deserialize<ApiEnvelope<List<ApprovedLeaveInfo>>>(json, JsonOptions);
            return result?.Data ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to query HRM.Leave approved leaves");
            return [];
        }
    }

    public async Task<bool> IsOnApprovedLeaveAsync(Guid employeeId, DateTime date, CancellationToken ct = default)
    {
        var leaves = await GetApprovedLeavesAsync(date.Date, date.Date, employeeId, ct);
        return leaves.Any(x => x.StartDate.Date <= date.Date && x.EndDate.Date >= date.Date);
    }

    private class ApiEnvelope<T>
    {
        [JsonPropertyName("data")]
        public T? Data { get; set; }

        [JsonPropertyName("Data")]
        public T? DataPascal { set => Data ??= value; }
    }
}
