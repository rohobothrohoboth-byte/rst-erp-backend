using Microsoft.Extensions.Diagnostics.HealthChecks;
using Svc.HRM.Payroll.Persistence;

namespace Svc.HRM.Payroll.HealthChecks;

public class PayrollHealthCheck : IHealthCheck
{
    private readonly PayrollDbContext _context;
    private readonly ILogger<PayrollHealthCheck> _logger;

    public PayrollHealthCheck(PayrollDbContext context, ILogger<PayrollHealthCheck> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync(ct);
            if (canConnect)
            {
                return HealthCheckResult.Healthy("Payroll database is healthy");
            }
            return HealthCheckResult.Unhealthy("Cannot connect to payroll database");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Payroll health check failed");
            return HealthCheckResult.Unhealthy("Payroll health check failed", ex);
        }
    }
}