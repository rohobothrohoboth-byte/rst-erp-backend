// Models/Entities/ProjectNotification.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.ProjectManagement.Models.Entities
{
    public enum NotificationType
    {
        ProjectCreated = 1,
        ProjectUpdated = 2,
        ProjectStatusChanged = 3,
        TaskAssigned = 4,
        TaskCompleted = 5,
        TaskOverdue = 6,
        MilestoneDue = 7,
        MilestoneCompleted = 8,
        RiskIdentified = 9,
        RiskEscalated = 10,
        IssueReported = 11,
        IssueResolved = 12,
        ChangeRequested = 13,
        ChangeApproved = 14,
        BudgetAlert = 15,
        TimesheetSubmitted = 16,
        TimesheetApproved = 17,
        DocumentUploaded = 18,
        CommentAdded = 19,
        Mentioned = 20
    }

    public enum NotificationPriority
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Urgent = 4
    }

    public class ProjectNotification : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;

        [Required]
        public NotificationType Type { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public Guid? ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;

        public string? EntityId { get; set; }
        public string EntityType { get; set; } = string.Empty;

        public NotificationPriority Priority { get; set; } = NotificationPriority.Medium;

        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }

        public bool IsDelivered { get; set; }
        public DateTime? DeliveredAt { get; set; }

        public string? DeliveryChannel { get; set; } // Email, Push, SMS, InApp

        public string? ActionUrl { get; set; }

        [Column(TypeName = "jsonb")]
        public string? Metadata { get; set; }

        public DateTime ExpiresAt { get; set; }
    }
}