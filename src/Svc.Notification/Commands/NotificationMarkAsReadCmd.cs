using MediatR;

namespace Svc.Notification.Commands;

public class NotificationMarkAsReadCmd : IRequest<Unit>
{
    public Guid Id { get; set; }
}