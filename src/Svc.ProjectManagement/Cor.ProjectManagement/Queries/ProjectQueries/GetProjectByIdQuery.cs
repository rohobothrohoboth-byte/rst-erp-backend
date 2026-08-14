// Queries/ProjectQueries/GetProjectByIdQuery.cs
using MediatR;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Models.Entities;
namespace Cor.ProjectManagement.Queries.ProjectQueries
{
    public class GetProjectByIdQuery : IRequest<ProjectDto>
    {
        public Guid Id { get; set; }
    }

    public class GetProjectsQuery : IRequest<PaginatedResponse<ProjectDto>>
    {
        public string? Search { get; set; }
        public ProjectStatus? Status { get; set; }
        public ProjectType? Type { get; set; }
        public Guid? ProjectManagerId { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public DateTime? EndDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }
        public Guid? CustomerId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? OrderBy { get; set; }
        public bool Descending { get; set; } = false;
    }

    public class GetProjectDashboardQuery : IRequest<ProjectDashboardDto>
    {
        public Guid? UserId { get; set; }
    }

    public class GetProjectStatisticsQuery : IRequest<ProjectStatisticsDto>
    {
        public Guid ProjectId { get; set; }
    }
}