using MediatR;

namespace Svc.Notification.Commands;

public class NotificationDeleteCmd : IRequest<Unit>
{
    public Guid Id { get; set; }
}