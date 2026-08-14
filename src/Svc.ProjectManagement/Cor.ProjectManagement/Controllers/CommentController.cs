// Controllers/CommentController.cs
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Commands.CommentCommands;
using Cor.ProjectManagement.Queries.CommentQueries;

namespace Cor.ProjectManagement.Controllers
{
    [ApiController]
    [Route("api/projectmanagement/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class CommentController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CommentController> _logger;

        public CommentController(IMediator mediator, ILogger<CommentController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProjectCommentDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetComment(Guid id)
        {
            try
            {
                var result = await _mediator.Send(new GetCommentByIdQuery { Id = id });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comment with ID: {CommentId}", id);
                return StatusCode(500, new { error = "An error occurred while retrieving the comment" });
            }
        }

        [HttpGet("project/{projectId}")]
        [ProducesResponseType(typeof(List<ProjectCommentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCommentsByProject(Guid projectId, [FromQuery] GetCommentsByProjectQuery query)
        {
            try
            {
                query.ProjectId = projectId;
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comments for project ID: {ProjectId}", projectId);
                return StatusCode(500, new { error = "An error occurred while retrieving comments" });
            }
        }

        [HttpGet("thread/{commentId}")]
        [ProducesResponseType(typeof(List<ProjectCommentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCommentThread(Guid commentId)
        {
            try
            {
                var result = await _mediator.Send(new GetCommentThreadQuery { CommentId = commentId });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comment thread for ID: {CommentId}", commentId);
                return StatusCode(500, new { error = "An error occurred while retrieving comment thread" });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProjectCommentDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetComment), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating comment");
                return StatusCode(500, new { error = "An error occurred while creating the comment" });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProjectCommentDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateComment(Guid id, [FromBody] UpdateCommentCommand command)
        {
            try
            {
                command.Id = id;
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating comment with ID: {CommentId}", id);
                return StatusCode(500, new { error = "An error occurred while updating the comment" });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteComment(Guid id, [FromQuery] string? deletedBy = null)
        {
            try
            {
                await _mediator.Send(new DeleteCommentCommand { Id = id, DeletedBy = deletedBy });
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting comment with ID: {CommentId}", id);
                return StatusCode(500, new { error = "An error occurred while deleting the comment" });
            }
        }

        [HttpPatch("{id}/pin")]
        [ProducesResponseType(typeof(ProjectCommentDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> PinComment(Guid id, [FromBody] PinCommentCommand command)
        {
            try
            {
                command.Id = id;
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error pinning comment with ID: {CommentId}", id);
                return StatusCode(500, new { error = "An error occurred while pinning the comment" });
            }
        }

        [HttpPatch("{id}/resolve")]
        [ProducesResponseType(typeof(ProjectCommentDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> ResolveComment(Guid id, [FromBody] ResolveCommentCommand command)
        {
            try
            {
                command.Id = id;
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving comment with ID: {CommentId}", id);
                return StatusCode(500, new { error = "An error occurred while resolving the comment" });
            }
        }
    }
}