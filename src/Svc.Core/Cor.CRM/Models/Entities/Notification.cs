// Notification.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities;

public enum NotificationType
{
    Lead = 1,
    Task = 2,
    Activity = 3,
    Opportunity = 4,
    System = 5,
    Alert = 6,
    Reminder = 7
}

public enum NotificationPriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Urgent = 4
}

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Message { get; set; } = string.Empty;

    public NotificationType Type { get; set; }
    public NotificationPriority Priority { get; set; } = NotificationPriority.Medium;

    public bool IsRead { get; set; } = false;
    public DateTime? ReadDate { get; set; }

    public Guid? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }

    public string? ActionUrl { get; set; }
    public string? ActionText { get; set; }

    public bool IsDelivered { get; set; } = false;
    public DateTime? DeliveredDate { get; set; }

    [MaxLength(100)]
    public string? Channel { get; set; } // Email, SMS, Push, InApp

    public DateTime? ExpirationDate { get; set; }

    // Navigation Properties
    //[ForeignKey("UserId")]
    //public virtual User User { get; set; } = null!;
}