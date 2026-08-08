using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Cor.PlanDev.HealthChecks;

public class PlanDevHealthCheck : IHealthCheck
{
    private readonly ILogger<PlanDevHealthCheck> _logger;

    public PlanDevHealthCheck(ILogger<PlanDevHealthCheck> logger)
    {
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Add your health checks here
            // e.g., database connection, external service availability

            return await Task.FromResult(HealthCheckResult.Healthy("Plan & Development service is healthy"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return await Task.FromResult(HealthCheckResult.Unhealthy("Health check failed", ex));
        }
    }
}