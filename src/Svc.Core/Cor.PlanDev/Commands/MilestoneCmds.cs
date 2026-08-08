using MediatR;
using Cor.PlanDev.Models.DTOs;

namespace Cor.PlanDev.Commands;

// ============================================================
// CREATE MILESTONE
// ============================================================
public class CreateMilestoneCommand : IRequest<MilestoneDto>
{
    public CreateMilestoneDto CreateDto { get; set; } = new();
}

// ============================================================
// UPDATE MILESTONE
// ============================================================
public class UpdateMilestoneCommand : IRequest<MilestoneDto>
{
    public UpdateMilestoneDto UpdateDto { get; set; } = new();
}

// ============================================================
// DELETE MILESTONE
// ============================================================
public class DeleteMilestoneCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}

// ============================================================
// ACHIEVE MILESTONE
// ============================================================
public class AchieveMilestoneCommand : IRequest<MilestoneDto>
{
    public Guid Id { get; set; }
}

// ============================================================
// UPDATE MILESTONE PROGRESS
// ============================================================
public class UpdateMilestoneProgressCommand : IRequest<MilestoneDto>
{
    public Guid Id { get; set; }
    public decimal CompletionPercentage { get; set; }
}

// ============================================================
// BULK CREATE MILESTONES
// ============================================================
public class BulkCreateMilestonesCommand : IRequest<List<MilestoneDto>>
{
    public List<CreateMilestoneDto> Milestones { get; set; } = new();
}