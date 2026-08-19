// Queries/ResourceQueries/GetResourceAllocationsQuery.cs
using MediatR;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Queries.ResourceQueries
{
    public class GetResourceAllocationsQuery : IRequest<PaginatedResponse<ProjectResourceDto>>
    {
        public Guid? ProjectId { get; set; }
        public Guid? ResourceId { get; set; }
        public ResourceType? Type { get; set; }
        public ResourceAllocationStatus? Status { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GetResourcesByProjectQuery : IRequest<List<ProjectResourceDto>>
    {
        public Guid ProjectId { get; set; }
        public ResourceAllocationStatus? Status { get; set; }
    }

    public class GetAvailableResourcesQuery : IRequest<List<ResourceAvailabilityDto>>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ResourceType? Type { get; set; }
        public string? Skills { get; set; }
    }
}