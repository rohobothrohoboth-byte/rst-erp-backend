// src/Shared/Helpers/Extensions/CacheServiceExtensions.cs - Simplified
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Shared.Helpers.Services;

namespace Shared.Helpers.Extensions;

public static class CacheServiceExtensions
{
    public static IServiceCollection AddCacheService(this IServiceCollection services, IConfiguration configuration)
    {
        // Check if Redis is enabled
        var useRedis = configuration["CacheSettings:UseRedis"] == "true";

        if (useRedis)
        {
            var redisConnectionString = configuration.GetConnectionString("Redis");

            if (!string.IsNullOrEmpty(redisConnectionString))
            {
                // Register Redis
                services.AddSingleton<IConnectionMultiplexer>(sp =>
                    ConnectionMultiplexer.Connect(redisConnectionString));
                services.AddScoped<ICacheService, RedisCacheService>();
                return services;
            }
        }

        // Default: Use MemoryCache
        services.AddMemoryCache();
        services.AddScoped<ICacheService, MemoryCacheService>();
        return services;
    }
}