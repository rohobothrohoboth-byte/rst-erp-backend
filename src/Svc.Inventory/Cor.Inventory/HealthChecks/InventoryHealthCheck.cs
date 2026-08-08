using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Cor.Inventory.HealthChecks;

public class InventoryHealthCheck : IHealthCheck
{
    private readonly ILogger<InventoryHealthCheck> _logger;

    public InventoryHealthCheck(ILogger<InventoryHealthCheck> logger)
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
            // Check database connectivity, etc.

            return await Task.FromResult(HealthCheckResult.Healthy("Procurement service is healthy"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return await Task.FromResult(
                HealthCheckResult.Unhealthy($"Procurement service is unhealthy: {ex.Message}"));
        }
    }
}