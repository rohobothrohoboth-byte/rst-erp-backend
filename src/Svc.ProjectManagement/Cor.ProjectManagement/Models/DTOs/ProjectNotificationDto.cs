// Models/DTOs/ProjectNotificationDto.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Models.DTOs
{
    public class ProjectNotificationDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public Guid? ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string? EntityId { get; set; }
        public string EntityType { get; set; } = string.Empty;
        public NotificationPriority Priority { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public bool IsDelivered { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public string? DeliveryChannel { get; set; }
        public string? ActionUrl { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class NotificationMarkReadDto
    {
        public List<Guid> NotificationIds { get; set; } = new List<Guid>();
        public bool MarkAllAsRead { get; set; }
    }

    public class NotificationPreferenceDto
    {
        public Guid UserId { get; set; }
        public Dictionary<NotificationType, bool> EnabledTypes { get; set; } = new Dictionary<NotificationType, bool>();
        public bool EmailEnabled { get; set; }
        public bool PushEnabled { get; set; }
        public bool InAppEnabled { get; set; }
        public bool SMSEnabled { get; set; }
    }
}