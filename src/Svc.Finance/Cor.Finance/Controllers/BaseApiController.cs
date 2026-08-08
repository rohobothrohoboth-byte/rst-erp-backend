using Cor.Finance.Commands;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Cor.Finance.Helpers;
using System.Security.Claims; // Add this for ClaimTypes
namespace Cor.Finance.Controllers;

[ApiController]
[Authorize]
[Route("api/finance/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public abstract class BaseApiController : ControllerBase
{
    private IMediator? _mediator;
    private ILogger? _logger;

    // Default constructor for DI
    protected BaseApiController()
    {
    }

    // Constructor with parameters (optional)
    protected BaseApiController(IMediator mediator, ILogger logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>()!;
    protected ILogger Logger => _logger ??= HttpContext.RequestServices.GetService<ILogger<BaseApiController>>()!;

    #region Helper Methods

    /// <summary>
    /// Handles 404 Not Found responses
    /// </summary>
    protected IActionResult HandleNotFound(string entityName, object id)
    {
        return NotFound(new {
            message = $"{entityName} with ID '{id}' not found",
            entity = entityName,
            id = id
        });
    }
// Add this method to BaseApiController.cs

    /// <summary>
    /// Handles 400 Bad Request responses
    /// </summary>
    protected IActionResult HandleBadRequest(string message)
    {
        return BadRequest(new {
            message = message,
            status = 400
        });
    }

    /// <summary>
    /// Handles exceptions and returns appropriate status codes
    /// </summary>
    protected IActionResult HandleException(Exception ex, string action, object? id = null)
    {
        Logger.LogError(ex, "Error {Action} for {Entity} with ID {Id}", action, GetType().Name, id);

        // Handle specific exception types
        return ex switch
        {
            InvalidOperationException when ex.Message.Contains("not found") || ex.Message.Contains("does not exist") =>
                NotFound(new { message = ex.Message }),

            InvalidOperationException when ex.Message.Contains("already exists") || ex.Message.Contains("duplicate") =>
                Conflict(new { message = ex.Message }),

            InvalidOperationException when ex.Message.Contains("invalid") || ex.Message.Contains("Invalid") =>
                BadRequest(new { message = ex.Message }),

            ArgumentException or ArgumentNullException =>
                BadRequest(new { message = ex.Message }),

            UnauthorizedAccessException =>
                Unauthorized(new { message = "You are not authorized to perform this action" }),

            _ => StatusCode(500, new {
                message = "An unexpected error occurred. Please try again later.",
                error = ex.Message // Remove this in production
            })
        };
    }

    /// <summary>
    /// Creates a 201 Created response
    /// </summary>
    protected IActionResult CreateResponse<T>(T result, string actionName, object routeValues)
    {
        return CreatedAtAction(actionName, routeValues, result);
    }

    /// <summary>
    /// Returns a 200 OK success response
    /// </summary>
    protected IActionResult SuccessResponse(string message, object? data = null)
    {
        return Ok(new {
            success = true,
            message = message,
            data = data
        });
    }

    /// <summary>
    /// Returns a 200 OK response with data
    /// </summary>
    protected IActionResult SuccessResponse<T>(T data, string? message = null)
    {
        return Ok(new {
            success = true,
            message = message ?? "Operation completed successfully",
            data = data
        });
    }

    /// <summary>
    /// Returns a 200 OK response with paginated data
    /// </summary>
    protected IActionResult PaginatedResponse<T>(IEnumerable<T> data, int totalCount, int pageNumber, int pageSize)
    {
        return Ok(new
        {
            success = true,
            data = data,
            pagination = new
            {
                totalCount,
                pageNumber,
                pageSize,
                totalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                hasNextPage = pageNumber < (int)Math.Ceiling((double)totalCount / pageSize),
                hasPreviousPage = pageNumber > 1
            }
        });
    }

    /// <summary>
    /// Ensures a DateTime is in UTC format
    /// </summary>
    protected static DateTime EnsureUtc(DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Unspecified)
        {
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }
        return dateTime.ToUniversalTime();
    }

    /// <summary>
    /// Ensures a nullable DateTime is in UTC format
    /// </summary>
    protected static DateTime? EnsureUtc(DateTime? dateTime)
    {
        if (!dateTime.HasValue)
            return null;
        return EnsureUtc(dateTime.Value);
    }

    /// <summary>
    /// Validates that a date range is valid
    /// </summary>
    protected bool IsValidDateRange(DateTime startDate, DateTime endDate)
    {
        return startDate <= endDate;
    }

    /// <summary>
    /// Validates that a date is not in the future
    /// </summary>
    protected bool IsValidDate(DateTime date)
    {
        return date <= DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the current user ID from the claims
    /// </summary>
    protected string? GetCurrentUserId()
    {
        return User?.FindFirst("sub")?.Value ?? User?.FindFirst("userId")?.Value;
    }

    /// <summary>
    /// Gets the current user's email from the claims
    /// </summary>
    protected string? GetCurrentUserEmail()
    {
        return User?.FindFirst("email")?.Value ?? User?.FindFirst(ClaimTypes.Email)?.Value;
    }

    /// <summary>
    /// Gets the current user's role from the claims
    /// </summary>
    protected string? GetCurrentUserRole()
    {
        return User?.FindFirst(ClaimTypes.Role)?.Value;
    }

    #endregion
}