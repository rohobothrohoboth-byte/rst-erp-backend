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
public class TaskController : BaseApiController
{
    private readonly IMediator _mediator;

    public TaskController(IMediator mediator, ILogger<TaskController> logger)
        : base(logger)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all tasks for a project
    /// </summary>
    [HttpGet("by-project/{projectId}")]
    [ProducesResponseType(typeof(List<TaskDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByProject(
        Guid projectId,
        [FromQuery] string? status = null,
        [FromQuery] Guid? assignedToUserId = null,
        [FromQuery] string? priority = null)
    {
        try
        {
            var query = new GetTasksByProjectQuery
            {
                ProjectId = projectId,
                Status = status,
                AssignedToUserId = assignedToUserId,
                Priority = priority
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
    /// Get task by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var query = new GetTaskByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            if (result == null)
                return NotFound(new { message = $"Task with ID '{id}' not found" });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetById));
        }
    }

    /// <summary>
    /// Get subtasks by parent task
    /// </summary>
    [HttpGet("subtasks/{parentTaskId}")]
    [ProducesResponseType(typeof(List<TaskDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSubtasks(Guid parentTaskId)
    {
        try
        {
            var query = new GetTaskByParentQuery { ParentTaskId = parentTaskId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetSubtasks));
        }
    }

    /// <summary>
    /// Search tasks
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(List<TaskDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromQuery] string? searchTerm = null,
        [FromQuery] Guid? projectId = null,
        [FromQuery] string? status = null,
        [FromQuery] Guid? assignedToUserId = null)
    {
        try
        {
            var query = new SearchTasksQuery
            {
                SearchTerm = searchTerm,
                ProjectId = projectId,
                Status = status,
                AssignedToUserId = assignedToUserId
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Search));
        }
    }

    /// <summary>
    /// Get task status summary
    /// </summary>
    [HttpGet("summary/{projectId}")]
    [ProducesResponseType(typeof(TaskStatusSummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummary(Guid projectId)
    {
        try
        {
            var query = new GetTaskStatusSummaryQuery { ProjectId = projectId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetSummary));
        }
    }

    /// <summary>
    /// Get tasks by assignee
    /// </summary>
    [HttpGet("by-assignee/{assigneeId}")]
    [ProducesResponseType(typeof(List<TaskDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByAssignee(
        Guid assigneeId,
        [FromQuery] string? status = null)
    {
        try
        {
            var query = new GetTaskByAssigneeQuery
            {
                AssigneeId = assigneeId,
                Status = status
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetByAssignee));
        }
    }

    /// <summary>
    /// Create a new task
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto createDto)
    {
        try
        {
            var command = new CreateTaskCommand { CreateDto = createDto };
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Create));
        }
    }

    /// <summary>
    /// Update a task
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] UpdateTaskDto updateDto)
    {
        try
        {
            var command = new UpdateTaskCommand { UpdateDto = updateDto };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Update));
        }
    }

    /// <summary>
    /// Delete a task
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var command = new DeleteTaskCommand { Id = id };
            var result = await _mediator.Send(command);
            if (!result)
                return NotFound(new { message = $"Task with ID '{id}' not found" });
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Delete));
        }
    }

    /// <summary>
    /// Update task status
    /// </summary>
    [HttpPatch("status")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateTaskStatusCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(UpdateStatus));
        }
    }

    /// <summary>
    /// Update task progress
    /// </summary>
    [HttpPatch("{id}/progress")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProgress(Guid id, [FromBody] int progress)
    {
        try
        {
            var command = new UpdateTaskProgressCommand
            {
                Id = id,
                Progress = progress
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
    /// Assign task to user
    /// </summary>
    [HttpPatch("assign")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Assign([FromBody] AssignTaskCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Assign));
        }
    }

    /// <summary>
    /// Update task hours
    /// </summary>
    [HttpPatch("{id}/hours")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateHours(Guid id, [FromBody] int actualHours)
    {
        try
        {
            var command = new UpdateTaskHoursCommand
            {
                Id = id,
                ActualHours = actualHours
            };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(UpdateHours));
        }
    }

    /// <summary>
    /// Reorder tasks
    /// </summary>
    [HttpPost("reorder")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Reorder([FromBody] ReorderTasksCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(new { success = result });
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Reorder));
        }
    }

    /// <summary>
    /// Bulk create tasks
    /// </summary>
    [HttpPost("bulk")]
    [ProducesResponseType(typeof(List<TaskDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BulkCreate([FromBody] BulkCreateTasksCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetByProject), new { projectId = command.Tasks.FirstOrDefault()?.ProjectId }, result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(BulkCreate));
        }
    }
}