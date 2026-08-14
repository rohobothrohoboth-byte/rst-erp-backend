// Commands/NotificationCommands.cs
using MediatR;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Commands.NotificationCommands
{
    public class CreateNotificationCommand : IRequest<ProjectNotificationDto>
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

    public class MarkNotificationReadCommand : IRequest<bool>
    {
        public List<Guid> NotificationIds { get; set; } = new List<Guid>();
        public bool MarkAllAsRead { get; set; }
        public Guid UserId { get; set; }
    }

    public class DeleteNotificationCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
    }

    public class DeleteAllNotificationsCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
    }
}