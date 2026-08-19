// Controllers/NotificationController.cs
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Commands.NotificationCommands;
using Cor.ProjectManagement.Queries.NotificationQueries;

namespace Cor.ProjectManagement.Controllers
{
   [ApiController]
   [Route("api/projectmanagement/v{version:apiVersion}/[controller]")]
   [ApiVersion("1.0")]
   [Authorize]
    public class NotificationController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(IMediator mediator, ILogger<NotificationController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProjectNotificationDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNotification(Guid id, [FromQuery] Guid userId)
        {
            try
            {
                var result = await _mediator.Send(new GetNotificationByIdQuery { Id = id, UserId = userId });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notification {NotificationId} for user {UserId}", id, userId);
                return StatusCode(500, new { error = "An error occurred while retrieving the notification" });
            }
        }

        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(PaginatedResponse<ProjectNotificationDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserNotifications(Guid userId, [FromQuery] GetUserNotificationsQuery query)
        {
            try
            {
                query.UserId = userId;
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications for user {UserId}", userId);
                return StatusCode(500, new { error = "An error occurred while retrieving notifications" });
            }
        }

        [HttpGet("user/{userId}/unread-count")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUnreadCount(Guid userId)
        {
            try
            {
                var result = await _mediator.Send(new GetUnreadCountQuery { UserId = userId });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread count for user {UserId}", userId);
                return StatusCode(500, new { error = "An error occurred while getting unread count" });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProjectNotificationDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetNotification), new { id = result.Id, userId = result.UserId }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification");
                return StatusCode(500, new { error = "An error occurred while creating the notification" });
            }
        }

        [HttpPost("mark-read")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> MarkNotificationsRead([FromBody] MarkNotificationReadCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notifications as read for user {UserId}", command.UserId);
                return StatusCode(500, new { error = "An error occurred while marking notifications as read" });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteNotification(Guid id, [FromQuery] Guid userId)
        {
            try
            {
                await _mediator.Send(new DeleteNotificationCommand { Id = id, UserId = userId });
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification {NotificationId} for user {UserId}", id, userId);
                return StatusCode(500, new { error = "An error occurred while deleting the notification" });
            }
        }

        [HttpDelete("user/{userId}/all")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteAllNotifications(Guid userId)
        {
            try
            {
                await _mediator.Send(new DeleteAllNotificationsCommand { UserId = userId });
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting all notifications for user {UserId}", userId);
                return StatusCode(500, new { error = "An error occurred while deleting notifications" });
            }
        }
    }
}