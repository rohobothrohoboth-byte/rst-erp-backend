using MediatR;

namespace Svc.Notification.Commands;

public class NotificationMarkAllAsReadCmd : IRequest<Unit>
{
    public Guid UserId { get; set; }
}