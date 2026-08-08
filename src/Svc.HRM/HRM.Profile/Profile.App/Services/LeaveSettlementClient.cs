using System.Text.Json;
using Microsoft.Extensions.Logging;
using Profile.App.Interfaces;

namespace Profile.App.Services;

public class LeaveSettlementClient(HttpClient http, ILogger<LeaveSettlementClient> logger) : ILeaveSettlementClient
{
    public async Task<LeaveSettlementSnapshot> GetUnpaidDaysAsync(
        Guid employeeId,
        DateTime from,
        DateTime to,
        CancellationToken ct = default)
    {
        try
        {
            var url =
                $"api/hrm/leave/v1/Integration/UnpaidDays/{employeeId}?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}";
            using var response = await http.GetAsync(url, ct);
            var body = await response.Content.ReadAsStringAsync(ct);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Leave unpaid-days failed ({Status}): {Body}", response.StatusCode, body);
                return new LeaveSettlementSnapshot
                {
                    Success = false,
                    Message = $"Leave settlement lookup failed ({(int)response.StatusCode})."
                };
            }

            using var doc = JsonDocument.Parse(string.IsNullOrWhiteSpace(body) ? "{}" : body);
            var root = doc.RootElement;
            decimal unpaid = 0;
            static bool TryReadDays(JsonElement el, out decimal days)
            {
                days = 0;
                if (el.ValueKind == JsonValueKind.Number) { days = el.GetDecimal(); return true; }
                foreach (var name in new[] { "unpaidDays", "UnpaidDays", "days", "Days" })
                {
                    if (el.TryGetProperty(name, out var p) && p.ValueKind == JsonValueKind.Number)
                    {
                        days = p.GetDecimal();
                        return true;
                    }
                }
                return false;
            }

            if (!TryReadDays(root, out unpaid) && root.TryGetProperty("data", out var data))
                TryReadDays(data, out unpaid);

            return new LeaveSettlementSnapshot
            {
                Success = true,
                UnpaidDays = unpaid,
                Message = "Leave unpaid days snapshot retrieved."
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Leave settlement lookup error for employee {EmployeeId}", employeeId);
            return new LeaveSettlementSnapshot { Success = false, Message = ex.Message };
        }
    }
}
