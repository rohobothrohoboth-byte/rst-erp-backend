// Svc.Task/HealthChecks/RedisHealthCheck.cs
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;  // ✅ ADD THIS

namespace Svc.Task.HealthChecks;

public class RedisHealthCheck : IHealthCheck
{
    private readonly IConnectionMultiplexer _connection;

    public RedisHealthCheck(IConnectionMultiplexer connection)
    {
        _connection = connection;
    }

    public System.Threading.Tasks.Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (_connection.IsConnected)
            {
                var db = _connection.GetDatabase();
                var ping = db.Ping();
                return System.Threading.Tasks.Task.FromResult(HealthCheckResult.Healthy(
                    $"Redis is healthy. PING: {ping.TotalMilliseconds}ms"));
            }

            return System.Threading.Tasks.Task.FromResult(HealthCheckResult.Unhealthy(
                "Redis is not connected."));
        }
        catch (Exception ex)
        {
            return System.Threading.Tasks.Task.FromResult(HealthCheckResult.Unhealthy(
                $"Redis health check failed: {ex.Message}"));
        }
    }
}