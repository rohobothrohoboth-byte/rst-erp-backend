using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Cor.PlanDev.Controllers;

[ApiController]
[Authorize]
public abstract class BaseApiController : ControllerBase
{
    protected readonly ILogger _logger;

    protected BaseApiController(ILogger logger)
    {
        _logger = logger;
    }

    protected BaseApiController()
    {
        _logger = null!;
    }

    protected IActionResult HandleException(Exception ex, string actionName)
    {
        _logger?.LogError(ex, "Error in {ActionName}: {Message}", actionName, ex.Message);

        return ex switch
        {
            InvalidOperationException => BadRequest(new { message = ex.Message }),
            KeyNotFoundException => NotFound(new { message = ex.Message }),
            ArgumentException => BadRequest(new { message = ex.Message }),
            UnauthorizedAccessException => Unauthorized(new { message = "Unauthorized access" }),
            _ => StatusCode(500, new { message = "An unexpected error occurred" })
        };
    }
}