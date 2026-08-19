// Models/Entities/ProjectResource.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Cor.ProjectManagement.Models.Entities
{
    public enum ResourceType
    {
        Human = 1,
        Equipment = 2,
        Material = 3,
        Software = 4,
        Facility = 5
    }

    public enum ResourceAllocationStatus
    {
        Planned = 1,
        Allocated = 2,
        InUse = 3,
        Released = 4,
        Completed = 5
    }

    public class ProjectResource : BaseEntity
    {
        [Required]
        public Guid ProjectId { get; set; }

        [Required]
        public Guid ResourceId { get; set; }
        public string ResourceName { get; set; } = string.Empty;

        [Required]
        public ResourceType Type { get; set; }

        public int Quantity { get; set; } = 1;

        public decimal CostPerUnit { get; set; }
        public decimal TotalCost { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public ResourceAllocationStatus Status { get; set; } = ResourceAllocationStatus.Planned;

        public string Notes { get; set; } = string.Empty;

        public string? UnitOfMeasure { get; set; }

        public string Skills { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;

        public virtual Project Project { get; set; } = null!;
    }
}