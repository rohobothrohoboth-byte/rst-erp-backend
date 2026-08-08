// Controllers/BaseApiController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Cor.Procurement.Controllers;

[ApiController]
[Authorize]
public abstract class BaseApiController : ControllerBase
{
    protected readonly ILogger _logger;

    // ✅ Constructor with single ILogger parameter
    protected BaseApiController(ILogger logger)
    {
        _logger = logger;
    }

    // ✅ Alternative: Constructor with no parameters (for controllers that don't need logger)
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
            _ => StatusCode(500, new { message = "An unexpected error occurred" })
        };
    }
}