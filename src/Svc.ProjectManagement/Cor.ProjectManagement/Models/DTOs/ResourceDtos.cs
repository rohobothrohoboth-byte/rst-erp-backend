// Models/DTOs/ResourceDtos.cs
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Models.DTOs
{
    public class AllocateResourceDto
    {
        public Guid ProjectId { get; set; }
        public Guid ResourceId { get; set; }
        public string? ResourceName { get; set; }
        public ResourceType Type { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal CostPerUnit { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Notes { get; set; }
        public string? UnitOfMeasure { get; set; }
        public string? Skills { get; set; }
        public string? Department { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class UpdateResourceAllocationDto
    {
        public int? Quantity { get; set; }
        public decimal? CostPerUnit { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ResourceAllocationStatus? Status { get; set; }
        public string? Notes { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class ReleaseResourceDto
    {
        public string? ReleasedBy { get; set; }
        public string? Notes { get; set; }
    }

    public class ResourceFilterDto
    {
        public Guid? ProjectId { get; set; }
        public Guid? ResourceId { get; set; }
        public ResourceType? Type { get; set; }
        public ResourceAllocationStatus? Status { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}