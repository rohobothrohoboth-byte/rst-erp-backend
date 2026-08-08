using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Svc.Notification.Models.Entities;

namespace Svc.Notification.Services;

public interface INotificationService
{
    Task<NotificationEntity> CreateNotificationAsync(
        Guid userId,
        string title,
        string message,
        string type,
        string priority = "medium",
        Dictionary<string, object>? metadata = null);

    // Task Notifications - All using string parameters
    Task NotifyTaskAssigned(string userId, string taskTitle, string taskId);
    Task NotifyTaskCompleted(string userId, string taskTitle, string taskId);
    Task NotifyTaskOverdue(string userId, string taskTitle, string taskId);
    Task NotifyTaskUpdated(string userId, string taskTitle, string taskId);

    // Leave Notifications
    Task NotifyLeaveRequested(string userId, string leaveType, DateTime startDate, DateTime endDate);
    Task NotifyLeaveApproved(string userId, string leaveType, DateTime startDate, DateTime endDate);
    Task NotifyLeaveRejected(string userId, string leaveType, string reason);

    // HR Notifications
    Task NotifyEmployeeAdded(string userId, string employeeName);
    Task NotifyEmployeeBirthday(string userId, string employeeName);
    Task NotifyDocumentExpiring(string userId, string documentName, DateTime expiryDate);

    // Training Notifications
    Task NotifyTrainingAssigned(string userId, string trainingName, DateTime startDate);
    Task NotifyTrainingReminder(string userId, string trainingName, DateTime startDate);

    // Onboarding Notifications
    Task NotifyOnboardingTask(string userId, string taskName, DateTime dueDate);

    // Finance Notifications
    Task NotifyBudgetApproval(string userId, string budgetName, decimal amount);
    Task NotifyPaymentProcessed(string userId, string invoiceNumber, decimal amount);

    // System Notifications
    Task NotifySystemAlert(string userId, string alertMessage, string priority = "high");
    Task NotifyWelcome(string userId, string employeeName);
}