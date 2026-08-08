using System;

namespace Svc.Notification.Models.Entities;

public class NotificationEntity  // Renamed from Notification to NotificationEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "info";
    public string Priority { get; set; } = "medium";
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public string? Metadata { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public bool IsDeleted { get; set; } = false;
}