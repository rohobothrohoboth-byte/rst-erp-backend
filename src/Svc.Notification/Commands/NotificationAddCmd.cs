using MediatR;
using Svc.Notification.Models.Dtos;

namespace Svc.Notification.Commands;

public class NotificationAddCmd : IRequest<NotificationDto>
{
    public NotificationAddDto AddDto { get; set; } = null!;
}