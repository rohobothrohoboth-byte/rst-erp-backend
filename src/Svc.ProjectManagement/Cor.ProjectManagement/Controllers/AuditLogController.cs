// Controllers/AuditLogController.cs
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Queries.AuditLogQueries;

namespace Cor.ProjectManagement.Controllers
{
   [ApiController]
   [Route("api/projectmanagement/v{version:apiVersion}/[controller]")]
   [ApiVersion("1.0")]
   [Authorize]
    public class AuditLogController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuditLogController> _logger;

        public AuditLogController(IMediator mediator, ILogger<AuditLogController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProjectAuditLogDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAuditLog(Guid id)
        {
            try
            {
                var result = await _mediator.Send(new GetAuditLogByIdQuery { Id = id });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting audit log with ID: {AuditLogId}", id);
                return StatusCode(500, new { error = "An error occurred while retrieving the audit log" });
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResponse<ProjectAuditLogDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAuditLogs([FromQuery] GetAuditLogsQuery query)
        {
            try
            {
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting audit logs");
                return StatusCode(500, new { error = "An error occurred while retrieving audit logs" });
            }
        }

        [HttpGet("project/{projectId}/summary")]
        [ProducesResponseType(typeof(AuditLogSummaryDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAuditLogSummary(Guid projectId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            try
            {
                var result = await _mediator.Send(new GetAuditLogSummaryQuery
                {
                    ProjectId = projectId,
                    FromDate = fromDate,
                    ToDate = toDate
                });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting audit log summary for project ID: {ProjectId}", projectId);
                return StatusCode(500, new { error = "An error occurred while retrieving audit log summary" });
            }
        }
    }
}