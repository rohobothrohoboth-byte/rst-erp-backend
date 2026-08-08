using Microsoft.Extensions.Diagnostics.HealthChecks;
using Svc.HRM.Attendance.Persistence;

namespace Svc.HRM.Attendance.HealthChecks;

public class AttendanceHealthCheck : IHealthCheck
{
    private readonly AttendanceDbContext _context;
    private readonly ILogger<AttendanceHealthCheck> _logger;

    public AttendanceHealthCheck(AttendanceDbContext context, ILogger<AttendanceHealthCheck> logger)
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
                return HealthCheckResult.Healthy("Attendance database is healthy");
            }
            return HealthCheckResult.Unhealthy("Cannot connect to attendance database");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Attendance health check failed");
            return HealthCheckResult.Unhealthy("Attendance health check failed", ex);
        }
    }
}