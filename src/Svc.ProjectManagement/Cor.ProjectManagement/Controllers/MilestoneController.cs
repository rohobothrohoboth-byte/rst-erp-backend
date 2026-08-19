// Controllers/MilestoneController.cs
// Controllers/IssueController.cs
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Commands.IssueCommands;
using Cor.ProjectManagement.Queries.IssueQueries;
using Cor.ProjectManagement.Commands.TaskCommands;
using Cor.ProjectManagement.Queries.ResourceQueries;
using Cor.ProjectManagement.Commands.ResourceCommands;
using Cor.ProjectManagement.Commands.MilestoneCommands;
using Cor.ProjectManagement.Queries.MilestoneQueries;
namespace Cor.ProjectManagement.Controllers;


[ApiController]
[Route("api/projectmanagement/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class MilestoneController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ILogger<MilestoneController> _logger;

    public MilestoneController(IMediator mediator, ILogger<MilestoneController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMilestone(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetMilestoneByIdQuery { Id = id });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting milestone with ID: {MilestoneId}", id);
            return StatusCode(500, new { error = "An error occurred while retrieving the milestone" });
        }
    }

    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetMilestonesByProject(Guid projectId, [FromQuery] GetMilestonesByProjectQuery query)
    {
        try
        {
            query.ProjectId = projectId;
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting milestones for project ID: {ProjectId}", projectId);
            return StatusCode(500, new { error = "An error occurred while retrieving milestones" });
        }
    }

    [HttpGet("project/{projectId}/upcoming")]
    public async Task<IActionResult> GetUpcomingMilestones(Guid projectId, [FromQuery] int daysThreshold = 30)
    {
        try
        {
            var result = await _mediator.Send(new GetUpcomingMilestonesQuery
            {
                ProjectId = projectId,
                DaysThreshold = daysThreshold
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting upcoming milestones for project ID: {ProjectId}", projectId);
            return StatusCode(500, new { error = "An error occurred while retrieving upcoming milestones" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateMilestone([FromBody] CreateMilestoneCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetMilestone), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating milestone");
            return StatusCode(500, new { error = "An error occurred while creating the milestone" });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMilestone(Guid id, [FromBody] UpdateMilestoneCommand command)
    {
        try
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating milestone with ID: {MilestoneId}", id);
            return StatusCode(500, new { error = "An error occurred while updating the milestone" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMilestone(Guid id, [FromQuery] string? deletedBy = null)
    {
        try
        {
            await _mediator.Send(new DeleteMilestoneCommand { Id = id, DeletedBy = deletedBy });
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting milestone with ID: {MilestoneId}", id);
            return StatusCode(500, new { error = "An error occurred while deleting the milestone" });
        }
    }

    [HttpPost("{id}/complete")]
    public async Task<IActionResult> CompleteMilestone(Guid id, [FromBody] CompleteMilestoneCommand command)
    {
        try
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing milestone with ID: {MilestoneId}", id);
            return StatusCode(500, new { error = "An error occurred while completing the milestone" });
        }
    }
}