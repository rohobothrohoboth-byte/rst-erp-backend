using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Channels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Shared.Helpers.Audit;

/// <summary>
/// Captures write requests (POST/PUT/PATCH/DELETE) and queues an <see cref="AuditLog"/>
/// onto an in-process channel; a background writer persists them. Shared across modules.
/// </summary>
public class AuditLogMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditLogMiddleware> _logger;
    private readonly Channel<AuditLog> _channel;

    public AuditLogMiddleware(RequestDelegate next, ILogger<AuditLogMiddleware> logger, Channel<AuditLog> channel)
    {
        _next = next;
        _logger = logger;
        _channel = channel;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.ToString();
        var method = context.Request.Method;

        if (ShouldSkip(path) || !IsWrite(method))
        {
            await _next(context);
            return;
        }

        var sw = Stopwatch.StartNew();
        Exception? error = null;
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            error = ex;
            throw;
        }
        finally
        {
            try
            {
                var log = Build(context, sw.Elapsed, error);
                await _channel.Writer.WriteAsync(log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to enqueue audit log");
            }
        }
    }

    private static bool IsWrite(string method) =>
        method is "POST" or "PUT" or "PATCH" or "DELETE";

    private static readonly string[] SkipContains =
        { "/health", "/metrics", "/scalar", "/swagger", "/favicon", "/Audit/", "/Debug/" };

    private static bool ShouldSkip(string path) =>
        SkipContains.Any(p => path.Contains(p, StringComparison.OrdinalIgnoreCase));

    private static AuditLog Build(HttpContext context, TimeSpan duration, Exception? error)
    {
        var user = context.User;
        var userIdRaw = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user?.FindFirst("sub")?.Value;
        Guid? userId = Guid.TryParse(userIdRaw, out var g) ? g : null;

        var status = error != null ? AuditStatus.Error
            : context.Response.StatusCode is 401 or 403 ? AuditStatus.Unauthorized
            : AuditStatus.Success;

        return new AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            UserName = user?.FindFirst(ClaimTypes.Name)?.Value ?? user?.FindFirst("name")?.Value ?? "Anonymous",
            UserEmail = user?.FindFirst(ClaimTypes.Email)?.Value ?? user?.FindFirst("email")?.Value,
            UserRole = user?.FindFirst(ClaimTypes.Role)?.Value ?? user?.FindFirst("role")?.Value ?? "guest",
            Action = $"{context.Request.Method} {context.Request.Path}",
            EntityType = EntityType(context.Request.Path),
            EntityId = EntityId(context.Request.Path),
            ActionDate = DateTime.UtcNow,
            Status = status,
            DurationMs = (long)duration.TotalMilliseconds,
            IpAddress = ClientIp(context),
            UserAgent = context.Request.Headers.UserAgent.ToString(),
            RequestId = context.TraceIdentifier,
            QueryString = context.Request.QueryString.ToString(),
            RequestHeaders = SerializeHeaders(context),
            ErrorMessage = error?.Message
        };
    }

    private static string ClientIp(HttpContext context)
    {
        var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (string.IsNullOrEmpty(ip)) ip = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        return string.IsNullOrEmpty(ip) ? context.Connection.RemoteIpAddress?.ToString() ?? "unknown" : ip;
    }

    private static string SerializeHeaders(HttpContext context)
    {
        try
        {
            var headers = context.Request.Headers
                .Where(h => !h.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase)
                            && !h.Key.Equals("Cookie", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(h => h.Key, h => h.Value.ToString());
            return JsonSerializer.Serialize(headers);
        }
        catch { return "{}"; }
    }

    private static string EntityType(string path)
    {
        var segments = path.Split('?')[0].Split('/', StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < segments.Length; i++)
            if (segments[i].Equals("api", StringComparison.OrdinalIgnoreCase) && i + 3 < segments.Length)
                return segments[i + 3];
        return segments.LastOrDefault() ?? "Unknown";
    }

    private static string? EntityId(string path)
        => path.Split('/').FirstOrDefault(s => Guid.TryParse(s, out _));
}
