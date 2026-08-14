// Models/DTOs/ProjectTaskDto.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Models.DTOs
{
    public class ProjectTaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public Guid? ParentTaskId { get; set; }
        public string? ParentTaskTitle { get; set; }
        public Guid? PhaseId { get; set; }
        public string? PhaseName { get; set; }
         public Entities.TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public int EstimatedHours { get; set; }
        public int ActualHours { get; set; }
        public int RemainingHours { get; set; }
        public decimal EstimatedCost { get; set; }
        public decimal ActualCost { get; set; }
        public Guid? AssigneeId { get; set; }
        public string AssigneeName { get; set; } = string.Empty;
        public Guid? ReviewerId { get; set; }
        public string ReviewerName { get; set; } = string.Empty;
        public int Order { get; set; }
        public double CompletionPercentage { get; set; }
        public string Tags { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public List<ProjectTaskDto> SubTasks { get; set; } = new List<ProjectTaskDto>();
        public int SubTaskCount { get; set; }
        public int CommentCount { get; set; }
        public bool IsOverdue => DueDate.HasValue && DueDate.Value < DateTime.UtcNow && Status != Cor.ProjectManagement.Models.Entities.TaskStatus.Completed && Status != Cor.ProjectManagement.Models.Entities.TaskStatus.Cancelled;
    }

    public class ProjectTaskCreateDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public Guid ProjectId { get; set; }

        public Guid? ParentTaskId { get; set; }
        public Guid? PhaseId { get; set; }

        [Required]
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

    public class ProjectTaskUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
       public Entities.TaskStatus Status { get; set; }
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

    public class TaskAssignmentDto
    {
        public Guid TaskId { get; set; }
        public Guid AssigneeId { get; set; }
        public string AssigneeName { get; set; } = string.Empty;
        public string? AssignedBy { get; set; }
    }

    public class TaskStatusUpdateDto
    {
        public Guid TaskId { get; set; }
        public Entities.TaskStatus Status { get; set; }
        public string? Notes { get; set; }
        public string? UpdatedBy { get; set; }
    }
    public class TaskFilterDto
        {
            public Guid? ProjectId { get; set; }
            public Guid? AssigneeId { get; set; }
            public Guid? ReviewerId { get; set; }
            public Cor.ProjectManagement.Models.Entities.TaskStatus? Status { get; set; }
            public TaskPriority? Priority { get; set; }
            public DateTime? DueDateFrom { get; set; }
            public DateTime? DueDateTo { get; set; }
            public DateTime? StartDateFrom { get; set; }
            public DateTime? StartDateTo { get; set; }
            public double? MinCompletion { get; set; }
            public double? MaxCompletion { get; set; }
            public string? Search { get; set; }
            public int Page { get; set; } = 1;
            public int PageSize { get; set; } = 20;
            public string? OrderBy { get; set; }
            public bool Descending { get; set; } = false;
        }

         public class TaskCreateDto
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

            public class TaskUpdateDto
            {
                public string? Title { get; set; }
                public string? Description { get; set; }
                public Entities.TaskStatus? Status { get; set; }
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
}