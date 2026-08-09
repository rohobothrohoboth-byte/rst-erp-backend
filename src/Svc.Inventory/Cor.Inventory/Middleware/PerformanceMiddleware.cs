// Middleware/PerformanceMiddleware.cs

using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
//using Cor.Inventory.Helpers;
using Cor.Inventory.Models.Entities;
using Cor.Inventory.Models.Enums;
using Cor.Inventory.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Channels;
using Microsoft.IdentityModel.Tokens;
namespace Cor.Inventory.Middlewares;

public class PerformanceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceMiddleware> _logger;
    private readonly IMemoryCache _cache;

    public PerformanceMiddleware(
        RequestDelegate next,
        ILogger<PerformanceMiddleware> logger,
        IMemoryCache cache)
    {
        _next = next;
        _logger = logger;
        _cache = cache;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var path = context.Request.Path;
        var method = context.Request.Method;

        // ✅ Track database query count
        var dbQueryCount = 0;
        var dbQueryStart = DateTime.UtcNow;

        try
        {
            // Use a scope to track database queries
            using (var scope = _logger.BeginScope(new { Path = path, Method = method }))
            {
                await _next(context);
            }
        }
        finally
        {
            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            // ✅ Log performance metrics
            _logger.LogInformation(
                "📊 {Method} {Path} completed in {ElapsedMs}ms | Status: {StatusCode} | DB Queries: {DbQueryCount}",
                method,
                path,
                elapsedMs,
                context.Response.StatusCode,
                dbQueryCount
            );

            // ✅ Store in cache for dashboard
            var cacheKey = $"perf:{DateTime.UtcNow:yyyy-MM-dd-HH-mm}";
            var metrics = _cache.Get<PerformanceMetrics>(cacheKey) ?? new PerformanceMetrics();
            metrics.TotalRequests++;
            metrics.AvgResponseTime = ((metrics.AvgResponseTime * (metrics.TotalRequests - 1)) + elapsedMs) / metrics.TotalRequests;
            metrics.TotalDbQueries += dbQueryCount;
            _cache.Set(cacheKey, metrics, TimeSpan.FromMinutes(5));
        }
    }
}

public class PerformanceMetrics
{
    public int TotalRequests { get; set; }
    public double AvgResponseTime { get; set; }
    public int TotalDbQueries { get; set; }
}