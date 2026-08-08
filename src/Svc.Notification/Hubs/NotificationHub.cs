// Svc.Notification/Hubs/NotificationHub.cs
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Svc.Notification.Models.Dtos;
using Svc.Notification.Services;

namespace Svc.Notification.Hubs;

public class NotificationHub : Hub
{
    private readonly INotificationQueueService _queue;
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(INotificationQueueService queue, ILogger<NotificationHub> logger)
    {
        _queue = queue;
        _logger = logger;
    }

    public async Task SendNotification(NotificationDto notification)
    {
        await Clients.All.SendAsync("ReceiveNotification", notification);
    }

    public async Task MarkAsRead(Guid notificationId)
    {
        await Clients.Caller.SendAsync("NotificationRead", notificationId);
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}