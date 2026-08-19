// Commands/TaskCommands/CreateTaskCommand.cs
using MediatR;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Commands.TaskCommands
{
    public class CreateTaskCommand : IRequest<ProjectTaskDto>
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
        public Guid? ParentTaskId { get; set; }
        public Guid? PhaseId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public int EstimatedHours { get; set; }
        public decimal EstimatedCost { get; set; }
        public Guid? AssigneeId { get; set; }
        public string? AssigneeName { get; set; }
        public Guid? ReviewerId { get; set; }
        public string? ReviewerName { get; set; }
        public string? Tags { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class UpdateTaskCommand : IRequest<ProjectTaskDto>
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
         public Cor.ProjectManagement.Models.Entities.TaskStatus ?Status { get; set; }
        public TaskPriority? Priority { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public int? EstimatedHours { get; set; }
        public int? ActualHours { get; set; }
        public int? RemainingHours { get; set; }
        public decimal? EstimatedCost { get; set; }
        public decimal? ActualCost { get; set; }
        public Guid? AssigneeId { get; set; }
        public string? AssigneeName { get; set; }
        public Guid? ReviewerId { get; set; }
        public string? ReviewerName { get; set; }
        public double? CompletionPercentage { get; set; }
        public string? Tags { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class DeleteTaskCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string? DeletedBy { get; set; }
    }

    public class AssignTaskCommand : IRequest<ProjectTaskDto>
    {
        public Guid TaskId { get; set; }
        public Guid AssigneeId { get; set; }
        public string AssigneeName { get; set; } = string.Empty;
        public string? AssignedBy { get; set; }
    }

    public class UpdateTaskStatusCommand : IRequest<ProjectTaskDto>
    {
        public Guid TaskId { get; set; }
        public Cor.ProjectManagement.Models.Entities.TaskStatus? Status { get; set; }
        public string? Notes { get; set; }
        public string? UpdatedBy { get; set; }
    }
}