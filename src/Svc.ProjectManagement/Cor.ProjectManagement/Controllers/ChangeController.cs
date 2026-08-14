// Controllers/ChangeController.cs
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Commands.ChangeCommands;
using Cor.ProjectManagement.Queries.ChangeQueries;

namespace Cor.ProjectManagement.Controllers
{
   [ApiController]
   [Route("api/projectmanagement/v{version:apiVersion}/[controller]")]
   [ApiVersion("1.0")]
   [Authorize]
    public class ChangeController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ChangeController> _logger;

        public ChangeController(IMediator mediator, ILogger<ChangeController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProjectChangeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetChange(Guid id)
        {
            try
            {
                var result = await _mediator.Send(new GetChangeByIdQuery { Id = id });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting change with ID: {ChangeId}", id);
                return StatusCode(500, new { error = "An error occurred while retrieving the change" });
            }
        }

        [HttpGet("project/{projectId}")]
        [ProducesResponseType(typeof(List<ProjectChangeDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetChangesByProject(Guid projectId, [FromQuery] GetChangesByProjectQuery query)
        {
            try
            {
                query.ProjectId = projectId;
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting changes for project ID: {ProjectId}", projectId);
                return StatusCode(500, new { error = "An error occurred while retrieving changes" });
            }
        }

        [HttpGet("project/{projectId}/summary")]
        [ProducesResponseType(typeof(ChangeSummaryDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetChangeSummary(Guid projectId)
        {
            try
            {
                var result = await _mediator.Send(new GetChangeSummaryQuery { ProjectId = projectId });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting change summary for project ID: {ProjectId}", projectId);
                return StatusCode(500, new { error = "An error occurred while retrieving change summary" });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProjectChangeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateChange([FromBody] CreateChangeCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetChange), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating change");
                return StatusCode(500, new { error = "An error occurred while creating the change" });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProjectChangeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateChange(Guid id, [FromBody] UpdateChangeCommand command)
        {
            try
            {
                command.Id = id;
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating change with ID: {ChangeId}", id);
                return StatusCode(500, new { error = "An error occurred while updating the change" });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteChange(Guid id, [FromQuery] string? deletedBy = null)
        {
            try
            {
                await _mediator.Send(new DeleteChangeCommand { Id = id, DeletedBy = deletedBy });
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting change with ID: {ChangeId}", id);
                return StatusCode(500, new { error = "An error occurred while deleting the change" });
            }
        }

        [HttpPost("{id}/approve")]
        [ProducesResponseType(typeof(ProjectChangeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ApproveChange(Guid id, [FromBody] ApproveChangeCommand command)
        {
            try
            {
                command.Id = id;
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving change with ID: {ChangeId}", id);
                return StatusCode(500, new { error = "An error occurred while approving the change" });
            }
        }

        [HttpPost("{id}/reject")]
        [ProducesResponseType(typeof(ProjectChangeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RejectChange(Guid id, [FromBody] RejectChangeCommand command)
        {
            try
            {
                command.Id = id;
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting change with ID: {ChangeId}", id);
                return StatusCode(500, new { error = "An error occurred while rejecting the change" });
            }
        }

        [HttpPost("{id}/implement")]
        [ProducesResponseType(typeof(ProjectChangeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ImplementChange(Guid id, [FromBody] ImplementChangeCommand command)
        {
            try
            {
                command.Id = id;
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error implementing change with ID: {ChangeId}", id);
                return StatusCode(500, new { error = "An error occurred while implementing the change" });
            }
        }
    }
}