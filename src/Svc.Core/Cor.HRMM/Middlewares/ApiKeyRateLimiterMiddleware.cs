// E:\untitled46\RST_ERP\src\Svc.Core\Cor.HRMM\Middleware\ApiKeyRateLimiterMiddleware.cs
using System.Collections.Concurrent;
using Microsoft.Extensions.Options;

namespace Cor.HRMM.Middleware;

public class ApiKeyRateLimiterMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiKeyRateLimiterMiddleware> _logger;
    private readonly ConcurrentDictionary<string, RateLimitInfo> _clientLimits = new();
    private readonly ApiKeyRateLimitOptions _options;

    public ApiKeyRateLimiterMiddleware(
        RequestDelegate next,
        ILogger<ApiKeyRateLimiterMiddleware> logger,
        IOptions<ApiKeyRateLimitOptions> options)
    {
        _next = next;
        _logger = logger;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("X-API-Key", out var apiKeyHeader))
        {
            await _next(context);
            return;
        }

        var apiKey = apiKeyHeader.FirstOrDefault();
        if (string.IsNullOrEmpty(apiKey))
        {
            await _next(context);
            return;
        }

        if (_options.Enabled && !IsRateLimitAllowed(apiKey, out var retryAfter))
        {
            _logger.LogWarning("Rate limit exceeded for API Key");

            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.Headers["Retry-After"] = retryAfter.ToString();
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "Rate limit exceeded. Please try again later.",
                retryAfter = retryAfter.Seconds
            });
            return;
        }

        await _next(context);
    }

    private bool IsRateLimitAllowed(string apiKey, out TimeSpan retryAfter)
    {
        retryAfter = TimeSpan.Zero;

        var limitInfo = _clientLimits.GetOrAdd(apiKey, new RateLimitInfo
        {
            WindowStart = DateTime.UtcNow,
            RequestCount = 0
        });

        lock (limitInfo)
        {
            var now = DateTime.UtcNow;
            var windowElapsed = now - limitInfo.WindowStart;

            if (windowElapsed > TimeSpan.FromMinutes(1))
            {
                limitInfo.WindowStart = now;
                limitInfo.RequestCount = 1;
                return true;
            }

            if (limitInfo.RequestCount >= _options.MaxRequestsPerMinute)
            {
                retryAfter = TimeSpan.FromMinutes(1) - windowElapsed;
                return false;
            }

            limitInfo.RequestCount++;
            return true;
        }
    }

    private class RateLimitInfo
    {
        public DateTime WindowStart { get; set; }
        public int RequestCount { get; set; }
    }
}

public class ApiKeyRateLimitOptions
{
    public bool Enabled { get; set; } = true;
    public int MaxRequestsPerMinute { get; set; } = 60;
}