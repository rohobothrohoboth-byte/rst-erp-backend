// Controllers/ProjectController.cs
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Commands.ProjectCommands;
using Cor.ProjectManagement.Queries.ProjectQueries;

namespace Cor.ProjectManagement.Controllers
{
    [ApiController]
    [Route("api/projectmanagement/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class ProjectController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProjectController> _logger;

        public ProjectController(IMediator mediator, ILogger<ProjectController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Get all projects with filtering and pagination
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResponse<ProjectDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProjects([FromQuery] GetProjectsQuery query)
        {
            try
            {
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting projects");
                return StatusCode(500, new { error = "An error occurred while retrieving projects" });
            }
        }

        /// <summary>
        /// Get project by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProject(Guid id)
        {
            try
            {
                var result = await _mediator.Send(new GetProjectByIdQuery { Id = id });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting project with ID: {ProjectId}", id);
                return StatusCode(500, new { error = "An error occurred while retrieving the project" });
            }
        }

        /// <summary>
        /// Create a new project
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetProject), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating project");
                return StatusCode(500, new { error = "An error occurred while creating the project" });
            }
        }

        /// <summary>
        /// Update an existing project
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProject(Guid id, [FromBody] UpdateProjectCommand command)
        {
            try
            {
                command.Id = id;
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating project with ID: {ProjectId}", id);
                return StatusCode(500, new { error = "An error occurred while updating the project" });
            }
        }

        /// <summary>
        /// Delete a project (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProject(Guid id, [FromQuery] string? deletedBy = null)
        {
            try
            {
                await _mediator.Send(new DeleteProjectCommand { Id = id, DeletedBy = deletedBy });
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting project with ID: {ProjectId}", id);
                return StatusCode(500, new { error = "An error occurred while deleting the project" });
            }
        }

        /// <summary>
        /// Change project status
        /// </summary>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeProjectStatusCommand command)
        {
            try
            {
                command.Id = id;
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing status for project with ID: {ProjectId}", id);
                return StatusCode(500, new { error = "An error occurred while changing project status" });
            }
        }

        /// <summary>
        /// Get project dashboard
        /// </summary>
        [HttpGet("dashboard")]
        [ProducesResponseType(typeof(ProjectDashboardDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDashboard([FromQuery] GetProjectDashboardQuery query)
        {
            try
            {
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard");
                return StatusCode(500, new { error = "An error occurred while retrieving dashboard data" });
            }
        }

        /// <summary>
        /// Get project statistics
        /// </summary>
        [HttpGet("{id}/statistics")]
        [ProducesResponseType(typeof(ProjectStatisticsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStatistics(Guid id)
        {
            try
            {
                var result = await _mediator.Send(new GetProjectStatisticsQuery { ProjectId = id });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting statistics for project ID: {ProjectId}", id);
                return StatusCode(500, new { error = "An error occurred while retrieving project statistics" });
            }
        }
    }
}