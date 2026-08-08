using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Svc.Notification.Commands;
using Svc.Notification.Models.Dtos;
using Svc.Notification.Queries;
using Svc.Notification.Services;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Svc.Notification.Controllers;

[ApiController]
[Route("api/auth/v{version:apiVersion}/Notification")]
[ApiVersion("1.0")]
public class NotificationController : ControllerBase
{
    private readonly IMediator _med;
    private readonly IBulkNotificationService _bulkService;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(
        IMediator med,
        IBulkNotificationService bulkService,
        ILogger<NotificationController> logger)
    {
        _med = med;
        _bulkService = bulkService;
        _logger = logger;
    }

    [HttpGet("user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserNotifications(Guid userId, [FromQuery] bool? read = null, [FromQuery] string? type = null, [FromQuery] int? limit = 50)
    {
        var result = await _med.Send(new NotificationAllQry { UserId = userId, IsRead = read, Type = type, Limit = limit });
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetNotificationById(Guid id)
    {
        var result = await _med.Send(new NotificationByIdQry { Id = id });
        if (result == null)
            throw new DomainException($"Notification with id [{id}] not found");
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("unread/count/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnreadCount(Guid userId)
    {
        var count = await _med.Send(new NotificationUnreadCountQry { UserId = userId });
        return Ok(ApiResponse<object>.Ok(count));
    }

    [HttpGet("stats/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNotificationStats(Guid userId)
    {
        var stats = await _med.Send(new NotificationStatsQry { UserId = userId });
        return Ok(ApiResponse<object>.Ok(stats));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateNotification([FromBody] NotificationAddDto dto)
    {
        var result = await _med.Send(new NotificationAddCmd { AddDto = dto });
        return Ok(ApiResponse<object>.Ok(result, "Notification created successfully"));
    }

    /// <summary>
    /// Create bulk notifications for multiple users (efficient for 5000+ employees)
    /// </summary>
    [HttpPost("bulk")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBulkNotifications([FromBody] BulkNotificationRequest request)
    {
        if (request.UserIds == null || !request.UserIds.Any())
            throw new DomainException("UserIds list cannot be empty");

        if (request.UserIds.Count > 10000)
            throw new DomainException("Cannot send more than 10,000 notifications at once");

        await _bulkService.SendBulkNotificationsAsync(request.UserIds, request.Notification);

        _logger.LogInformation("Bulk notifications queued for {Count} users", request.UserIds.Count);
        return Ok(ApiResponse<string>.Ok(null, $"Queued {request.UserIds.Count} notifications successfully"));
    }

    /// <summary>
    /// Send notification to all employees in a department
    /// </summary>
    [HttpPost("department/{departmentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SendToDepartment(Guid departmentId, [FromBody] CreateNotificationDto notification)
    {
        await _bulkService.SendToDepartmentAsync(departmentId, notification);
        return Ok(ApiResponse<string>.Ok(null, $"Notifications queued for department {departmentId}"));
    }

    /// <summary>
    /// Send notification to all active employees
    /// </summary>
    [HttpPost("all-employees")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SendToAllEmployees([FromBody] CreateNotificationDto notification)
    {
        await _bulkService.SendToAllEmployeesAsync(notification);
        return Ok(ApiResponse<string>.Ok(null, "Notifications queued for all employees"));
    }

    /// <summary>
    /// Send notification by employee type (Permanent, Contract, etc.)
    /// </summary>
    [HttpPost("by-employment-type")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SendByEmploymentType([FromBody] EmploymentTypeNotificationRequest request)
    {
        await _bulkService.SendByEmploymentTypeAsync(request.EmploymentType, request.Notification);
        return Ok(ApiResponse<string>.Ok(null, $"Notifications queued for {request.EmploymentType} employees"));
    }

    [HttpPatch("{id}/read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        await _med.Send(new NotificationMarkAsReadCmd { Id = id });
        return Ok(ApiResponse<object>.Ok(null, "Notification marked as read"));
    }

    [HttpPatch("user/{userId}/read-all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkAllAsRead(Guid userId)
    {
        await _med.Send(new NotificationMarkAllAsReadCmd { UserId = userId });
        return Ok(ApiResponse<object>.Ok(null, "All notifications marked as read"));
    }

    /// <summary>
    /// Mark multiple notifications as read in batch
    /// </summary>
    [HttpPatch("batch-read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkBatchAsRead([FromBody] BatchReadRequest request)
    {
        var result = await _med.Send(new NotificationBatchMarkAsReadCmd { NotificationIds = request.NotificationIds });
        return Ok(ApiResponse<int>.Ok(result, $"{result} notifications marked as read"));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteNotification(Guid id)
    {
        await _med.Send(new NotificationDeleteCmd { Id = id });
        return Ok(ApiResponse<object>.Ok(null, "Notification deleted successfully"));
    }

    [HttpDelete("user/{userId}/clear")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ClearAllNotifications(Guid userId)
    {
        await _med.Send(new NotificationClearAllCmd { UserId = userId });
        return Ok(ApiResponse<object>.Ok(null, "All notifications cleared"));
    }

    /// <summary>
    /// Delete old notifications (older than specified days)
    /// </summary>
    [HttpDelete("cleanup")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CleanupOldNotifications([FromQuery] int daysOld = 90)
    {
        var count = await _med.Send(new NotificationCleanupCmd { DaysOld = daysOld });
        return Ok(ApiResponse<int>.Ok(count, $"Deleted {count} old notifications"));
    }

    /// <summary>
    /// Get paginated notifications with advanced filtering (MOST EFFICIENT)
    /// </summary>
    [HttpGet("paginated")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaginatedNotifications([FromQuery] NotificationPaginatedQry query)
    {
        var result = await _med.Send(query);
        return Ok(ApiResponse<PaginatedResult<NotificationDto>>.Ok(result));
    }

    /// <summary>
    /// Get real-time notification stream (Server-Sent Events) - ENHANCED VERSION
    /// </summary>
    [HttpGet("stream/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task StreamNotifications(Guid userId, CancellationToken cancellationToken)
    {
        // Log connection attempt
        _logger.LogInformation("SSE connection requested for user {UserId}", userId);

        // Set response headers for SSE
        Response.Headers.Append("Content-Type", "text/event-stream");
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        Response.Headers.Append("X-Accel-Buffering", "no"); // Disable buffering for nginx

        // Generate connection ID for tracking
        var connectionId = Guid.NewGuid();

        try
        {
            // Send initial connection event
            var connectionEvent = new
            {
                type = "connected",
                connectionId = connectionId,
                userId = userId,
                timestamp = DateTime.UtcNow
            };
            await WriteSseEventAsync("connected", connectionEvent, cancellationToken);

            _logger.LogInformation("SSE connection established for user {UserId} with ID {ConnectionId}", userId, connectionId);

            // Get initial unread count
            var initialUnreadCount = await _med.Send(new NotificationUnreadCountQry { UserId = userId }, cancellationToken);
            var statsEvent = new
            {
                type = "stats",
                unreadCount = initialUnreadCount,
                timestamp = DateTime.UtcNow
            };
            await WriteSseEventAsync("stats", statsEvent, cancellationToken);

            // Track last check time
            var lastCheckTime = DateTime.UtcNow;
            var heartbeatCounter = 0;

            // Keep connection alive
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // Check for new notifications
                    var newNotifications = await _med.Send(new NotificationStreamQry
                    {
                        UserId = userId,
                        Since = lastCheckTime
                    }, cancellationToken);

                    // Send notifications if any
                    if (newNotifications != null && newNotifications.Any())
                    {
                        _logger.LogDebug("Sending {Count} notifications to user {UserId}", newNotifications.Count(), userId);

                        foreach (var notification in newNotifications)
                        {
                            var notificationEvent = new
                            {
                                type = "notification",
                                data = notification,
                                timestamp = DateTime.UtcNow
                            };
                            await WriteSseEventAsync("notification", notificationEvent, cancellationToken);
                        }

                        // Also send updated stats
                        var updatedUnreadCount = await _med.Send(new NotificationUnreadCountQry { UserId = userId }, cancellationToken);
                        var updatedStatsEvent = new
                        {
                            type = "stats",
                            unreadCount = updatedUnreadCount,
                            timestamp = DateTime.UtcNow
                        };
                        await WriteSseEventAsync("stats", updatedStatsEvent, cancellationToken);

                        lastCheckTime = DateTime.UtcNow;
                    }

                    // Send heartbeat every 15 seconds to keep connection alive
                    heartbeatCounter++;
                    if (heartbeatCounter >= 3) // Every 15 seconds (3 * 5 second delays)
                    {
                        await WriteSseCommentAsync($"heartbeat {DateTime.UtcNow:O}", cancellationToken);
                        heartbeatCounter = 0;
                    }

                    // Wait before next check
                    await Task.Delay(5000, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("SSE connection cancelled for user {UserId}", userId);
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in SSE stream loop for user {UserId}", userId);
                    // Send error event to client
                    var errorEvent = new
                    {
                        type = "error",
                        message = "Internal server error",
                        timestamp = DateTime.UtcNow
                    };
                    await WriteSseEventAsync("error", errorEvent, cancellationToken);
                    await Task.Delay(10000, cancellationToken); // Wait longer before retry
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("SSE connection closed for user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal error in SSE stream for user {UserId}", userId);
        }
        finally
        {
            // Send disconnect event
            try
            {
                var disconnectEvent = new
                {
                    type = "disconnected",
                    connectionId = connectionId,
                    userId = userId,
                    timestamp = DateTime.UtcNow
                };
                await WriteSseEventAsync("disconnected", disconnectEvent, cancellationToken);
            }
            catch { /* Ignore disposal errors */ }

            _logger.LogInformation("SSE connection closed for user {UserId}", userId);
        }
    }

    /// <summary>
    /// Helper method to write SSE events
    /// </summary>
    private async Task WriteSseEventAsync(string eventName, object data, CancellationToken cancellationToken)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(data, new System.Text.Json.JsonSerializerOptions
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        });

        await Response.WriteAsync($"event: {eventName}\n", cancellationToken);
        await Response.WriteAsync($"data: {json}\n\n", cancellationToken);
        await Response.Body.FlushAsync(cancellationToken);
    }

    /// <summary>
    /// Helper method to write SSE comments (heartbeat)
    /// </summary>
    private async Task WriteSseCommentAsync(string comment, CancellationToken cancellationToken)
    {
        await Response.WriteAsync($": {comment}\n\n", cancellationToken);
        await Response.Body.FlushAsync(cancellationToken);
    }
}
