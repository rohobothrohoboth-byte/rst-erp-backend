using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Profile.App.Commands;
using Profile.App.Queries;
using Profile.Domain.DTOs;
using Profile.App.Interfaces;
using Profile.Domain.Entities;
using Common;
using Dapper;
using Contracts;
using EthiopianCalendar;
using Profile.App.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
namespace Profile.App.Services;

public interface ICachedReferenceService
{
    Task<(Dictionary<Guid, CorModListAm> Departments, Dictionary<Guid, CorHrmmList> Positions, Dictionary<Guid, CorHrmmList> JobGrades)>
        GetReferenceDataAsync(CancellationToken ct);

    Task<Dictionary<Guid, CorModListAm>> GetDepartmentsAsync(CancellationToken ct);
    Task<Dictionary<Guid, CorHrmmList>> GetPositionsAsync(CancellationToken ct);
    Task<Dictionary<Guid, CorHrmmList>> GetJobGradesAsync(CancellationToken ct);

    void InvalidateCache();
}

public class CachedReferenceService : ICachedReferenceService
{
    private readonly ICorModClient _corMod;
    private readonly ICorHrmmClient _corHRMM;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachedReferenceService> _logger;

    private const string DEPT_CACHE_KEY = "ref_dept";
    private const string POS_CACHE_KEY = "ref_pos";
    private const string JG_CACHE_KEY = "ref_jg";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

    private static readonly SemaphoreSlim _cacheLock = new SemaphoreSlim(1, 1);

    public CachedReferenceService(
        ICorModClient corMod,
        ICorHrmmClient corHRMM,
        IMemoryCache cache,
        ILogger<CachedReferenceService> logger)
    {
        _corMod = corMod;
        _corHRMM = corHRMM;
        _cache = cache;
        _logger = logger;
    }

    public async Task<(Dictionary<Guid, CorModListAm> Departments, Dictionary<Guid, CorHrmmList> Positions, Dictionary<Guid, CorHrmmList> JobGrades)>
        GetReferenceDataAsync(CancellationToken ct)
    {
        var deptTask = GetDepartmentsAsync(ct);
        var posTask = GetPositionsAsync(ct);
        var jgTask = GetJobGradesAsync(ct);

        await Task.WhenAll(deptTask, posTask, jgTask);

        return (await deptTask, await posTask, await jgTask);
    }

    public async Task<Dictionary<Guid, CorModListAm>> GetDepartmentsAsync(CancellationToken ct)
    {
        return await GetOrCreateAsync(DEPT_CACHE_KEY, async () =>
        {
            _logger.LogDebug("Fetching departments from gRPC...");
            var response = await _corMod.GetListDept(ct);
            return response.Res.ToDictionary(d => Guid.Parse(d.Id));
        }, ct);
    }

    public async Task<Dictionary<Guid, CorHrmmList>> GetPositionsAsync(CancellationToken ct)
    {
        return await GetOrCreateAsync(POS_CACHE_KEY, async () =>
        {
            _logger.LogDebug("Fetching positions from gRPC...");
            var response = await _corHRMM.GetListPosition(ct);
            return response.Res.ToDictionary(p => Guid.Parse(p.Id));
        }, ct);
    }

    public async Task<Dictionary<Guid, CorHrmmList>> GetJobGradesAsync(CancellationToken ct)
    {
        return await GetOrCreateAsync(JG_CACHE_KEY, async () =>
        {
            _logger.LogDebug("Fetching job grades from gRPC...");
            var response = await _corHRMM.GetListJobGrade(ct);
            return response.Res.ToDictionary(p => Guid.Parse(p.Id));
        }, ct);
    }

    private async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, CancellationToken ct) where T : class
    {
        if (_cache.TryGetValue(key, out T? cached))
        {
            _logger.LogDebug("Cache HIT: {Key}", key);
            return cached!;
        }

        await _cacheLock.WaitAsync(ct);
        try
        {
            // Double-check after lock
            if (_cache.TryGetValue(key, out T? cachedAfterLock))
            {
                return cachedAfterLock!;
            }

            _logger.LogDebug("Cache MISS: {Key}, fetching...", key);
            var result = await factory();

            if (result != null)
            {
                _cache.Set(key, result, CacheDuration);
            }

            return result!;
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    public void InvalidateCache()
    {
        _logger.LogInformation("Invalidating reference cache...");
        _cache.Remove(DEPT_CACHE_KEY);
        _cache.Remove(POS_CACHE_KEY);
        _cache.Remove(JG_CACHE_KEY);
    }
}