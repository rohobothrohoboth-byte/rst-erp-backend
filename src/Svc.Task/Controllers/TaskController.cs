using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Svc.Task.Commands;
using Svc.Task.Models.Dtos;
using Svc.Task.Queries;

namespace Svc.Task.Controllers;

[ApiController]
[Route("api/auth/v{version:apiVersion}/Task")]
[ApiVersion("1.0")]
public class TaskController : ControllerBase
{
    private readonly IMediator _med;

    public TaskController(IMediator med)
    {
        _med = med;
    }

    [HttpGet("user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserTasks(Guid userId, [FromQuery] string? status = null, [FromQuery] string? priority = null)
    {
        var result = await _med.Send(new TaskAllQry { UserId = userId, Status = status, Priority = priority });
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTaskById(Guid id)
    {
        var result = await _med.Send(new TaskByIdQry { Id = id });
        if (result == null)
            throw new DomainException($"Task with id [{id}] not found");
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("stats/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTaskStats(Guid userId)
    {
        var stats = await _med.Send(new TaskStatsQry { UserId = userId });
        return Ok(ApiResponse<object>.Ok(stats));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTask([FromBody] TaskAddDto dto)
    {
        var result = await _med.Send(new TaskAddCmd { AddDto = dto });
        return Ok(ApiResponse<object>.Ok(result, "Task created successfully"));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTask(Guid id, [FromBody] TaskModDto dto)
    {
        if (id != dto.Id)
            throw new DomainException("ID mismatch");

        var result = await _med.Send(new TaskModCmd { ModDto = dto });
        return Ok(ApiResponse<object>.Ok(result, "Task updated successfully"));
    }

    [HttpPatch("{id}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateTaskStatus(Guid id, [FromBody] UpdateTaskStatusDto dto)
    {
        await _med.Send(new TaskUpdateStatusCmd { Id = id, Status = dto.Status });
        return Ok(ApiResponse<object>.Ok(null, "Task status updated successfully"));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        await _med.Send(new TaskDeleteCmd { Id = id });
        return Ok(ApiResponse<object>.Ok(null, "Task deleted successfully"));
    }

    [HttpPost("bulk-delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> BulkDeleteTasks([FromBody] List<Guid> ids)
    {
        await _med.Send(new TaskBulkDeleteCmd { Ids = ids });
        return Ok(ApiResponse<object>.Ok(null, $"{ids.Count} tasks deleted successfully"));
    }

    [HttpGet("paginated")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaginatedTasks([FromQuery] TaskPaginatedQry query)
    {
        var result = await _med.Send(query);
        return Ok(ApiResponse<PaginatedResult<TaskDto>>.Ok(result));
    }
}