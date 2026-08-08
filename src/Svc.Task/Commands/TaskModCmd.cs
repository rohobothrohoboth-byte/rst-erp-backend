using MediatR;
using Svc.Task.Models.Dtos;

namespace Svc.Task.Commands;

public class TaskModCmd : IRequest<TaskDto>
{
    public TaskModDto ModDto { get; set; } = null!;
}