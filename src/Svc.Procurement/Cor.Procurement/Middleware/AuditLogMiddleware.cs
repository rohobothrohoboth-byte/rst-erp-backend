// Middleware/AuditLogMiddleware.cs
using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Cor.Procurement.Models.DTOs;
using Cor.Procurement.Models.Enums;
using Cor.Procurement.Services;
namespace Cor.Procurement.Middleware;

public class AuditLogMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditLogMiddleware> _logger;
    private readonly IAuditLogPublisher _auditPublisher;

    public AuditLogMiddleware(
        RequestDelegate next,
        ILogger<AuditLogMiddleware> logger,
        IAuditLogPublisher auditPublisher)
    {
        _next = next;
        _logger = logger;
        _auditPublisher = auditPublisher;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString();

        try
        {
            // Skip audit for health checks and static files
            if (ShouldSkipAudit(context))
            {
                await _next(context);
                return;
            }

            // Capture request body for POST/PUT/PATCH
            string? requestBody = null;
            if (HttpMethods.IsPost(context.Request.Method) ||
                HttpMethods.IsPut(context.Request.Method) ||
                HttpMethods.IsPatch(context.Request.Method))
            {
                context.Request.EnableBuffering();
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            // Capture response body
            var originalBodyStream = context.Response.Body;
            using var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            await _next(context);

            stopwatch.Stop();

            // Read response body
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            // Get user ID as Guid? (fix for error CS0029)
            Guid? userId = null;
            var userIdClaim = context.User?.FindFirst("sub")?.Value
                              ?? context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var parsedUserId))
            {
                userId = parsedUserId;
            }

            // Publish audit event
            var auditEvent = new AuditLogEventDto
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                UserId = userId,
                UserName = context.User?.Identity?.Name ?? "anonymous",
                UserEmail = context.User?.FindFirst("email")?.Value,
                UserRole = context.User?.FindFirst("role")?.Value ?? context.User?.FindFirst(ClaimTypes.Role)?.Value,
                EntityType = context.Request.Path.Value?.Split('/')[1] ?? "Unknown",
                EntityId = context.Request.RouteValues["id"]?.ToString(),
                Action = context.Request.Method,
                ActionDate = DateTime.UtcNow,
                OldValues = null,
                NewValues = responseBody,
                Changes = requestBody,
                ChangesJson = requestBody,
                IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                ClientIP = context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                RequestId = requestId,
                UserAgent = context.Request.Headers.UserAgent.ToString(),
                QueryString = context.Request.QueryString.ToString(),
                RequestHeaders = JsonSerializer.Serialize(context.Request.Headers.ToDictionary(
                    h => h.Key,
                    h => h.Value.ToString())),
                DurationMs = stopwatch.ElapsedMilliseconds,
                Duration = stopwatch.ElapsedMilliseconds,
                Status = context.Response.StatusCode < 400 ? AuditStatus.Success : AuditStatus.Failed
            };

            await _auditPublisher.PublishAsync(auditEvent);

            // Copy response back to original stream
            await responseBodyStream.CopyToAsync(originalBodyStream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in audit middleware");

            // Create error audit event
            try
            {
                // Get user ID as Guid? (fix for error CS0029)
                Guid? userId = null;
                var userIdClaim = context.User?.FindFirst("sub")?.Value
                                  ?? context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var parsedUserId))
                {
                    userId = parsedUserId;
                }

                var errorAudit = new AuditLogEventDto
                {
                    Id = Guid.NewGuid(),
                    Timestamp = DateTime.UtcNow,
                    UserId = userId,
                    UserName = context.User?.Identity?.Name ?? "anonymous",
                    EntityType = context.Request.Path.Value?.Split('/')[1] ?? "Unknown",
                    Action = context.Request.Method,
                    ActionDate = DateTime.UtcNow,
                    IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    RequestId = requestId,
                    DurationMs = stopwatch.ElapsedMilliseconds,
                    Duration = stopwatch.ElapsedMilliseconds,
                    Status = AuditStatus.Failed,
                    ErrorMessage = ex.Message
                };

                await _auditPublisher.PublishAsync(errorAudit);
            }
            catch (Exception auditEx)
            {
                _logger.LogError(auditEx, "Failed to publish error audit event");
            }

            throw;
        }
    }

    private bool ShouldSkipAudit(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower() ?? string.Empty;
        return path.Contains("/health") ||
               path.Contains("/metrics") ||
               path.Contains("/swagger") ||
               path.Contains("/scalar") ||
               path.Contains("/favicon.ico") ||
               path.EndsWith(".css") ||
               path.EndsWith(".js") ||
               path.EndsWith(".png") ||
               path.EndsWith(".jpg") ||
               path.EndsWith(".svg");
    }
}