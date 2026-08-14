// Commands/RiskCommands.cs
using MediatR;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Commands.RiskCommands
{
    public class CreateRiskCommand : IRequest<ProjectRiskDto>
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
        public RiskImpact Impact { get; set; }
        public RiskProbability Probability { get; set; }
        public string MitigationStrategy { get; set; } = string.Empty;
        public string ContingencyPlan { get; set; } = string.Empty;
        public Guid? AssignedToId { get; set; }
        public string? AssignedToName { get; set; }
        public string? IdentifiedByName { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class UpdateRiskCommand : IRequest<ProjectRiskDto>
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public RiskImpact? Impact { get; set; }
        public RiskProbability? Probability { get; set; }
        public string? MitigationStrategy { get; set; }
        public string? ContingencyPlan { get; set; }
        public RiskStatus? Status { get; set; }
        public Guid? AssignedToId { get; set; }
        public string? AssignedToName { get; set; }
        public DateTime? ReviewDate { get; set; }
        public string? ResolutionNotes { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class DeleteRiskCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string? DeletedBy { get; set; }
    }

    public class ResolveRiskCommand : IRequest<ProjectRiskDto>
    {
        public Guid Id { get; set; }
        public string ResolutionNotes { get; set; } = string.Empty;
        public string? ResolvedBy { get; set; }
    }

    public class UpdateRiskStatusCommand : IRequest<ProjectRiskDto>
    {
        public Guid Id { get; set; }
        public RiskStatus Status { get; set; }
        public string? Notes { get; set; }
        public string? UpdatedBy { get; set; }
    }
}