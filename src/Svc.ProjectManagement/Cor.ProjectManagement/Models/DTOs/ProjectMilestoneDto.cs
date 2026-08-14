// Models/DTOs/ProjectMilestoneDto.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Models.DTOs
{
    public class ProjectMilestoneDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public Guid? PhaseId { get; set; }
        public string? PhaseName { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ActualDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string CompletedByName { get; set; } = string.Empty;
        public double CompletionPercentage { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public bool IsOverdue => !IsCompleted && DueDate < DateTime.UtcNow;
    }

    public class ProjectMilestoneCreateDto
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

        public string? CreatedBy { get; set; }
    }

    public class ProjectMilestoneUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ActualDate { get; set; }
        public bool? IsCompleted { get; set;}

        }
        }
