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
public class ResourceController : BaseApiController
{
    private readonly IMediator _mediator;

    public ResourceController(IMediator mediator, ILogger<ResourceController> logger)
        : base(logger)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all resources for a project
    /// </summary>
    [HttpGet("by-project/{projectId}")]
    [ProducesResponseType(typeof(List<ResourceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByProject(
        Guid projectId,
        [FromQuery] string? status = null,
        [FromQuery] Guid? resourceUserId = null)
    {
        try
        {
            var query = new GetResourcesByProjectQuery
            {
                ProjectId = projectId,
                Status = status,
                ResourceUserId = resourceUserId
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
    /// Get resource by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResourceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var query = new GetResourceByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            if (result == null)
                return NotFound(new { message = $"Resource with ID '{id}' not found" });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetById));
        }
    }

    /// <summary>
    /// Get resource allocation
    /// </summary>
    [HttpGet("allocation/{projectId}")]
    [ProducesResponseType(typeof(List<ResourceAllocationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllocation(Guid projectId)
    {
        try
        {
            var query = new GetResourceAllocationQuery { ProjectId = projectId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetAllocation));
        }
    }

    /// <summary>
    /// Get resource utilization
    /// </summary>
    [HttpGet("utilization/{projectId}")]
    [ProducesResponseType(typeof(ResourceUtilizationDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUtilization(Guid projectId)
    {
        try
        {
            var query = new GetResourceUtilizationQuery { ProjectId = projectId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetUtilization));
        }
    }

    /// <summary>
    /// Create a new resource
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ResourceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateResourceDto createDto)
    {
        try
        {
            var command = new CreateResourceCommand { CreateDto = createDto };
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Create));
        }
    }

    /// <summary>
    /// Update a resource
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(ResourceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] UpdateResourceDto updateDto)
    {
        try
        {
            var command = new UpdateResourceCommand { UpdateDto = updateDto };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Update));
        }
    }

    /// <summary>
    /// Delete a resource
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var command = new DeleteResourceCommand { Id = id };
            var result = await _mediator.Send(command);
            if (!result)
                return NotFound(new { message = $"Resource with ID '{id}' not found" });
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Delete));
        }
    }

    /// <summary>
    /// Update resource status
    /// </summary>
    [HttpPatch("status")]
    [ProducesResponseType(typeof(ResourceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateResourceStatusCommand command)
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
    /// Update resource allocation
    /// </summary>
    [HttpPatch("{id}/allocation")]
    [ProducesResponseType(typeof(ResourceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAllocation(Guid id, [FromBody] decimal allocation)
    {
        try
        {
            var command = new UpdateResourceAllocationCommand
            {
                Id = id,
                Allocation = allocation
            };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(UpdateAllocation));
        }
    }

    /// <summary>
    /// Update resource hours
    /// </summary>
    [HttpPatch("{id}/hours")]
    [ProducesResponseType(typeof(ResourceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateHours(Guid id, [FromBody] int hoursWorked)
    {
        try
        {
            var command = new UpdateResourceHoursCommand
            {
                Id = id,
                HoursWorked = hoursWorked
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
    /// Bulk create resources
    /// </summary>
    [HttpPost("bulk")]
    [ProducesResponseType(typeof(List<ResourceDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BulkCreate([FromBody] BulkCreateResourcesCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetByProject), new { projectId = command.Resources.FirstOrDefault()?.ProjectId }, result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(BulkCreate));
        }
    }
}