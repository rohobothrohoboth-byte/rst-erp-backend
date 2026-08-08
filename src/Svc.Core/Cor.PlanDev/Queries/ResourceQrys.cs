using MediatR;
using Cor.PlanDev.Models.DTOs;

namespace Cor.PlanDev.Queries;

public class GetResourcesByProjectQuery : IRequest<List<ResourceDto>>
{
    public Guid ProjectId { get; set; }
    public string? Status { get; set; }
    public Guid? ResourceUserId { get; set; }
}

public class GetResourceByIdQuery : IRequest<ResourceDto>
{
    public Guid Id { get; set; }
}

public class GetResourceAllocationQuery : IRequest<List<ResourceAllocationDto>>
{
    public Guid ProjectId { get; set; }
}

public class GetResourceUtilizationQuery : IRequest<ResourceUtilizationDto>
{
    public Guid ProjectId { get; set; }
}

