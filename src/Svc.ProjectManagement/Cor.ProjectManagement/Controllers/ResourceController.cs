// Controllers/ResourceController.cs
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Models.Entities;
using Cor.ProjectManagement.Commands.ProjectCommands;
using Cor.ProjectManagement.Queries.ProjectQueries;
using Cor.ProjectManagement.Commands.TaskCommands;
using Cor.ProjectManagement.Queries.ResourceQueries;
using Cor.ProjectManagement.Commands.ResourceCommands;
using Cor.ProjectManagement.Commands.MilestoneCommands;
namespace Cor.ProjectManagement.Controllers;


[ApiController]
[Route("api/projectmanagement/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class ResourceController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ILogger<ResourceController> _logger;

    public ResourceController(IMediator mediator, ILogger<ResourceController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetResourceAllocations([FromQuery] GetResourceAllocationsQuery query)
    {
        try
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting resource allocations");
            return StatusCode(500, new { error = "An error occurred while retrieving resource allocations" });
        }
    }

    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetResourcesByProject(Guid projectId, [FromQuery] ResourceAllocationStatus? status)
    {
        try
        {
            var result = await _mediator.Send(new GetResourcesByProjectQuery
            {
                ProjectId = projectId,
                Status = status
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting resources for project ID: {ProjectId}", projectId);
            return StatusCode(500, new { error = "An error occurred while retrieving resources" });
        }
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableResources([FromQuery] GetAvailableResourcesQuery query)
    {
        try
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available resources");
            return StatusCode(500, new { error = "An error occurred while retrieving available resources" });
        }
    }

    [HttpPost("allocate")]
    public async Task<IActionResult> AllocateResource([FromBody] AllocateResourceCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error allocating resource");
            return StatusCode(500, new { error = "An error occurred while allocating the resource" });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAllocation(Guid id, [FromBody] UpdateResourceAllocationCommand command)
    {
        try
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating resource allocation with ID: {ResourceId}", id);
            return StatusCode(500, new { error = "An error occurred while updating the resource allocation" });
        }
    }

    [HttpPost("{id}/release")]
    public async Task<IActionResult> ReleaseResource(Guid id, [FromBody] ReleaseResourceCommand command)
    {
        try
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error releasing resource with ID: {ResourceId}", id);
            return StatusCode(500, new { error = "An error occurred while releasing the resource" });
        }
    }
}