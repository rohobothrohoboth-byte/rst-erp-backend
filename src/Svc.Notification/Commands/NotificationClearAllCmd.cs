using MediatR;

namespace Svc.Notification.Commands;

public class NotificationClearAllCmd : IRequest<Unit>
{
    public Guid UserId { get; set; }
}