using MediatR;
using Cor.PlanDev.Models.DTOs;

namespace Cor.PlanDev.Queries;

public class GetAllProjectsQuery : IRequest<List<ProjectDto>>
{
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public Guid? ManagerId { get; set; }
    public string? Department { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class GetProjectByIdQuery : IRequest<ProjectDto>
{
    public Guid Id { get; set; }
}

public class GetProjectByCodeQuery : IRequest<ProjectDto>
{
    public string Code { get; set; } = string.Empty;
}

public class SearchProjectsQuery : IRequest<List<ProjectDto>>
{
    public string? SearchTerm { get; set; }
    public string? Status { get; set; }
}

