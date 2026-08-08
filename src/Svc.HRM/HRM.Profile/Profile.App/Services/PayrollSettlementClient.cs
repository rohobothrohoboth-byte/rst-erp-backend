using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Profile.App.Interfaces;

namespace Profile.App.Services;

public class PayrollSettlementClient(HttpClient http, ILogger<PayrollSettlementClient> logger) : IPayrollSettlementClient
{
    public async Task<PayrollSettlementResult> CreateFinalPayRunAsync(
        Guid employeeId,
        DateTime lastWorkingDate,
        string reason,
        CancellationToken ct = default)
    {
        try
        {
            var periodStart = new DateTime(lastWorkingDate.Year, lastWorkingDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var payload = new
            {
                Name = $"Final Pay {lastWorkingDate:yyyy-MM} ({employeeId.ToString("N")[..8]})",
                PayPeriodStart = periodStart,
                PayPeriodEnd = DateTime.SpecifyKind(lastWorkingDate.Date, DateTimeKind.Utc),
                PaymentDate = DateTime.SpecifyKind(lastWorkingDate.Date, DateTimeKind.Utc),
                EmployeeIds = new List<Guid> { employeeId },
                Notes = $"Termination final pay settlement. {reason}".Trim()
            };

            using var response = await http.PostAsJsonAsync("api/v1/payroll-runs", payload, ct);
            var body = await response.Content.ReadAsStringAsync(ct);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Final pay run failed ({Status}): {Body}", response.StatusCode, body);
                return new PayrollSettlementResult
                {
                    Success = false,
                    Message = $"Payroll final-pay request failed ({(int)response.StatusCode})."
                };
            }

            using var doc = JsonDocument.Parse(body);
            Guid? runId = null;
            if (doc.RootElement.TryGetProperty("id", out var idEl) && idEl.TryGetGuid(out var g))
                runId = g;
            else if (doc.RootElement.TryGetProperty("data", out var data) &&
                     data.ValueKind == JsonValueKind.Object &&
                     data.TryGetProperty("id", out var nested) &&
                     nested.TryGetGuid(out var g2))
                runId = g2;

            return new PayrollSettlementResult
            {
                Success = true,
                PayrollRunId = runId,
                Message = "Final pay payroll run created."
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Final pay run request error for employee {EmployeeId}", employeeId);
            return new PayrollSettlementResult { Success = false, Message = ex.Message };
        }
    }
}
