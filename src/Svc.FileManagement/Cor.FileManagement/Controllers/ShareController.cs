// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Controllers\ShareController.cs

using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Cor.FileManagement.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Cor.FileManagement.Commands;
using Cor.FileManagement.Queries;
namespace Cor.FileManagement.Controllers;

[ApiController]
[Route("api/file/v{version:apiVersion}/share")]
[ApiVersion("1.0")]
[Authorize]
public class ShareController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ShareController(IMediator mediator, IHttpContextAccessor httpContextAccessor)
    {
        _mediator = mediator;
        _httpContextAccessor = httpContextAccessor;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("userId")?.Value
            ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            throw new UnauthorizedAccessException("User not authenticated");

        return Guid.Parse(userIdClaim);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ShareFile([FromBody] FileShareCreateDto dto)
    {
        var userId = GetCurrentUserId();

        var command = new ShareFileCommand
        {
            DocumentId = dto.DocumentId,
            FolderId = dto.FolderId,
            SharedWithId = dto.SharedWithId,
            SharedWithType = dto.SharedWithType,
            Permission = dto.Permission,
            CanDownload = dto.CanDownload,
            CanDelete = dto.CanDelete,
            ExpiresAt = dto.ExpiresAt,
            SharedBy = userId
        };

        var result = await _mediator.Send(command);
        return Ok(ApiResponse<object>.Ok(result, "File shared successfully"));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateShare(Guid id, [FromBody] FileShareCreateDto dto)
    {
        var userId = GetCurrentUserId();

        var command = new UpdateShareCommand
        {
            Id = id,
            Permission = dto.Permission,
            CanDownload = dto.CanDownload,
            CanDelete = dto.CanDelete,
            ExpiresAt = dto.ExpiresAt,
            ModifiedBy = userId
        };

        var result = await _mediator.Send(command);
        return Ok(ApiResponse<object>.Ok(result, "Share updated successfully"));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveShare(Guid id)
    {
        var userId = GetCurrentUserId();

        var command = new RemoveShareCommand
        {
            Id = id,
            RemovedBy = userId
        };

        var result = await _mediator.Send(command);
        return Ok(ApiResponse<object>.Ok(result, "Share removed successfully"));
    }

    [HttpGet("file/{documentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFileShares(Guid documentId)
    {
        var query = new GetFileSharesQuery
        {
            DocumentId = documentId
        };

        var result = await _mediator.Send(query);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserShares(Guid userId)
    {
        var query = new GetFileSharesByUserQuery
        {
            UserId = userId,
            ActiveOnly = true
        };

        var result = await _mediator.Send(query);
        return Ok(ApiResponse<object>.Ok(result));
    }

    // ✅ GET /api/file/v1/share/shared-with-me - Get files shared with current user
    [HttpGet("shared-with-me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSharedWithMe()
    {
        var userId = GetCurrentUserId();
        var query = new GetSharedWithMeQuery { UserId = userId };
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<object>.Ok(result));
    }

    // ✅ GET /api/file/v1/share/my-shares - Get files shared by current user
    [HttpGet("my-shares")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyShares()
    {
        var userId = GetCurrentUserId();
        var query = new GetMySharesQuery { UserId = userId };
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<object>.Ok(result));
    }

    // ✅ GET /api/file/v1/share/folder/{folderId} - Get shares for a folder
    [HttpGet("folder/{folderId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFolderShares(Guid folderId)
    {
        var query = new GetFolderSharesQuery { FolderId = folderId };
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<object>.Ok(result));
    }
}