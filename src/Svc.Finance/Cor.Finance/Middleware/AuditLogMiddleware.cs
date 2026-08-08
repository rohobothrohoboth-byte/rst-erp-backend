// Middleware/AuditLogMiddleware.cs
using Cor.Finance.Helpers;
using Cor.Finance.Models.Entities;
using Cor.Finance.Models.Enums;
using Cor.Finance.Services;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Channels;

namespace Cor.Finance.Middlewares;

public class AuditLogMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditLogMiddleware> _logger;
    private readonly Channel<AuditLog> _auditChannel;

    public AuditLogMiddleware(
        RequestDelegate next,
        ILogger<AuditLogMiddleware> logger,
        Channel<AuditLog> auditChannel)
    {
        _next = next;
        _logger = logger;
        _auditChannel = auditChannel;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.ToString();
        var method = context.Request.Method;

        // Skip health and static endpoints
        if (ShouldSkipAudit(path))
        {
            await _next(context);
            return;
        }

        // ✅ Check if we should audit based on method
        if (!ShouldAuditByMethod(method))
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
            var auditLog = BuildAuditLog(context, path, sw.Elapsed, error);

            // Write to channel (non-blocking)
            await _auditChannel.Writer.WriteAsync(auditLog);

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("Audit {User} {Action}", auditLog.UserName, auditLog.Action);
        }
    }

    /// <summary>
    /// ✅ Determine if we should audit based on HTTP method
    /// </summary>
    private static bool ShouldAuditByMethod(string method)
    {
        // Audit all write operations (POST, PUT, PATCH, DELETE)
        if (method == "POST" || method == "PUT" || method == "PATCH" || method == "DELETE")
            return true;

        // Audit GET requests only for specific sensitive endpoints
        // (Optional: Remove this if you don't want to audit GETs)
        if (method == "GET")
        {
            // You can add specific GET paths that need auditing here
            // For example: /api/finance/v1.0/Invoice/All
            // But generally, we don't audit GETs to reduce noise
            return false;
        }

        // Audit other methods (HEAD, OPTIONS, etc.)
        return false;
    }

    private AuditLog BuildAuditLog(HttpContext context, string path, TimeSpan duration, Exception? error)
    {
        var user = context.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? user?.FindFirst("sub")?.Value;

        var userName = user?.FindFirst(ClaimTypes.Name)?.Value
                       ?? user?.FindFirst("name")?.Value
                       ?? "Anonymous";

        var userEmail = user?.FindFirst(ClaimTypes.Email)?.Value
                        ?? user?.FindFirst("email")?.Value;

        var userRole = user?.FindFirst(ClaimTypes.Role)?.Value
                       ?? user?.FindFirst("role")?.Value
                       ?? "guest";

        var entityType = GetEntityType(path);
        var entityId = GetEntityId(path);
        var ipAddress = GetClientIpAddress(context);
        var userAgent = context.Request.Headers["User-Agent"].ToString();

        var status = error == null ? AuditStatus.Success : AuditStatus.Error;
        if (context.Response.StatusCode == 401 || context.Response.StatusCode == 403)
        {
            status = AuditStatus.Unauthorized;
        }

        return new AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = !string.IsNullOrEmpty(userId) ? Guid.Parse(userId) : null,
            UserName = userName,
            UserEmail = userEmail,
            UserRole = userRole,
            Action = $"{context.Request.Method} {context.Request.Path}",
            EntityType = entityType,
            EntityId = entityId,
            ActionDate = DateTime.UtcNow,
            Status = status,
            DurationMs = (long)duration.TotalMilliseconds,
            ClientIP = ipAddress,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            RequestId = context.TraceIdentifier,
            QueryString = context.Request.QueryString.ToString(),
            RequestHeaders = GetSerializedHeaders(context),
            ErrorMessage = error?.Message,
            IsDeleted = false,
            DateAdd = DateTime.UtcNow,
            DateMod = DateTime.UtcNow
        };
    }

    private string GetSerializedHeaders(HttpContext context)
    {
        try
        {
            var headers = context.Request.Headers
                .Where(h => !h.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase) &&
                           !h.Key.Equals("Cookie", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(h => h.Key, h => h.Value.ToString());

            return JsonSerializer.Serialize(headers);
        }
        catch
        {
            return "{}";
        }
    }

    private string GetClientIpAddress(HttpContext context)
    {
        var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (string.IsNullOrEmpty(ip))
            ip = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (string.IsNullOrEmpty(ip))
            ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return ip;
    }

    private static bool ShouldSkipAudit(string path)
    {
        var skipPaths = new[]
        {
            "/health",
            "/metrics",
            "/scalar",
            "/swagger",
            "/favicon.ico",
            "/Audit/Logs",
            "/Audit/Summary",
            "/Debug/claims"
        };
        return skipPaths.Any(p => path.Contains(p, StringComparison.OrdinalIgnoreCase));
    }

    private static string GetEntityType(string path)
    {
        try
        {
            var pathWithoutQuery = path.Split('?')[0];
            var segments = pathWithoutQuery.Split('/', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i].Equals("api", StringComparison.OrdinalIgnoreCase) && i + 2 < segments.Length)
                {
                    var entityIndex = i + 3;
                    if (entityIndex < segments.Length)
                    {
                        var entityName = segments[entityIndex];

                        if (entityName == "Audit" && entityIndex + 1 < segments.Length)
                        {
                            return $"{entityName}{segments[entityIndex + 1]}";
                        }

                        return entityName;
                    }
                }
            }

            var lastSegment = segments.LastOrDefault();
            if (!string.IsNullOrEmpty(lastSegment) && !lastSegment.Contains("v1.0") && !lastSegment.Contains("api"))
            {
                return lastSegment;
            }

            return "Unknown";
        }
        catch
        {
            return "Unknown";
        }
    }

    private static string? GetEntityId(string path)
    {
        try
        {
            var segments = path.Split('/');
            foreach (var segment in segments)
            {
                if (Guid.TryParse(segment, out _))
                {
                    return segment;
                }
            }
            return null;
        }
        catch
        {
            return null;
        }
    }
}