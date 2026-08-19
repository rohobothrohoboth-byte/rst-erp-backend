// Controllers/DocumentController.cs
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Commands.DocumentCommands;
using Cor.ProjectManagement.Queries.DocumentQueries;

namespace Cor.ProjectManagement.Controllers
{
   [ApiController]
   [Route("api/projectmanagement/v{version:apiVersion}/[controller]")]
   [ApiVersion("1.0")]
   [Authorize]
    public class DocumentController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DocumentController> _logger;

        public DocumentController(IMediator mediator, ILogger<DocumentController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProjectDocumentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDocument(Guid id)
        {
            try
            {
                var result = await _mediator.Send(new GetDocumentByIdQuery { Id = id });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting document with ID: {DocumentId}", id);
                return StatusCode(500, new { error = "An error occurred while retrieving the document" });
            }
        }

        [HttpGet("project/{projectId}")]
        [ProducesResponseType(typeof(List<ProjectDocumentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDocumentsByProject(Guid projectId, [FromQuery] GetDocumentsByProjectQuery query)
        {
            try
            {
                query.ProjectId = projectId;
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting documents for project ID: {ProjectId}", projectId);
                return StatusCode(500, new { error = "An error occurred while retrieving documents" });
            }
        }

        [HttpGet("{id}/versions")]
        [ProducesResponseType(typeof(List<ProjectDocumentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDocumentVersions(Guid id)
        {
            try
            {
                var result = await _mediator.Send(new GetDocumentVersionsQuery { DocumentId = id });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting document versions for ID: {DocumentId}", id);
                return StatusCode(500, new { error = "An error occurred while retrieving document versions" });
            }
        }

        [HttpPost("upload")]
        [ProducesResponseType(typeof(ProjectDocumentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadDocument([FromBody] UploadDocumentCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetDocument), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading document");
                return StatusCode(500, new { error = "An error occurred while uploading the document" });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProjectDocumentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateDocument(Guid id, [FromBody] UpdateDocumentCommand command)
        {
            try
            {
                command.Id = id;
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating document with ID: {DocumentId}", id);
                return StatusCode(500, new { error = "An error occurred while updating the document" });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDocument(Guid id, [FromQuery] string? deletedBy = null)
        {
            try
            {
                await _mediator.Send(new DeleteDocumentCommand { Id = id, DeletedBy = deletedBy });
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting document with ID: {DocumentId}", id);
                return StatusCode(500, new { error = "An error occurred while deleting the document" });
            }
        }

        [HttpPost("{id}/approve")]
        [ProducesResponseType(typeof(ProjectDocumentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ApproveDocument(Guid id, [FromBody] ApproveDocumentCommand command)
        {
            try
            {
                command.Id = id;
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving document with ID: {DocumentId}", id);
                return StatusCode(500, new { error = "An error occurred while approving the document" });
            }
        }

        [HttpPost("{id}/archive")]
        [ProducesResponseType(typeof(ProjectDocumentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ArchiveDocument(Guid id, [FromBody] ArchiveDocumentCommand command)
        {
            try
            {
                command.Id = id;
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error archiving document with ID: {DocumentId}", id);
                return StatusCode(500, new { error = "An error occurred while archiving the document" });
            }
        }
    }
}