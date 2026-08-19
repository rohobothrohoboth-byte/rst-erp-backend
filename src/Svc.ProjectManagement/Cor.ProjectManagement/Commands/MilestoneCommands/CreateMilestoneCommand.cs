// Commands/MilestoneCommands/CreateMilestoneCommand.cs
using MediatR;
using Cor.ProjectManagement.Models.DTOs;

namespace Cor.ProjectManagement.Commands.MilestoneCommands
{
    public class CreateMilestoneCommand : IRequest<ProjectMilestoneDto>
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
        public Guid? PhaseId { get; set; }
        public DateTime DueDate { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class UpdateMilestoneCommand : IRequest<ProjectMilestoneDto>
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ActualDate { get; set; }
        public bool? IsCompleted { get; set; }
        public double? CompletionPercentage { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class DeleteMilestoneCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string? DeletedBy { get; set; }
    }

    public class CompleteMilestoneCommand : IRequest<ProjectMilestoneDto>
    {
        public Guid Id { get; set; }
        public string? CompletedBy { get; set; }
        public string? Notes { get; set; }
    }
}