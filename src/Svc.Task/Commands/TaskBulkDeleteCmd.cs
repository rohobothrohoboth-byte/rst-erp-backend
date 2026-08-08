using MediatR;

namespace Svc.Task.Commands;

public class TaskBulkDeleteCmd : IRequest<Unit>
{
    public List<Guid> Ids { get; set; } = new();
}