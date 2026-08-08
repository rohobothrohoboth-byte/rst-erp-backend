using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Svc.Notification.Models.Dtos;
using Svc.Notification.Models.Entities;
using Svc.Notification.Commands;

namespace Svc.Notification.Services;

public class NotificationService : INotificationService
{
    private readonly IMediator _mediator;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(IMediator mediator, ILogger<NotificationService> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<NotificationEntity> CreateNotificationAsync(
        Guid userId,
        string title,
        string message,
        string type,
        string priority = "medium",
        Dictionary<string, object>? metadata = null)
    {
        var notification = new NotificationEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            Priority = priority,
            IsRead = false,
            CreatedAt = DateTime.UtcNow,
            Metadata = metadata != null ? JsonSerializer.Serialize(metadata) : null,
            DateAdd = DateTime.UtcNow
        };

        var dto = new NotificationAddDto
        {
            UserId = notification.UserId,
            Title = notification.Title,
            Message = notification.Message,
            Type = notification.Type,
            Priority = notification.Priority,
            Metadata = notification.Metadata
        };

        await _mediator.Send(new NotificationAddCmd { AddDto = dto });

        return notification;
    }

    // Task Notifications - Using string parameters (matching the interface)
    public async Task NotifyTaskAssigned(string userId, string taskTitle, string taskId)
    {
        await CreateNotificationAsync(
            Guid.Parse(userId),
            "📋 New Task Assigned",
            $"You have been assigned a new task: \"{taskTitle}\"",
            "task",
            "medium",
            new Dictionary<string, object>
            {
                ["taskId"] = taskId,
                ["action"] = "view_task",
                ["module"] = "Task"
            });
    }

    public async Task NotifyTaskCompleted(string userId, string taskTitle, string taskId)
    {
        await CreateNotificationAsync(
            Guid.Parse(userId),
            "✅ Task Completed",
            $"Task \"{taskTitle}\" has been marked as completed",
            "task",
            "low",
            new Dictionary<string, object>
            {
                ["taskId"] = taskId,
                ["action"] = "view_task",
                ["module"] = "Task"
            });
    }

    public async Task NotifyTaskOverdue(string userId, string taskTitle, string taskId)
    {
        await CreateNotificationAsync(
            Guid.Parse(userId),
            "⚠️ Task Overdue",
            $"Task \"{taskTitle}\" is overdue. Please take action.",
            "task",
            "urgent",
            new Dictionary<string, object>
            {
                ["taskId"] = taskId,
                ["action"] = "view_task",
                ["module"] = "Task"
            });
    }

    public async Task NotifyTaskUpdated(string userId, string taskTitle, string taskId)
    {
        await CreateNotificationAsync(
            Guid.Parse(userId),
            "✏️ Task Updated",
            $"Task \"{taskTitle}\" has been updated",
            "task",
            "low",
            new Dictionary<string, object>
            {
                ["taskId"] = taskId,
                ["action"] = "view_task",
                ["module"] = "Task"
            });
    }

    // Leave Notifications
    public async Task NotifyLeaveRequested(string userId, string leaveType, DateTime startDate, DateTime endDate)
    {
        await CreateNotificationAsync(
            Guid.Parse(userId),
            "📅 Leave Request Submitted",
            $"Your {leaveType} leave request from {startDate:MMM dd} to {endDate:MMM dd} has been submitted",
            "leave",
            "medium",
            new Dictionary<string, object>
            {
                ["startDate"] = startDate.ToString(),
                ["endDate"] = endDate.ToString(),
                ["action"] = "view_leave",
                ["module"] = "HRM"
            });
    }

    public async Task NotifyLeaveApproved(string userId, string leaveType, DateTime startDate, DateTime endDate)
    {
        await CreateNotificationAsync(
            Guid.Parse(userId),
            "✅ Leave Request Approved",
            $"Your {leaveType} leave request from {startDate:MMM dd} to {endDate:MMM dd} has been approved",
            "leave",
            "low",
            new Dictionary<string, object>
            {
                ["startDate"] = startDate.ToString(),
                ["endDate"] = endDate.ToString(),
                ["action"] = "view_leave",
                ["module"] = "HRM"
            });
    }

    public async Task NotifyLeaveRejected(string userId, string leaveType, string reason)
    {
        await CreateNotificationAsync(
            Guid.Parse(userId),
            "❌ Leave Request Rejected",
            $"Your {leaveType} leave request has been rejected. Reason: {reason}",
            "leave",
            "high",
            new Dictionary<string, object>
            {
                ["reason"] = reason,
                ["action"] = "view_leave",
                ["module"] = "HRM"
            });
    }

    // HR Notifications
    public async Task NotifyEmployeeAdded(string userId, string employeeName)
    {
        await CreateNotificationAsync(
            Guid.Parse(userId),
            "👤 New Employee Joined",
            $"{employeeName} has joined the company",
            "hr",
            "low",
            new Dictionary<string, object>
            {
                ["employeeName"] = employeeName,
                ["action"] = "view_employee",
                ["module"] = "HRM"
            });
    }

    public async Task NotifyEmployeeBirthday(string userId, string employeeName)
    {
        await CreateNotificationAsync(
            Guid.Parse(userId),
            "🎂 Happy Birthday!",
            $"Today is {employeeName}'s birthday!",
            "hr",
            "low",
            new Dictionary<string, object>
            {
                ["employeeName"] = employeeName,
                ["action"] = "view_employee",
                ["module"] = "HRM"
            });
    }

    public async Task NotifyDocumentExpiring(string userId, string documentName, DateTime expiryDate)
    {
        await CreateNotificationAsync(
            Guid.Parse(userId),
            "📄 Document Expiring Soon",
            $"Your {documentName} will expire on {expiryDate:MMM dd, yyyy}",
            "hr",
            "high",
            new Dictionary<string, object>
            {
                ["documentName"] = documentName,
                ["expiryDate"] = expiryDate.ToString(),
                ["action"] = "view_document",
                ["module"] = "HRM"
            });
    }

    // Training Notifications
    public async Task NotifyTrainingAssigned(string userId, string trainingName, DateTime startDate)
    {
        await CreateNotificationAsync(
            Guid.Parse(userId),
            "📚 Training Assigned",
            $"You have been assigned to training: \"{trainingName}\" starting {startDate:MMM dd, yyyy}",
            "training",
            "medium",
            new Dictionary<string, object>
            {
                ["trainingName"] = trainingName,
                ["startDate"] = startDate.ToString(),
                ["action"] = "view_training",
                ["module"] = "HRM"
            });
    }

    public async Task NotifyTrainingReminder(string userId, string trainingName, DateTime startDate)
    {
        await CreateNotificationAsync(
            Guid.Parse(userId),
            "⏰ Training Reminder",
            $"Reminder: Training \"{trainingName}\" starts tomorrow at {startDate:MMM dd, yyyy}",
            "training",
            "medium",
            new Dictionary<string, object>
            {
                ["trainingName"] = trainingName,
                ["startDate"] = startDate.ToString(),
                ["action"] = "view_training",
                ["module"] = "HRM"
            });
    }

    // Onboarding Notifications
    public async Task NotifyOnboardingTask(string userId, string taskName, DateTime dueDate)
    {
        await CreateNotificationAsync(
            Guid.Parse(userId),
            "📋 Onboarding Task",
            $"Onboarding task \"{taskName}\" is due on {dueDate:MMM dd, yyyy}",
            "onboarding",
            "medium",
            new Dictionary<string, object>
            {
                ["taskName"] = taskName,
                ["dueDate"] = dueDate.ToString(),
                ["action"] = "view_onboarding",
                ["module"] = "HRM"
            });
    }

    // Finance Notifications
    public async Task NotifyBudgetApproval(string userId, string budgetName, decimal amount)
    {
        await CreateNotificationAsync(
            Guid.Parse(userId),
            "💰 Budget Approval Required",
            $"Budget \"{budgetName}\" (${amount:N2}) requires your approval",
            "finance",
            "high",
            new Dictionary<string, object>
            {
                ["budgetName"] = budgetName,
                ["amount"] = amount.ToString(),
                ["action"] = "approve_budget",
                ["module"] = "Finance"
            });
    }

    public async Task NotifyPaymentProcessed(string userId, string invoiceNumber, decimal amount)
    {
        await CreateNotificationAsync(
            Guid.Parse(userId),
            "💵 Payment Processed",
            $"Payment of ${amount:N2} for invoice {invoiceNumber} has been processed",
            "finance",
            "low",
            new Dictionary<string, object>
            {
                ["invoiceNumber"] = invoiceNumber,
                ["amount"] = amount.ToString(),
                ["action"] = "view_invoice",
                ["module"] = "Finance"
            });
    }

    // System Notifications
    public async Task NotifySystemAlert(string userId, string alertMessage, string priority = "high")
    {
        await CreateNotificationAsync(
            Guid.Parse(userId),
            "⚠️ System Alert",
            alertMessage,
            "system",
            priority,
            new Dictionary<string, object>
            {
                ["alert"] = alertMessage,
                ["action"] = "view_system",
                ["module"] = "System"
            });
    }

    public async Task NotifyWelcome(string userId, string employeeName)
    {
        await CreateNotificationAsync(
            Guid.Parse(userId),
            "🎉 Welcome to RST ERP!",
            $"Welcome aboard, {employeeName}! We're excited to have you.",
            "system",
            "low",
            new Dictionary<string, object>
            {
                ["employeeName"] = employeeName,
                ["action"] = "welcome",
                ["module"] = "System"
            });
    }
}