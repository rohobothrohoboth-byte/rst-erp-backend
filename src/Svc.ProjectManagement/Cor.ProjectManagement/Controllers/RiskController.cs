// Controllers/RiskController.cs
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Commands.RiskCommands;
using Cor.ProjectManagement.Queries.RiskQueries;

namespace Cor.ProjectManagement.Controllers
{
   [ApiController]
   [Route("api/projectmanagement/v{version:apiVersion}/[controller]")]
   [ApiVersion("1.0")]
   [Authorize]
    public class RiskController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RiskController> _logger;

        public RiskController(IMediator mediator, ILogger<RiskController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProjectRiskDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRisk(Guid id)
        {
            try
            {
                var result = await _mediator.Send(new GetRiskByIdQuery { Id = id });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting risk with ID: {RiskId}", id);
                return StatusCode(500, new { error = "An error occurred while retrieving the risk" });
            }
        }

        [HttpGet("project/{projectId}")]
        [ProducesResponseType(typeof(List<ProjectRiskDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRisksByProject(Guid projectId, [FromQuery] GetRisksByProjectQuery query)
        {
            try
            {
                query.ProjectId = projectId;
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting risks for project ID: {ProjectId}", projectId);
                return StatusCode(500, new { error = "An error occurred while retrieving risks" });
            }
        }

        [HttpGet("project/{projectId}/heatmap")]
        [ProducesResponseType(typeof(RiskHeatmapDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRiskHeatmap(Guid projectId)
        {
            try
            {
                var result = await _mediator.Send(new GetRiskHeatmapQuery { ProjectId = projectId });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting risk heatmap for project ID: {ProjectId}", projectId);
                return StatusCode(500, new { error = "An error occurred while retrieving risk heatmap" });
            }
        }

        [HttpGet("project/{projectId}/summary")]
        [ProducesResponseType(typeof(RiskSummaryDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRiskSummary(Guid projectId)
        {
            try
            {
                var result = await _mediator.Send(new GetRiskSummaryQuery { ProjectId = projectId });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting risk summary for project ID: {ProjectId}", projectId);
                return StatusCode(500, new { error = "An error occurred while retrieving risk summary" });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProjectRiskDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateRisk([FromBody] CreateRiskCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetRisk), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating risk");
                return StatusCode(500, new { error = "An error occurred while creating the risk" });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProjectRiskDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateRisk(Guid id, [FromBody] UpdateRiskCommand command)
        {
            try
            {
                command.Id = id;
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating risk with ID: {RiskId}", id);
                return StatusCode(500, new { error = "An error occurred while updating the risk" });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRisk(Guid id, [FromQuery] string? deletedBy = null)
        {
            try
            {
                await _mediator.Send(new DeleteRiskCommand { Id = id, DeletedBy = deletedBy });
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting risk with ID: {RiskId}", id);
                return StatusCode(500, new { error = "An error occurred while deleting the risk" });
            }
        }

        [HttpPost("{id}/resolve")]
        [ProducesResponseType(typeof(ProjectRiskDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ResolveRisk(Guid id, [FromBody] ResolveRiskCommand command)
        {
            try
            {
                command.Id = id;
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving risk with ID: {RiskId}", id);
                return StatusCode(500, new { error = "An error occurred while resolving the risk" });
            }
        }

        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(ProjectRiskDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateRiskStatus(Guid id, [FromBody] UpdateRiskStatusCommand command)
        {
            try
            {
                command.Id = id;
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating risk status with ID: {RiskId}", id);
                return StatusCode(500, new { error = "An error occurred while updating risk status" });
            }
        }
    }
}