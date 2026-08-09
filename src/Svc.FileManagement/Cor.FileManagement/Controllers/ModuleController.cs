// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Controllers\ModuleController.cs

using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Cor.FileManagement.Commands;
using Cor.FileManagement.Queries;

namespace Cor.FileManagement.Controllers;

[ApiController]
[Route("api/file/v{version:apiVersion}/module")]
[ApiVersion("1.0")]
[Authorize]
public class ModuleController : ControllerBase
{
    private readonly IMediator _mediator;

    public ModuleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User?.FindFirst("userId")?.Value
            ?? User?.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            throw new UnauthorizedAccessException("User not authenticated");

        return Guid.Parse(userIdClaim);
    }

    // ✅ GET /api/file/v1/module/{module}/reference/{referenceId}
    // Get documents by module and reference (e.g., for invoice attachments)
    [HttpGet("{module}/reference/{referenceId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDocumentsByModuleAndReference(string module, Guid referenceId, [FromQuery] string? category = null)
    {
        var query = new GetDocumentsByModuleAndReferenceQuery
        {
            Module = module,
            ReferenceId = referenceId,
            Category = category
        };

        var result = await _mediator.Send(query);
        return Ok(ApiResponse<object>.Ok(result));
    }

    // ✅ GET /api/file/v1/module/{module}
    // Get documents by module only
    [HttpGet("{module}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDocumentsByModule(string module, [FromQuery] string? category = null)
    {
        // ✅ Special handling for favorites
        if (module?.ToLower() == "favorites")
        {
            var userId = GetCurrentUserId();
            var query = new GetFavoriteDocumentsQuery { UserId = userId };
            var result = await _mediator.Send(query);
            return Ok(ApiResponse<object>.Ok(result));
        }

        var query2 = new GetDocumentsByModuleQuery
        {
            Module = module ?? string.Empty,
            Category = category
        };

        var result2 = await _mediator.Send(query2);
        return Ok(ApiResponse<object>.Ok(result2));
    }
}