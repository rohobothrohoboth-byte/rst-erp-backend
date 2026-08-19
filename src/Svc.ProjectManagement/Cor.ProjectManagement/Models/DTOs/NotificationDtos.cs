// Models/DTOs/NotificationDtos.cs
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Models.DTOs
{
    public class CreateNotificationDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public Guid? ProjectId { get; set; }
        public string? EntityId { get; set; }
        public string EntityType { get; set; } = string.Empty;
        public NotificationPriority Priority { get; set; } = NotificationPriority.Medium;
        public string? ActionUrl { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class MarkNotificationReadDto
    {
        public List<Guid> NotificationIds { get; set; } = new();
        public bool MarkAllAsRead { get; set; }
        public Guid UserId { get; set; }
    }

    public class NotificationFilterDto
    {
        public bool? IsRead { get; set; }
        public NotificationType? Type { get; set; }
        public NotificationPriority? Priority { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}