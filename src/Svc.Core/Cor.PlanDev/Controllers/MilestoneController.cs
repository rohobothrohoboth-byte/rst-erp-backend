using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cor.PlanDev.Commands;
using Cor.PlanDev.Queries;
using Cor.PlanDev.Models.DTOs;

namespace Cor.PlanDev.Controllers;

[ApiController]
[Route("api/plandev/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class MilestoneController : BaseApiController
{
    private readonly IMediator _mediator;

    public MilestoneController(IMediator mediator, ILogger<MilestoneController> logger)
        : base(logger)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all milestones for a project
    /// </summary>
    [HttpGet("by-project/{projectId}")]
    [ProducesResponseType(typeof(List<MilestoneDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByProject(
        Guid projectId,
        [FromQuery] string? status = null)
    {
        try
        {
            var query = new GetMilestonesByProjectQuery
            {
                ProjectId = projectId,
                Status = status
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetByProject));
        }
    }

    /// <summary>
    /// Get milestone by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MilestoneDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var query = new GetMilestoneByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            if (result == null)
                return NotFound(new { message = $"Milestone with ID '{id}' not found" });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetById));
        }
    }

    /// <summary>
    /// Get upcoming milestones
    /// </summary>
    [HttpGet("upcoming")]
    [ProducesResponseType(typeof(List<MilestoneDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUpcoming(
        [FromQuery] int days = 30,
        [FromQuery] Guid? projectId = null)
    {
        try
        {
            var query = new GetUpcomingMilestonesQuery
            {
                Days = days,
                ProjectId = projectId
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetUpcoming));
        }
    }

    /// <summary>
    /// Get critical milestones
    /// </summary>
    [HttpGet("critical/{projectId}")]
    [ProducesResponseType(typeof(List<MilestoneDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCritical(Guid projectId)
    {
        try
        {
            var query = new GetCriticalMilestonesQuery { ProjectId = projectId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetCritical));
        }
    }

    /// <summary>
    /// Get milestone status summary
    /// </summary>
    [HttpGet("summary/{projectId}")]
    [ProducesResponseType(typeof(MilestoneStatusSummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummary(Guid projectId)
    {
        try
        {
            var query = new GetMilestoneStatusSummaryQuery { ProjectId = projectId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetSummary));
        }
    }

    /// <summary>
    /// Create a new milestone
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(MilestoneDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMilestoneDto createDto)
    {
        try
        {
            var command = new CreateMilestoneCommand { CreateDto = createDto };
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Create));
        }
    }

    /// <summary>
    /// Update a milestone
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(MilestoneDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] UpdateMilestoneDto updateDto)
    {
        try
        {
            var command = new UpdateMilestoneCommand { UpdateDto = updateDto };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Update));
        }
    }

    /// <summary>
    /// Delete a milestone
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var command = new DeleteMilestoneCommand { Id = id };
            var result = await _mediator.Send(command);
            if (!result)
                return NotFound(new { message = $"Milestone with ID '{id}' not found" });
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Delete));
        }
    }

    /// <summary>
    /// Achieve a milestone
    /// </summary>
    [HttpPatch("{id}/achieve")]
    [ProducesResponseType(typeof(MilestoneDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Achieve(Guid id)
    {
        try
        {
            var command = new AchieveMilestoneCommand { Id = id };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Achieve));
        }
    }

    /// <summary>
    /// Update milestone progress
    /// </summary>
    [HttpPatch("{id}/progress")]
    [ProducesResponseType(typeof(MilestoneDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProgress(Guid id, [FromBody] decimal completionPercentage)
    {
        try
        {
            var command = new UpdateMilestoneProgressCommand
            {
                Id = id,
                CompletionPercentage = completionPercentage
            };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(UpdateProgress));
        }
    }

    /// <summary>
    /// Bulk create milestones
    /// </summary>
    [HttpPost("bulk")]
    [ProducesResponseType(typeof(List<MilestoneDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BulkCreate([FromBody] BulkCreateMilestonesCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetByProject), new { projectId = command.Milestones.FirstOrDefault()?.ProjectId }, result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(BulkCreate));
        }
    }
}