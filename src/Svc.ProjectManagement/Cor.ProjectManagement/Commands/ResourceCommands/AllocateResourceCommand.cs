// Commands/ResourceCommands/AllocateResourceCommand.cs
using MediatR;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Commands.ResourceCommands
{
    public class AllocateResourceCommand : IRequest<ProjectResourceDto>
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

    public class UpdateResourceAllocationCommand : IRequest<ProjectResourceDto>
    {
        public Guid Id { get; set; }
        public int? Quantity { get; set; }
        public decimal? CostPerUnit { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ResourceAllocationStatus? Status { get; set; }
        public string? Notes { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class ReleaseResourceCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string? ReleasedBy { get; set; }
        public string? Notes { get; set; }
    }
}