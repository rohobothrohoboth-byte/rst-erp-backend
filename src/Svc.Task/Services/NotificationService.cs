// Svc.Task/Services/NotificationServiceClient.cs
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Svc.Task.Services;

public class NotificationServiceClient : INotificationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NotificationServiceClient> _logger;
    private readonly string _notificationApiUrl;

    public NotificationServiceClient(HttpClient httpClient, IConfiguration configuration, ILogger<NotificationServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _notificationApiUrl = configuration["NotificationApiUrl"] ?? "https://localhost:7030";
        _httpClient.BaseAddress = new Uri(_notificationApiUrl);
    }

    public async System.Threading.Tasks.Task NotifyTaskAssigned(string userId, string taskTitle, string taskId)
    {
        var notification = new
        {
            UserId = Guid.Parse(userId),
            Title = "📋 New Task Assigned",
            Message = $"You have been assigned a new task: {taskTitle}",
            Type = "task_assigned",
            Priority = "high",
            ModuleName = "Task Management",
            Metadata = JsonSerializer.Serialize(new
            {
                taskId = taskId,
                taskTitle = taskTitle,
                action = "view_task"
            })
        };

        await SendNotification(notification);
    }

    public async System.Threading.Tasks.Task NotifyTaskCompleted(string userId, string taskTitle, string taskId)
    {
        var notification = new
        {
            UserId = Guid.Parse(userId),
            Title = "✅ Task Completed",
            Message = $"Task '{taskTitle}' has been marked as completed! 🎉",
            Type = "task_completed",
            Priority = "high",
            ModuleName = "Task Management",
            Metadata = JsonSerializer.Serialize(new
            {
                taskId = taskId,
                taskTitle = taskTitle,
                action = "view_task"
            })
        };

        await SendNotification(notification);
    }

    public async System.Threading.Tasks.Task NotifyTaskUpdated(string userId, string taskTitle, string taskId)
    {
        var notification = new
        {
            UserId = Guid.Parse(userId),
            Title = "📝 Task Updated",
            Message = $"Task '{taskTitle}' has been updated",
            Type = "task_updated",
            Priority = "medium",
            ModuleName = "Task Management",
            Metadata = JsonSerializer.Serialize(new
            {
                taskId = taskId,
                taskTitle = taskTitle,
                action = "view_task"
            })
        };

        await SendNotification(notification);
    }

    public async System.Threading.Tasks.Task NotifyTaskOverdue(string userId, string taskTitle, string taskId, DateTime dueDate)
    {
        var notification = new
        {
            UserId = Guid.Parse(userId),
            Title = "⚠️ Task Overdue",
            Message = $"Task '{taskTitle}' is overdue. Due date was {dueDate:yyyy-MM-dd}",
            Type = "task_overdue",
            Priority = "urgent",
            ModuleName = "Task Management",
            Metadata = JsonSerializer.Serialize(new
            {
                taskId = taskId,
                taskTitle = taskTitle,
                dueDate = dueDate,
                action = "view_task"
            })
        };

        await SendNotification(notification);
    }

    public async System.Threading.Tasks.Task NotifyTaskDeleted(string userId, string taskTitle, string taskId)
    {
        var notification = new
        {
            UserId = Guid.Parse(userId),
            Title = "🗑️ Task Deleted",
            Message = $"Task '{taskTitle}' has been deleted",
            Type = "task_deleted",
            Priority = "low",
            ModuleName = "Task Management",
            Metadata = JsonSerializer.Serialize(new
            {
                taskId = taskId,
                taskTitle = taskTitle,
                action = "view_tasks"
            })
        };

        await SendNotification(notification);
    }

    private async System.Threading.Tasks.Task SendNotification(object notification)
    {
        try
        {
            var content = new StringContent(
                JsonSerializer.Serialize(notification),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("/api/auth/v1/Notification", content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Notification sent successfully");
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to send notification: {StatusCode} - {Error}", response.StatusCode, error);
            }
        }
        catch (Exception ex)
        {
            // Don't throw - notification failure shouldn't break task operations
            _logger.LogError(ex, "Error sending notification");
        }
    }
}