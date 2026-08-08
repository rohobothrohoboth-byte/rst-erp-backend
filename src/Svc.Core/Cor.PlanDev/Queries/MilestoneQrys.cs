using MediatR;
using Cor.PlanDev.Models.DTOs;

namespace Cor.PlanDev.Queries;

public class GetMilestonesByProjectQuery : IRequest<List<MilestoneDto>>
{
    public Guid ProjectId { get; set; }
    public string? Status { get; set; }
}

public class GetMilestoneByIdQuery : IRequest<MilestoneDto>
{
    public Guid Id { get; set; }
}

public class GetUpcomingMilestonesQuery : IRequest<List<MilestoneDto>>
{
    public int Days { get; set; } = 30;
    public Guid? ProjectId { get; set; }
}

public class GetCriticalMilestonesQuery : IRequest<List<MilestoneDto>>
{
    public Guid ProjectId { get; set; }
}

public class GetMilestoneStatusSummaryQuery : IRequest<MilestoneStatusSummaryDto>
{
    public Guid ProjectId { get; set; }
}

public class MilestoneStatusSummaryDto
{
    public int Total { get; set; }
    public int Achieved { get; set; }
    public int Pending { get; set; }
    public int Missed { get; set; }
    public int Cancelled { get; set; }
    public int Critical { get; set; }
    public decimal CompletionPercentage { get; set; }
}