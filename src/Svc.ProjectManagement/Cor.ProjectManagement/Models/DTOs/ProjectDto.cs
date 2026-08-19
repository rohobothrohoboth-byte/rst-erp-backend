// Models/DTOs/ProjectDto.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Models.DTOs
{
    public class ProjectDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ProjectStatus Status { get; set; }
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
        public int Priority { get; set; }
        public string? CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? VendorId { get; set; }
        public string VendorName { get; set; } = string.Empty;
        public double CompletionPercentage { get; set; }
        public string Tags { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public int TaskCount { get; set; }
        public int MilestoneCount { get; set; }
        public int ResourceCount { get; set; }
        public List<ProjectPhaseDto>? Phases { get; set; }
        public List<ProjectTaskDto>? Tasks { get; set; }
        public List<ProjectMilestoneDto>? Milestones { get; set; }
        public List<ProjectResourceDto>? Resources { get; set; }
        public List<ProjectBudgetDto>? Budgets { get; set; }
        public List<ProjectRiskDto>? Risks { get; set; }
        public List<ProjectIssueDto>? Issues { get; set; }
    }

    public class ProjectCreateDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public ProjectType Type { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public decimal Budget { get; set; }

        public Guid? ProjectManagerId { get; set; }
        public string? ProjectManagerName { get; set; }

        public Guid? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }

        public int Priority { get; set; } = 1;

        public Guid? CustomerId { get; set; }
        public string? CustomerName { get; set; }

        public string? Tags { get; set; }

        public string? CreatedBy { get; set; }
    }

    public class ProjectUpdateDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public ProjectStatus? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public decimal? Budget { get; set; }
        public decimal? ActualCost { get; set; }
        public decimal? TotalBilled { get; set; }
        public Guid? ProjectManagerId { get; set; }
        public string? ProjectManagerName { get; set; }
        public int? Priority { get; set; }
        public double? CompletionPercentage { get; set; }
        public string? Tags { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class ProjectFilterDto
    {
        public string? Search { get; set; }
        public ProjectStatus? Status { get; set; }
        public ProjectType? Type { get; set; }
        public Guid? ProjectManagerId { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public DateTime? EndDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }
        public Guid? CustomerId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? OrderBy { get; set; }
        public bool Descending { get; set; } = false;
    }
}