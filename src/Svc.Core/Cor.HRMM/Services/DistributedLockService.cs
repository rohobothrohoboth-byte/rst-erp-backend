// Services/DistributedLockService.cs
using StackExchange.Redis;
using Shared.Helpers.Services;
namespace Cor.HRMM.Services;



public class DistributedLockService : IDistributedLockService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<DistributedLockService> _logger;

    public DistributedLockService(IConnectionMultiplexer redis, ILogger<DistributedLockService> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task<bool> AcquireLockAsync(string lockKey, TimeSpan expiry, CancellationToken ct = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var acquired = await db.LockTakeAsync(lockKey, Environment.MachineName, expiry);

            if (acquired)
            {
                _logger.LogInformation("🔒 Lock acquired: {LockKey}", lockKey);
            }
            else
            {
                _logger.LogDebug("🔒 Lock already held: {LockKey}", lockKey);
            }

            return acquired;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to acquire lock: {LockKey}", lockKey);
            return false;
        }
    }

    public async Task ReleaseLockAsync(string lockKey, CancellationToken ct = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            await db.LockReleaseAsync(lockKey, Environment.MachineName);
            _logger.LogInformation("🔓 Lock released: {LockKey}", lockKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to release lock: {LockKey}", lockKey);
        }
    }
}