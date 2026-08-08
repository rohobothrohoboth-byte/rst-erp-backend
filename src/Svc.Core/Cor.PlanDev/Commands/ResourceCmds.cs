using MediatR;
using  Cor.PlanDev.Models.DTOs;

namespace Cor.PlanDev.Commands;

// ============================================================
// CREATE RESOURCE
// ============================================================
public class CreateResourceCommand : IRequest<ResourceDto>
{
    public CreateResourceDto CreateDto { get; set; } = new();
}

// ============================================================
// UPDATE RESOURCE
// ============================================================
public class UpdateResourceCommand : IRequest<ResourceDto>
{
    public UpdateResourceDto UpdateDto { get; set; } = new();
}

// ============================================================
// DELETE RESOURCE
// ============================================================
public class DeleteResourceCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}

// ============================================================
// UPDATE RESOURCE STATUS
// ============================================================
public class UpdateResourceStatusCommand : IRequest<ResourceDto>
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty; // Active, Inactive, Completed
}

// ============================================================
// UPDATE RESOURCE ALLOCATION
// ============================================================
public class UpdateResourceAllocationCommand : IRequest<ResourceDto>
{
    public Guid Id { get; set; }
    public decimal Allocation { get; set; }
}

// ============================================================
// BULK CREATE RESOURCES
// ============================================================
public class BulkCreateResourcesCommand : IRequest<List<ResourceDto>>
{
    public List<CreateResourceDto> Resources { get; set; } = new();
}

// ============================================================
// UPDATE RESOURCE HOURS
// ============================================================
public class UpdateResourceHoursCommand : IRequest<ResourceDto>
{
    public Guid Id { get; set; }
    public int HoursWorked { get; set; }
}