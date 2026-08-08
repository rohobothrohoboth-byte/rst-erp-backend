using MediatR;
using Svc.Task.Models.Dtos;

namespace Svc.Task.Commands;

public class TaskAddCmd : IRequest<TaskDto>
{
    public TaskAddDto AddDto { get; set; } = null!;
}