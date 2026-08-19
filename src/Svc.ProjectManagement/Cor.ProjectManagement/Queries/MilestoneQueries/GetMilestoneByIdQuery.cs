// Queries/MilestoneQueries/GetMilestoneByIdQuery.cs
using MediatR;
using Cor.ProjectManagement.Models.DTOs;

namespace Cor.ProjectManagement.Queries.MilestoneQueries
{
    public class GetMilestoneByIdQuery : IRequest<ProjectMilestoneDto>
    {
        public Guid Id { get; set; }
    }

    public class GetMilestonesByProjectQuery : IRequest<PaginatedResponse<ProjectMilestoneDto>>
    {
        public Guid ProjectId { get; set; }
        public bool? IsCompleted { get; set; }
        public DateTime? DueDateFrom { get; set; }
        public DateTime? DueDateTo { get; set; }
        public Guid? PhaseId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GetUpcomingMilestonesQuery : IRequest<List<ProjectMilestoneDto>>
    {
        public Guid ProjectId { get; set; }
        public int DaysThreshold { get; set; } = 30;
    }
}