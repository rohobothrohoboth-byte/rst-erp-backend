// Svc.Task/Services/INotificationService.cs
using System;
// Remove: using System.Threading.Tasks;

namespace Svc.Task.Services;

public interface INotificationService
{
    System.Threading.Tasks.Task NotifyTaskAssigned(string userId, string taskTitle, string taskId);
    System.Threading.Tasks.Task NotifyTaskCompleted(string userId, string taskTitle, string taskId);
    System.Threading.Tasks.Task NotifyTaskUpdated(string userId, string taskTitle, string taskId);
    System.Threading.Tasks.Task NotifyTaskOverdue(string userId, string taskTitle, string taskId, DateTime dueDate);
    System.Threading.Tasks.Task NotifyTaskDeleted(string userId, string taskTitle, string taskId);
}