using MediatR;

namespace Svc.Task.Commands;

public class TaskUpdateStatusCmd : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
}