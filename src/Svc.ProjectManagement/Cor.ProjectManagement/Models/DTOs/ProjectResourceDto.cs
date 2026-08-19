// Models/DTOs/ProjectResourceDto.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Models.DTOs
{
    public class ProjectResourceDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public Guid ResourceId { get; set; }
        public string ResourceName { get; set; } = string.Empty;
        public ResourceType Type { get; set; }
        public int Quantity { get; set; }
        public decimal CostPerUnit { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ResourceAllocationStatus Status { get; set; }
        public string Notes { get; set; } = string.Empty;
        public string? UnitOfMeasure { get; set; }
        public string Skills { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public bool IsActive => Status == ResourceAllocationStatus.Allocated || Status == ResourceAllocationStatus.InUse;
    }

    public class ProjectResourceCreateDto
    {
        [Required]
        public Guid ProjectId { get; set; }

        [Required]
        public Guid ResourceId { get; set; }

        public string? ResourceName { get; set; }

        [Required]
        public ResourceType Type { get; set; }

        public int Quantity { get; set; } = 1;

        public decimal CostPerUnit { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? Notes { get; set; }

        public string? UnitOfMeasure { get; set; }

        public string? Skills { get; set; }
        public string? Department { get; set; }

        public string? CreatedBy { get; set; }
    }

    public class ProjectResourceUpdateDto
    {
        public int? Quantity { get; set; }
        public decimal? CostPerUnit { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ResourceAllocationStatus? Status { get; set; }
        public string? Notes { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class ResourceAllocationDto
    {
        public Guid ProjectId { get; set; }
        public Guid ResourceId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal AllocationPercentage { get; set; }
        public string? Notes { get; set; }
        public string? AllocatedBy { get; set; }
    }
      public class ResourceAvailabilityDto
        {
            public Guid ResourceId { get; set; }
            public string ResourceName { get; set; } = string.Empty;
            public string ResourceType { get; set; } = string.Empty;
            public string Skills { get; set; } = string.Empty;
            public string Department { get; set; } = string.Empty;
            public bool IsAvailable { get; set; }
            public decimal CostPerHour { get; set; }
            public int AvailableHours { get; set; }
            public string? UnitOfMeasure { get; set; }
        }
}