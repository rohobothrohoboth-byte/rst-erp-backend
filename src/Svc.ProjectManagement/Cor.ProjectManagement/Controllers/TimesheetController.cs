// Controllers/TimesheetController.cs
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Commands.TaskCommands;
using Cor.ProjectManagement.Queries.TaskQueries;
using Cor.ProjectManagement.Commands.ResourceCommands;
using Cor.ProjectManagement.Commands.MilestoneCommands;
using Cor.ProjectManagement.Queries.TimesheetQueries;

 using Cor.ProjectManagement.Commands.TimesheetCommands;
namespace Cor.ProjectManagement.Controllers;


[ApiController]
[Route("api/projectmanagement/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class TimesheetController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ILogger<TimesheetController> _logger;

    public TimesheetController(IMediator mediator, ILogger<TimesheetController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTimesheet(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetTimesheetByIdQuery { Id = id });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting timesheet with ID: {TimesheetId}", id);
            return StatusCode(500, new { error = "An error occurred while retrieving the timesheet" });
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetTimesheetsByUser(Guid userId, [FromQuery] GetTimesheetsByUserQuery query)
    {
        try
        {
            query.UserId = userId;
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting timesheets for user ID: {UserId}", userId);
            return StatusCode(500, new { error = "An error occurred while retrieving timesheets" });
        }
    }

    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetTimesheetsByProject(Guid projectId, [FromQuery] GetTimesheetsByProjectQuery query)
    {
        try
        {
            query.ProjectId = projectId;
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting timesheets for project ID: {ProjectId}", projectId);
            return StatusCode(500, new { error = "An error occurred while retrieving timesheets" });
        }
    }

    [HttpGet("summary/{userId}")]
    public async Task<IActionResult> GetTimesheetSummary(Guid userId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
    {
        try
        {
            var result = await _mediator.Send(new GetTimesheetSummaryQuery
            {
                UserId = userId,
                FromDate = fromDate,
                ToDate = toDate
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting timesheet summary for user ID: {UserId}", userId);
            return StatusCode(500, new { error = "An error occurred while retrieving timesheet summary" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateTimesheet([FromBody] CreateTimesheetCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetTimesheet), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating timesheet");
            return StatusCode(500, new { error = "An error occurred while creating the timesheet" });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTimesheet(Guid id, [FromBody] UpdateTimesheetCommand command)
    {
        try
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating timesheet with ID: {TimesheetId}", id);
            return StatusCode(500, new { error = "An error occurred while updating the timesheet" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTimesheet(Guid id, [FromQuery] string? deletedBy = null)
    {
        try
        {
            await _mediator.Send(new DeleteTimesheetCommand { Id = id, DeletedBy = deletedBy });
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting timesheet with ID: {TimesheetId}", id);
            return StatusCode(500, new { error = "An error occurred while deleting the timesheet" });
        }
    }

    [HttpPost("submit")]
    public async Task<IActionResult> SubmitTimesheets([FromBody] SubmitTimesheetCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting timesheets");
            return StatusCode(500, new { error = "An error occurred while submitting timesheets" });
        }
    }

    [HttpPost("approve")]
    public async Task<IActionResult> ApproveTimesheets([FromBody] ApproveTimesheetCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving timesheets");
            return StatusCode(500, new { error = "An error occurred while approving timesheets" });
        }
    }

    [HttpPost("reject")]
    public async Task<IActionResult> RejectTimesheets([FromBody] RejectTimesheetCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting timesheets");
            return StatusCode(500, new { error = "An error occurred while rejecting timesheets" });
        }
    }
}