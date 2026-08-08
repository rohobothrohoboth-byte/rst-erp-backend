// Cor.CRM/Controllers/TaskController.cs
using Asp.Versioning;
using Cor.CRM.Commands;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Queries;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cor.CRM.Interfaces;

namespace Cor.CRM.Controllers;

[Authorize]
[ApiController]
[Route("api/core/crm/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class TaskController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogService _logger;

    public TaskController(IMediator mediator, ILogService logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Create a new task
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
    {
        try
        {
            var command = new TaskAddCmd { Dto = dto };
            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = response.Id },
                ApiResponse<object>.Ok(response, "Task created successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating task");
            throw;
        }
    }

    /// <summary>
    /// Update an existing task
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskDto dto)
    {
        try
        {
            var command = new TaskUpdateCmd { Id = id, Dto = dto };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Task updated successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task: {TaskId}", id);
            throw;
        }
    }

    /// <summary>
    /// Get task by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new TaskByIdQry { Id = id });
            if (response == null)
                throw new DomainException($"Task with id [{id}] NOT FOUND.");

            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting task: {TaskId}", id);
            throw;
        }
    }

    /// <summary>
    /// Get all tasks with filters
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? status,
        [FromQuery] string? priority,
        [FromQuery] Guid? leadId,
        [FromQuery] Guid? customerId,
        [FromQuery] Guid? opportunityId,
        [FromQuery] Guid? assignedToUserId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var query = new TaskAllQry
            {
                Status = status,
                Priority = priority,
                LeadId = leadId,
                CustomerId = customerId,
                OpportunityId = opportunityId,
                AssignedToUserId = assignedToUserId,
                FromDate = fromDate,
                ToDate = toDate,
                Page = page,
                PageSize = pageSize
            };

            var response = await _mediator.Send(query);
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tasks");
            throw;
        }
    }

    /// <summary>
    /// Update task status
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateTaskStatusDto dto)
    {
        try
        {
            var command = new TaskUpdateStatusCmd { Id = id, Status = dto.Status };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Task status updated successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task status: {TaskId}", id);
            throw;
        }
    }

    /// <summary>
    /// Complete a task
    /// </summary>
    [HttpPost("{id:guid}/complete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Complete(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new TaskCompleteCmd { Id = id });
            return Ok(ApiResponse<object>.Ok(response, "Task completed successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing task: {TaskId}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete a task (soft delete)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _mediator.Send(new TaskDeleteCmd { Id = id });
            return Ok(ApiResponse<string>.Ok(null!, $"Task with Id {id} successfully deleted."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting task: {TaskId}", id);
            throw;
        }
    }

    /// <summary>
    /// Get task statistics
    /// </summary>
    [HttpGet("stats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var response = await _mediator.Send(new TaskStatsQry());
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting task stats");
            throw;
        }
    }
}

