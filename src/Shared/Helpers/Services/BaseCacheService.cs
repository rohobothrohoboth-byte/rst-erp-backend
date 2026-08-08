// src/Shared/Helpers/Services/BaseCacheService.cs
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Helpers.Services;

public abstract class BaseCacheService : ICacheService
{
    protected readonly ILogger _logger;

    protected BaseCacheService(ILogger logger)
    {
        _logger = logger;
    }

    // Abstract methods that implement the interface
    public abstract Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class;
    public abstract Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default);
    public abstract Task RemoveAsync(string key, CancellationToken ct = default);
    public abstract Task RemoveByPatternAsync(string pattern, CancellationToken ct = default);

    // Virtual methods that can be overridden
    public virtual async Task<bool> ExistsAsync(string key, CancellationToken ct = default)
    {
        var value = await GetAsync<object>(key, ct);
        return value != null;
    }

    public virtual async Task<T?> GetOrCreateAsync<T>(string key, Func<CancellationToken, Task<T>> factory, TimeSpan? expiry = null, CancellationToken ct = default) where T : class
    {
        var cached = await GetAsync<T>(key, ct);
        if (cached != null)
            return cached;

        var result = await factory(ct);
        if (result != null)
            await SetAsync(key, result, expiry, ct);

        return result;
    }

    public virtual async Task<T?> GetOrCreateWithLockAsync<T>(string key, Func<CancellationToken, Task<T>> factory, TimeSpan? expiry = null, CancellationToken ct = default) where T : class
    {
        // Simple implementation without lock - override if you need distributed locking
        return await GetOrCreateAsync(key, factory, expiry, ct);
    }
}