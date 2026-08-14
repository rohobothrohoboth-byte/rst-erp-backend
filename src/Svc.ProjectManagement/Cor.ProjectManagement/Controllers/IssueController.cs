// Controllers/IssueController.cs
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Commands.IssueCommands;
using Cor.ProjectManagement.Queries.IssueQueries;

namespace Cor.ProjectManagement.Controllers
{
   [ApiController]
   [Route("api/projectmanagement/v{version:apiVersion}/[controller]")]
   [ApiVersion("1.0")]
   [Authorize]
    public class IssueController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<IssueController> _logger;

        public IssueController(IMediator mediator, ILogger<IssueController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProjectIssueDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetIssue(Guid id)
        {
            try
            {
                var result = await _mediator.Send(new GetIssueByIdQuery { Id = id });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting issue with ID: {IssueId}", id);
                return StatusCode(500, new { error = "An error occurred while retrieving the issue" });
            }
        }

        [HttpGet("project/{projectId}")]
        [ProducesResponseType(typeof(List<ProjectIssueDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetIssuesByProject(Guid projectId, [FromQuery] GetIssuesByProjectQuery query)
        {
            try
            {
                query.ProjectId = projectId;
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting issues for project ID: {ProjectId}", projectId);
                return StatusCode(500, new { error = "An error occurred while retrieving issues" });
            }
        }

        [HttpGet("project/{projectId}/summary")]
        [ProducesResponseType(typeof(IssueSummaryDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetIssueSummary(Guid projectId)
        {
            try
            {
                var result = await _mediator.Send(new GetIssueSummaryQuery { ProjectId = projectId });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting issue summary for project ID: {ProjectId}", projectId);
                return StatusCode(500, new { error = "An error occurred while retrieving issue summary" });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProjectIssueDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateIssue([FromBody] CreateIssueCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetIssue), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating issue");
                return StatusCode(500, new { error = "An error occurred while creating the issue" });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProjectIssueDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateIssue(Guid id, [FromBody] UpdateIssueCommand command)
        {
            try
            {
                command.Id = id;
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating issue with ID: {IssueId}", id);
                return StatusCode(500, new { error = "An error occurred while updating the issue" });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteIssue(Guid id, [FromQuery] string? deletedBy = null)
        {
            try
            {
                await _mediator.Send(new DeleteIssueCommand { Id = id, DeletedBy = deletedBy });
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting issue with ID: {IssueId}", id);
                return StatusCode(500, new { error = "An error occurred while deleting the issue" });
            }
        }

        [HttpPost("{id}/resolve")]
        [ProducesResponseType(typeof(ProjectIssueDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ResolveIssue(Guid id, [FromBody] ResolveIssueCommand command)
        {
            try
            {
                command.Id = id;
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving issue with ID: {IssueId}", id);
                return StatusCode(500, new { error = "An error occurred while resolving the issue" });
            }
        }

        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(ProjectIssueDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateIssueStatus(Guid id, [FromBody] UpdateIssueStatusCommand command)
        {
            try
            {
                command.Id = id;
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating issue status with ID: {IssueId}", id);
                return StatusCode(500, new { error = "An error occurred while updating issue status" });
            }
        }
    }
}