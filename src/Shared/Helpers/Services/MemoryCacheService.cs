// src/Shared/Helpers/Services/MemoryCacheService.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Shared.Helpers.Services;

public class MemoryCacheService : BaseCacheService, ICacheService
{
    private readonly IMemoryCache _cache;

    public MemoryCacheService(IMemoryCache cache, ILogger<MemoryCacheService> logger)
        : base(logger)
    {
        _cache = cache;
    }

    public override Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class // ✅ Add class constraint
    {
        _cache.TryGetValue(key, out T? value);
        return Task.FromResult(value);
    }

    public override Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default)
    {
        var options = new MemoryCacheEntryOptions();
        if (expiry.HasValue)
            options.AbsoluteExpirationRelativeToNow = expiry;
        else
            options.SlidingExpiration = TimeSpan.FromMinutes(15);

        _cache.Set(key, value, options);
        return Task.CompletedTask;
    }

    public override Task RemoveAsync(string key, CancellationToken ct = default)
    {
        _cache.Remove(key);
        return Task.CompletedTask;
    }

    public override Task RemoveByPatternAsync(string pattern, CancellationToken ct = default)
    {
        _logger.LogWarning("RemoveByPatternAsync not implemented for MemoryCache");
        return Task.CompletedTask;
    }


}