// Controllers/TaskController.cs
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Commands.TaskCommands;
using Cor.ProjectManagement.Queries.TaskQueries;

namespace Cor.ProjectManagement.Controllers
{
   [ApiController]
   [Route("api/projectmanagement/v{version:apiVersion}/[controller]")]
   [ApiVersion("1.0")]
   [Authorize]
    public class TaskController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TaskController> _logger;

        public TaskController(IMediator mediator, ILogger<TaskController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Get task by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProjectTaskDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTask(Guid id)
        {
            try
            {
                var result = await _mediator.Send(new GetTaskByIdQuery { Id = id });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting task with ID: {TaskId}", id);
                return StatusCode(500, new { error = "An error occurred while retrieving the task" });
            }
        }

        /// <summary>
        /// Get tasks by project
        /// </summary>
        [HttpGet("project/{projectId}")]
        [ProducesResponseType(typeof(PaginatedResponse<ProjectTaskDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTasksByProject(Guid projectId, [FromQuery] GetTasksByProjectQuery query)
        {
            try
            {
                query.ProjectId = projectId;
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tasks for project ID: {ProjectId}", projectId);
                return StatusCode(500, new { error = "An error occurred while retrieving tasks" });
            }
        }

        /// <summary>
        /// Get tasks by assignee
        /// </summary>
        [HttpGet("assignee/{assigneeId}")]
        [ProducesResponseType(typeof(PaginatedResponse<ProjectTaskDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTasksByAssignee(Guid assigneeId, [FromQuery] GetTasksByAssigneeQuery query)
        {
            try
            {
                query.AssigneeId = assigneeId;
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tasks for assignee ID: {AssigneeId}", assigneeId);
                return StatusCode(500, new { error = "An error occurred while retrieving tasks" });
            }
        }

        /// <summary>
        /// Get task tree (hierarchy)
        /// </summary>
        [HttpGet("project/{projectId}/tree")]
        [ProducesResponseType(typeof(List<ProjectTaskDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTaskTree(Guid projectId)
        {
            try
            {
                var result = await _mediator.Send(new GetTaskTreeQuery { ProjectId = projectId });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting task tree for project ID: {ProjectId}", projectId);
                return StatusCode(500, new { error = "An error occurred while retrieving task tree" });
            }
        }

        /// <summary>
        /// Create a new task
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ProjectTaskDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetTask), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                return StatusCode(500, new { error = "An error occurred while creating the task" });
            }
        }

        /// <summary>
        /// Update an existing task
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProjectTaskDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskCommand command)
        {
            try
            {
                command.Id = id;
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task with ID: {TaskId}", id);
                return StatusCode(500, new { error = "An error occurred while updating the task" });
            }
        }

        /// <summary>
        /// Delete a task
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTask(Guid id, [FromQuery] string? deletedBy = null)
        {
            try
            {
                await _mediator.Send(new DeleteTaskCommand { Id = id, DeletedBy = deletedBy });
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task with ID: {TaskId}", id);
                return StatusCode(500, new { error = "An error occurred while deleting the task" });
            }
        }

        /// <summary>
        /// Assign task to a user
        /// </summary>
        [HttpPatch("{id}/assign")]
        [ProducesResponseType(typeof(ProjectTaskDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignTask(Guid id, [FromBody] AssignTaskCommand command)
        {
            try
            {
                command.TaskId = id;
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning task with ID: {TaskId}", id);
                return StatusCode(500, new { error = "An error occurred while assigning the task" });
            }
        }

        /// <summary>
        /// Update task status
        /// </summary>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(ProjectTaskDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateTaskStatusCommand command)
        {
            try
            {
                command.TaskId = id;
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for task with ID: {TaskId}", id);
                return StatusCode(500, new { error = "An error occurred while updating task status" });
            }
        }
    }
}