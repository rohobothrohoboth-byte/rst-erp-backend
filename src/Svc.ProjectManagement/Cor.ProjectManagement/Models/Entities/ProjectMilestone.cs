// Models/Entities/ProjectMilestone.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.ProjectManagement.Models.Entities
{
    public class ProjectMilestone : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public Guid ProjectId { get; set; }

        public Guid? PhaseId { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public DateTime? ActualDate { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime? CompletedAt { get; set; }

        public Guid? CompletedById { get; set; }
        public string CompletedByName { get; set; } = string.Empty;

        public double CompletionPercentage { get; set; }

        [Column(TypeName = "jsonb")]
        public string? Metadata { get; set; }

        // Navigation Properties
        public virtual Project Project { get; set; } = null!;
        public virtual ProjectPhase? Phase { get; set; }
    }
}