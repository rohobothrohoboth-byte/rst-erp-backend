// Queries/RiskQueries.cs
using MediatR;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Queries.RiskQueries
{
    public class GetRiskByIdQuery : IRequest<ProjectRiskDto>
    {
        public Guid Id { get; set; }
    }

    public class GetRisksByProjectQuery : IRequest<List<ProjectRiskDto>>
    {
        public Guid ProjectId { get; set; }
        public RiskStatus? Status { get; set; }
        public RiskSeverity? Severity { get; set; }
        public Guid? AssignedToId { get; set; }
    }

    public class GetRiskHeatmapQuery : IRequest<RiskHeatmapDto>
    {
        public Guid ProjectId { get; set; }
    }

    public class GetRiskSummaryQuery : IRequest<RiskSummaryDto>
    {
        public Guid ProjectId { get; set; }
    }
}