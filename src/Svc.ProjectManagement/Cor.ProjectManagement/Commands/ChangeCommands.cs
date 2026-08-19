// Commands/ChangeCommands.cs
using MediatR;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Commands.ChangeCommands
{
    public class CreateChangeCommand : IRequest<ProjectChangeDto>
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
        public ChangeType Type { get; set; }
        public ChangePriority Priority { get; set; }
        public string CurrentState { get; set; } = string.Empty;
        public string ProposedState { get; set; } = string.Empty;
        public string Justification { get; set; } = string.Empty;
        public string ImpactAnalysis { get; set; } = string.Empty;
        public decimal CostImpact { get; set; }
        public int ScheduleImpact { get; set; }
        public string? RequestedByName { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class UpdateChangeCommand : IRequest<ProjectChangeDto>
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public ChangeType? Type { get; set; }
        public ChangePriority? Priority { get; set; }
        public ChangeStatus? Status { get; set; }
        public string? CurrentState { get; set; }
        public string? ProposedState { get; set; }
        public string? Justification { get; set; }
        public string? ImpactAnalysis { get; set; }
        public decimal? CostImpact { get; set; }
        public int? ScheduleImpact { get; set; }
        public string? ReviewNotes { get; set; }
        public string? ApprovalNotes { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class DeleteChangeCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string? DeletedBy { get; set; }
    }

    public class ApproveChangeCommand : IRequest<ProjectChangeDto>
    {
        public Guid Id { get; set; }
        public string? ApprovedBy { get; set; }
        public string? Notes { get; set; }
    }

    public class RejectChangeCommand : IRequest<ProjectChangeDto>
    {
        public Guid Id { get; set; }
        public string? RejectedBy { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class ImplementChangeCommand : IRequest<ProjectChangeDto>
    {
        public Guid Id { get; set; }
        public string? ImplementedBy { get; set; }
        public string? Notes { get; set; }
    }
}