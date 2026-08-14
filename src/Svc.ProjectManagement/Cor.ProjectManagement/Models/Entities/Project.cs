// Models/Entities/Project.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.ProjectManagement.Models.Entities
{
    public enum ProjectStatus
    {
        Draft = 1,
        Planning = 2,
        InProgress = 3,
        OnHold = 4,
        Completed = 5,
        Cancelled = 6,
        Archived = 7
    }

    public enum ProjectType
    {
        Internal = 1,
        External = 2,
        Research = 3,
        Development = 4,
        Maintenance = 5,
        Consulting = 6
    }

    public class Project : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Code { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public ProjectStatus Status { get; set; } = ProjectStatus.Draft;

        [Required]
        public ProjectType Type { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }

        public Guid? ProjectManagerId { get; set; }
        public string ProjectManagerName { get; set; } = string.Empty;

        public Guid? DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;

        public decimal Budget { get; set; }
        public decimal ActualCost { get; set; }
        public decimal TotalBilled { get; set; }

        public int Priority { get; set; } = 1;

        public string? CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;

        public string? VendorId { get; set; }
        public string VendorName { get; set; } = string.Empty;

        // JSON fields for flexible data
        [Column(TypeName = "jsonb")]
        public string? CustomFields { get; set; }

        [Column(TypeName = "jsonb")]
        public string? Metadata { get; set; }

        public double CompletionPercentage { get; set; }

        public string Tags { get; set; } = string.Empty;

        // Navigation Properties
        public virtual ICollection<ProjectPhase> Phases { get; set; } = new List<ProjectPhase>();
        public virtual ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
        public virtual ICollection<ProjectMilestone> Milestones { get; set; } = new List<ProjectMilestone>();
        public virtual ICollection<ProjectResource> Resources { get; set; } = new List<ProjectResource>();
        public virtual ICollection<ProjectBudget> Budgets { get; set; } = new List<ProjectBudget>();
        public virtual ICollection<ProjectRisk> Risks { get; set; } = new List<ProjectRisk>();
        public virtual ICollection<ProjectIssue> Issues { get; set; } = new List<ProjectIssue>();
        public virtual ICollection<ProjectChange> Changes { get; set; } = new List<ProjectChange>();
        public virtual ICollection<ProjectDocument> Documents { get; set; } = new List<ProjectDocument>();
        public virtual ICollection<ProjectComment> Comments { get; set; } = new List<ProjectComment>();
    }
}