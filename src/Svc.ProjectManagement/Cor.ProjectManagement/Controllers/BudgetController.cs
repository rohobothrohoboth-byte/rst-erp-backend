// Controllers/BudgetController.cs
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Commands.BudgetCommands;
using Cor.ProjectManagement.Queries.BudgetQueries;

namespace Cor.ProjectManagement.Controllers
{
   [ApiController]
   [Route("api/projectmanagement/v{version:apiVersion}/[controller]")]
   [ApiVersion("1.0")]
   [Authorize]
    public class BudgetController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BudgetController> _logger;

        public BudgetController(IMediator mediator, ILogger<BudgetController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Get budget by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProjectBudgetDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBudget(Guid id)
        {
            try
            {
                var result = await _mediator.Send(new GetBudgetByIdQuery { Id = id });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting budget with ID: {BudgetId}", id);
                return StatusCode(500, new { error = "An error occurred while retrieving the budget" });
            }
        }

        /// <summary>
        /// Get budgets by project
        /// </summary>
        [HttpGet("project/{projectId}")]
        [ProducesResponseType(typeof(List<ProjectBudgetDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBudgetsByProject(Guid projectId, [FromQuery] GetBudgetsByProjectQuery query)
        {
            try
            {
                query.ProjectId = projectId;
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting budgets for project ID: {ProjectId}", projectId);
                return StatusCode(500, new { error = "An error occurred while retrieving budgets" });
            }
        }

        /// <summary>
        /// Get budget summary
        /// </summary>
        [HttpGet("project/{projectId}/summary")]
        [ProducesResponseType(typeof(BudgetSummaryDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBudgetSummary(Guid projectId)
        {
            try
            {
                var result = await _mediator.Send(new GetBudgetSummaryQuery { ProjectId = projectId });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting budget summary for project ID: {ProjectId}", projectId);
                return StatusCode(500, new { error = "An error occurred while retrieving budget summary" });
            }
        }

        /// <summary>
        /// Get budget utilization
        /// </summary>
        [HttpGet("project/{projectId}/utilization")]
        [ProducesResponseType(typeof(BudgetUtilizationDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBudgetUtilization(Guid projectId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            try
            {
                var result = await _mediator.Send(new GetBudgetUtilizationQuery
                {
                    ProjectId = projectId,
                    FromDate = fromDate,
                    ToDate = toDate
                });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting budget utilization for project ID: {ProjectId}", projectId);
                return StatusCode(500, new { error = "An error occurred while retrieving budget utilization" });
            }
        }

        /// <summary>
        /// Create a new budget
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ProjectBudgetDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateBudget([FromBody] CreateBudgetCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetBudget), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating budget");
                return StatusCode(500, new { error = "An error occurred while creating the budget" });
            }
        }

        /// <summary>
        /// Update an existing budget
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProjectBudgetDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateBudget(Guid id, [FromBody] UpdateBudgetCommand command)
        {
            try
            {
                command.Id = id;
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating budget with ID: {BudgetId}", id);
                return StatusCode(500, new { error = "An error occurred while updating the budget" });
            }
        }

        /// <summary>
        /// Delete a budget
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBudget(Guid id, [FromQuery] string? deletedBy = null)
        {
            try
            {
                await _mediator.Send(new DeleteBudgetCommand { Id = id, DeletedBy = deletedBy });
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting budget with ID: {BudgetId}", id);
                return StatusCode(500, new { error = "An error occurred while deleting the budget" });
            }
        }

        /// <summary>
        /// Approve a budget
        /// </summary>
        [HttpPost("{id}/approve")]
        [ProducesResponseType(typeof(ProjectBudgetDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ApproveBudget(Guid id, [FromBody] ApproveBudgetCommand command)
        {
            try
            {
                command.Id = id;
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving budget with ID: {BudgetId}", id);
                return StatusCode(500, new { error = "An error occurred while approving the budget" });
            }
        }
    }
}