// Svc.Notification/Services/NotificationSender.cs
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Svc.Notification.Hubs;
using Svc.Notification.Models.Dtos;
using System.Text.Json;

namespace Svc.Notification.Services;

public interface INotificationSender
{
    Task SendNotificationAsync(Guid userId, NotificationDto notification);
    Task BroadcastNotificationAsync(string message);
}

public class NotificationSender : INotificationSender
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly INotificationQueueService _queue;
    private readonly ILogger<NotificationSender> _logger;

    public NotificationSender(
        IHubContext<NotificationHub> hubContext,
        INotificationQueueService queue,
        ILogger<NotificationSender> logger)
    {
        _hubContext = hubContext;
        _queue = queue;
        _logger = logger;
    }

    public async Task SendNotificationAsync(Guid userId, NotificationDto notification)
    {
        try
        {
            // Convert string Metadata to Dictionary if needed
            Dictionary<string, object>? metadataDict = null;
            if (!string.IsNullOrEmpty(notification.Metadata))
            {
                try
                {
                    metadataDict = JsonSerializer.Deserialize<Dictionary<string, object>>(notification.Metadata);
                }
                catch
                {
                    // If deserialization fails, create a simple dictionary
                    metadataDict = new Dictionary<string, object> { { "RawData", notification.Metadata } };
                }
            }

            // Queue for database persistence
            await _queue.QueueNotificationAsync(new NotificationMessage
            {
                UserId = userId,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type,
                Priority = notification.Priority,
                ModuleName = notification.ModuleName,
                ReferenceId = notification.ReferenceId,
                Metadata = metadataDict
            });

            // Send real-time via SignalR
            await _hubContext.Clients.User(userId.ToString())
                .SendAsync("ReceiveNotification", notification);

            _logger.LogInformation("Notification sent to user {UserId}: {Title}", userId, notification.Title);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to user {UserId}", userId);
        }
    }

    public async Task BroadcastNotificationAsync(string message)
    {
        await _hubContext.Clients.All.SendAsync("BroadcastMessage", message);
    }
}