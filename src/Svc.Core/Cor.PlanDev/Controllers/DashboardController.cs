using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cor.PlanDev.Queries;
using Cor.PlanDev.Models.DTOs;

namespace Cor.PlanDev.Controllers;

[ApiController]
[Route("api/plandev/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class DashboardController : BaseApiController
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator, ILogger<DashboardController> logger)
        : base(logger)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get overall dashboard summary
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(DashboardSummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummary()
    {
        try
        {
            var result = await GetDashboardSummaryAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetSummary));
        }
    }

    /// <summary>
    /// Get project dashboard data
    /// </summary>
    [HttpGet("project/{projectId}")]
    [ProducesResponseType(typeof(ProjectDashboardDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProjectDashboard(Guid projectId)
    {
        try
        {
            var result = await GetProjectDashboardAsync(projectId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetProjectDashboard));
        }
    }

    /// <summary>
    /// Get resource utilization dashboard
    /// </summary>
    [HttpGet("resources")]
    [ProducesResponseType(typeof(ResourceUtilizationDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetResourceUtilization()
    {
        try
        {
            var query = new GetResourceUtilizationQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetResourceUtilization));
        }
    }

    /// <summary>
    /// Get upcoming milestones
    /// </summary>
    [HttpGet("upcoming-milestones")]
    [ProducesResponseType(typeof(List<MilestoneDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUpcomingMilestones([FromQuery] int days = 30)
    {
        try
        {
            var query = new GetUpcomingMilestonesQuery { Days = days };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetUpcomingMilestones));
        }
    }

    private async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
    {
        // Get all projects
        var projectQuery = new GetAllProjectsQuery();
        var projects = await _mediator.Send(projectQuery);

        var totalProjects = projects.Count;
        var activeProjects = projects.Count(p => p.Status == "Active");
        var completedProjects = projects.Count(p => p.Status == "Completed");
        var planningProjects = projects.Count(p => p.Status == "Planning");
        var onHoldProjects = projects.Count(p => p.Status == "OnHold");

        var totalBudget = projects.Sum(p => p.Budget);
        var totalActualCost = projects.Sum(p => p.ActualCost);
        var totalTasks = projects.Sum(p => p.TaskCount);
        var completedTasks = projects.Sum(p => p.CompletedTasks);

        return new DashboardSummaryDto
        {
            TotalProjects = totalProjects,
            ActiveProjects = activeProjects,
            CompletedProjects = completedProjects,
            PlanningProjects = planningProjects,
            OnHoldProjects = onHoldProjects,
            TotalBudget = totalBudget,
            TotalActualCost = totalActualCost,
            TotalTasks = totalTasks,
            CompletedTasks = completedTasks,
            OverallProgress = totalTasks > 0 ? (completedTasks / (decimal)totalTasks) * 100 : 0
        };
    }

    private async Task<ProjectDashboardDto> GetProjectDashboardAsync(Guid projectId)
    {
        var project = await _mediator.Send(new GetProjectByIdQuery { Id = projectId });

        if (project == null)
            throw new KeyNotFoundException($"Project with ID '{projectId}' not found");

        var tasks = await _mediator.Send(new GetTasksByProjectQuery { ProjectId = projectId });
        var milestones = await _mediator.Send(new GetMilestonesByProjectQuery { ProjectId = projectId });
        var budgets = await _mediator.Send(new GetBudgetsByProjectQuery { ProjectId = projectId });

        var taskSummary = await _mediator.Send(new GetTaskStatusSummaryQuery { ProjectId = projectId });
        var milestoneSummary = await _mediator.Send(new GetMilestoneStatusSummaryQuery { ProjectId = projectId });

        return new ProjectDashboardDto
        {
            Project = project,
            Tasks = tasks,
            Milestones = milestones,
            Budgets = budgets,
            TaskSummary = taskSummary,
            MilestoneSummary = milestoneSummary
        };
    }
}

public class DashboardSummaryDto
{
    public int TotalProjects { get; set; }
    public int ActiveProjects { get; set; }
    public int CompletedProjects { get; set; }
    public int PlanningProjects { get; set; }
    public int OnHoldProjects { get; set; }
    public decimal TotalBudget { get; set; }
    public decimal TotalActualCost { get; set; }
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public decimal OverallProgress { get; set; }
}

public class ProjectDashboardDto
{
    public ProjectDto Project { get; set; } = new();
    public List<TaskDto> Tasks { get; set; } = new();
    public List<MilestoneDto> Milestones { get; set; } = new();
    public List<BudgetDto> Budgets { get; set; } = new();
    public TaskStatusSummaryDto? TaskSummary { get; set; }
    public MilestoneStatusSummaryDto? MilestoneSummary { get; set; }
}