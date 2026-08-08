using MediatR;
using Core.Infrastructure.Caching;

namespace Core.Application.Common.Behaviors;

public interface ICacheableQuery
{
    string CacheKey { get; }
    TimeSpan? Expiration { get; }
    bool BypassCache { get; }
}

public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ICacheService _cache;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    public CachingBehavior(ICacheService cache, ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not ICacheableQuery cacheableQuery || cacheableQuery.BypassCache)
            return await next();

        var cacheKey = $"cache:{cacheableQuery.CacheKey}";
        _logger.LogDebug("Checking cache for key {CacheKey}", cacheKey);

        var cachedResponse = await _cache.GetAsync<TResponse>(cacheKey, cancellationToken);
        if (cachedResponse != null)
        {
            _logger.LogDebug("Cache hit for key {CacheKey}", cacheKey);
            return cachedResponse;
        }

        _logger.LogDebug("Cache miss for key {CacheKey}, executing query", cacheKey);
        var response = await next();

        await _cache.SetAsync(cacheKey, response, cacheableQuery.Expiration, cancellationToken);
        return response;
    }
}