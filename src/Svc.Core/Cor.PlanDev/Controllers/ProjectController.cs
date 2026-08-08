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
public class ProjectController : BaseApiController
{
    private readonly IMediator _mediator;

    public ProjectController(IMediator mediator, ILogger<ProjectController> logger)
        : base(logger)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ProjectDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? status = null,
        [FromQuery] string? priority = null,
        [FromQuery] Guid? managerId = null,
        [FromQuery] string? department = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        try
        {
            var query = new GetAllProjectsQuery
            {
                Status = status,
                Priority = priority,
                ManagerId = managerId,
                Department = department,
                FromDate = fromDate,
                ToDate = toDate
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetAll));
        }
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(List<ProjectDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? status = null)
    {
        try
        {
            var query = new SearchProjectsQuery
            {
                SearchTerm = searchTerm,
                Status = status
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Search));
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var query = new GetProjectByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            if (result == null)
                return NotFound(new { message = $"Project with ID '{id}' not found" });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetById));
        }
    }

    [HttpGet("by-code/{code}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string code)
    {
        try
        {
            var query = new GetProjectByCodeQuery { Code = code };
            var result = await _mediator.Send(query);
            if (result == null)
                return NotFound(new { message = $"Project with code '{code}' not found" });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetByCode));
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProjectDto createDto)
    {
        try
        {
            var command = new CreateProjectCommand { CreateDto = createDto };
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Create));
        }
    }

    [HttpPut]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] UpdateProjectDto updateDto)
    {
        try
        {
            var command = new UpdateProjectCommand { UpdateDto = updateDto };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Update));
        }
    }

    [HttpPatch("progress")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProgress([FromBody] UpdateProjectProgressDto progressDto)
    {
        try
        {
            var command = new UpdateProjectProgressCommand { ProgressDto = progressDto };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(UpdateProgress));
        }
    }

    [HttpPatch("{id}/complete")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Complete(Guid id)
    {
        try
        {
            var command = new CompleteProjectCommand { Id = id };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Complete));
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var command = new DeleteProjectCommand { Id = id };
            var result = await _mediator.Send(command);
            if (!result)
                return NotFound(new { message = $"Project with ID '{id}' not found" });
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Delete));
        }
    }
}