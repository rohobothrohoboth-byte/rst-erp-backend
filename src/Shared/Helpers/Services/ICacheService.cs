// Shared/Helpers/Services/ICacheService.cs
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Helpers.Services;

public interface ICacheService
{
    // Main methods with CancellationToken
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default);
    Task RemoveAsync(string key, CancellationToken ct = default);
    Task RemoveByPatternAsync(string pattern, CancellationToken ct = default);
    Task<bool> ExistsAsync(string key, CancellationToken ct = default);
    Task<T?> GetOrCreateAsync<T>(string key, Func<CancellationToken, Task<T>> factory, TimeSpan? expiry = null, CancellationToken ct = default) where T : class;
    Task<T?> GetOrCreateWithLockAsync<T>(string key, Func<CancellationToken, Task<T>> factory, TimeSpan? expiry = null, CancellationToken ct = default) where T : class;
}