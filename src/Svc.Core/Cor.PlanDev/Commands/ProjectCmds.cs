using MediatR;
using Cor.PlanDev.Models.DTOs;

namespace Cor.PlanDev.Commands;

public class CreateProjectCommand : IRequest<ProjectDto>
{
    public CreateProjectDto CreateDto { get; set; } = new();
}

public class UpdateProjectCommand : IRequest<ProjectDto>
{
    public UpdateProjectDto UpdateDto { get; set; } = new();
}

public class DeleteProjectCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class UpdateProjectProgressCommand : IRequest<ProjectDto>
{
    public UpdateProjectProgressDto ProgressDto { get; set; } = new();
}

public class CompleteProjectCommand : IRequest<ProjectDto>
{
    public Guid Id { get; set; }
}