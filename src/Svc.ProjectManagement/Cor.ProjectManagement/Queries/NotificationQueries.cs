// Queries/NotificationQueries.cs
using MediatR;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Queries.NotificationQueries
{
    public class GetNotificationByIdQuery : IRequest<ProjectNotificationDto>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
    }

    public class GetUserNotificationsQuery : IRequest<PaginatedResponse<ProjectNotificationDto>>
    {
        public Guid UserId { get; set; }
        public bool? IsRead { get; set; }
        public NotificationType? Type { get; set; }
        public NotificationPriority? Priority { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GetUnreadCountQuery : IRequest<int>
    {
        public Guid UserId { get; set; }
    }
}