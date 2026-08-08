// Cor.Finance/HealthChecks/RedisHealthCheck.cs
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace Cor.Finance.HealthChecks;

public class RedisHealthCheck : IHealthCheck
{
    private readonly IConnectionMultiplexer _connection;

    public RedisHealthCheck(IConnectionMultiplexer connection)
    {
        _connection = connection;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (_connection.IsConnected)
            {
                var db = _connection.GetDatabase();
                var ping = db.Ping();
                return Task.FromResult(HealthCheckResult.Healthy(
                    $"Redis is healthy. PING: {ping.TotalMilliseconds}ms"));
            }

            return Task.FromResult(HealthCheckResult.Unhealthy(
                "Redis is not connected."));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy(
                $"Redis health check failed: {ex.Message}"));
        }
    }
}