// Models/Entities/ProjectTask.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Cor.ProjectManagement.Models.Entities
{
    public enum TaskStatus
    {
        NotStarted = 1,
        InProgress = 2,
        Blocked = 3,
        UnderReview = 4,
        Completed = 5,
        Cancelled = 6
    }

    public enum TaskPriority
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Urgent = 4,
        Critical = 5
    }

    public class ProjectTask : BaseEntity
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
        public TaskStatus Status { get; set; } = TaskStatus.NotStarted;

        [Required]
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

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

        [Column(TypeName = "jsonb")]
        public string? CustomFields { get; set; }

        // Navigation Properties
        public virtual Project Project { get; set; } = null!;
        public virtual ProjectTask? ParentTask { get; set; }
         public virtual ProjectPhase? Phase { get; set; }
        public virtual ICollection<ProjectTask> SubTasks { get; set; } = new List<ProjectTask>();
        public virtual ICollection<ProjectComment> Comments { get; set; } = new List<ProjectComment>();
    }
}