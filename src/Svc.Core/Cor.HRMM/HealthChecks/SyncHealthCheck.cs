using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Cor.HRMM.HealthChecks;

public class SyncHealthCheck : IHealthCheck
{
    private static DateTime _lastSuccessfulSync = DateTime.UtcNow;
    private static bool _isHealthy = true;
    private static string _lastError = string.Empty;
    private static int _successfulSyncs = 0;
    private static int _failedSyncs = 0;

    public static void RecordSyncSuccess()
    {
        _lastSuccessfulSync = DateTime.UtcNow;
        _isHealthy = true;
        _successfulSyncs++;
        _lastError = string.Empty;
    }

    public static void RecordSyncFailure(Exception ex)
    {
        _isHealthy = false;
        _failedSyncs++;
        _lastError = ex.Message;
    }

    public static SyncHealthStatus GetStatus()
    {
        return new SyncHealthStatus
        {
            IsHealthy = _isHealthy,
            LastSuccessfulSync = _lastSuccessfulSync,
            SuccessfulSyncs = _successfulSyncs,
            FailedSyncs = _failedSyncs,
            LastError = _lastError,
            TimeSinceLastSync = DateTime.UtcNow - _lastSuccessfulSync
        };
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var timeSinceLastSync = DateTime.UtcNow - _lastSuccessfulSync;
        var data = new Dictionary<string, object>
        {
            { "LastSuccessfulSync", _lastSuccessfulSync },
            { "TimeSinceLastSync", timeSinceLastSync },
            { "IsHealthy", _isHealthy },
            { "SuccessfulSyncs", _successfulSyncs },
            { "FailedSyncs", _failedSyncs }
        };

        if (!_isHealthy)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy(
                $"Sync has failed. Last error: {_lastError}", data: data));
        }

        if (timeSinceLastSync > TimeSpan.FromHours(24))
        {
            return Task.FromResult(HealthCheckResult.Degraded(
                $"No successful sync in the last 24 hours. Last sync: {_lastSuccessfulSync}", data: data));
        }

        return Task.FromResult(HealthCheckResult.Healthy(
            $"Sync is working properly. Last sync: {_lastSuccessfulSync}", data: data));
    }
}

public class SyncHealthStatus
{
    public bool IsHealthy { get; set; }
    public DateTime LastSuccessfulSync { get; set; }
    public int SuccessfulSyncs { get; set; }
    public int FailedSyncs { get; set; }
    public string LastError { get; set; } = string.Empty;
    public TimeSpan TimeSinceLastSync { get; set; }
}
