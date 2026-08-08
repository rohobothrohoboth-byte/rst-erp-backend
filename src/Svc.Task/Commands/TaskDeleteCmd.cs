using MediatR;

namespace Svc.Task.Commands;

public class TaskDeleteCmd : IRequest<Unit>
{
    public Guid Id { get; set; }
}