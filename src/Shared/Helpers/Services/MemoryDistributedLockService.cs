// src/Shared/Helpers/Services/MemoryDistributedLockService.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Shared.Helpers.Services;

public class MemoryDistributedLockService : IDistributedLockService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<MemoryDistributedLockService> _logger;

    public MemoryDistributedLockService(IMemoryCache cache, ILogger<MemoryDistributedLockService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public Task<bool> AcquireLockAsync(string lockKey, TimeSpan expiry, CancellationToken ct = default)
    {
        try
        {
            var lockValue = Guid.NewGuid().ToString();
            var isAcquired = _cache.Set(lockKey, lockValue, expiry);

            if (isAcquired != null)
            {
                _logger.LogDebug("🔒 Memory lock acquired: {LockKey}", lockKey);
                return Task.FromResult(true);
            }

            _logger.LogDebug("🔒 Memory lock already held: {LockKey}", lockKey);
            return Task.FromResult(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to acquire memory lock: {LockKey}", lockKey);
            return Task.FromResult(false);
        }
    }

    public Task ReleaseLockAsync(string lockKey, CancellationToken ct = default)
    {
        try
        {
            _cache.Remove(lockKey);
            _logger.LogDebug("🔓 Memory lock released: {LockKey}", lockKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to release memory lock: {LockKey}", lockKey);
        }

        return Task.CompletedTask;
    }
}