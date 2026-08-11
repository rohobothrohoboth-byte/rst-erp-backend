// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Controllers\FolderController.cs

using Asp.Versioning;
using Common;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Cor.FileManagement.Models.DTOs;
using Cor.FileManagement.Commands;
using Cor.FileManagement.Queries;

namespace Cor.FileManagement.Controllers;

[ApiController]
[Route("api/file/v{version:apiVersion}/folders")]
[ApiVersion("1.0")]
[Authorize]
public class FolderController : ControllerBase
{
    private readonly IMediator _mediator;

    public FolderController(IMediator mediator)
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

    // ✅ GET /api/file/v1/folders/root - Get root folders
    [PerAuth("flm.company.folders.view")]
    [HttpGet("root")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRootFolders()
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new GetRootFoldersQuery { UserId = userId });
        return Ok(ApiResponse<object>.Ok(result));
    }

    // ✅ GET /api/file/v1/folders - Get all folders
    [PerAuth("flm.company.folders.view")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFolders([FromQuery] GetFoldersQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<object>.Ok(result));
    }

    // ✅ GET /api/file/v1/folders/{id} - Get folder by ID
    [PerAuth("flm.company.folders.view")]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFolderById(Guid id)
    {
        var result = await _mediator.Send(new GetFolderByIdQuery { Id = id });
        return Ok(ApiResponse<object>.Ok(result));
    }

    // ✅ GET /api/file/v1/folders/{id}/contents - Get folder contents (sub-folders and documents)
    [PerAuth("flm.company.folders.view")]
    [HttpGet("{id}/contents")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFolderContents(Guid id)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new GetFolderContentsQuery { FolderId = id, UserId = userId });
        return Ok(ApiResponse<object>.Ok(result));
    }

    // ✅ GET /api/file/v1/folders/{id}/subfolders - Get sub-folders only
    [PerAuth("flm.company.folders.view")]
    [HttpGet("{id}/subfolders")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSubFolders(Guid id)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new GetSubFoldersQuery { FolderId = id, UserId = userId });
        return Ok(ApiResponse<object>.Ok(result));
    }

    // ✅ GET /api/file/v1/folders/{id}/documents - Get documents only
    [PerAuth("flm.company.folders.view")]
    [HttpGet("{id}/documents")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFolderDocuments(Guid id)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new GetFolderDocumentsQuery { FolderId = id, UserId = userId });
        return Ok(ApiResponse<object>.Ok(result));
    }

    // ✅ POST /api/file/v1/folders - Create folder
    [PerAuth("flm.company.folders.create")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateFolder([FromBody] CreateFolderCommand command)
    {
        var userId = GetCurrentUserId();
        command.CreatedBy = userId;
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<object>.Ok(result, "Folder created successfully"));
    }

    // ✅ PUT /api/file/v1/folders/{id} - Update folder
    [PerAuth("flm.company.folders.mod")]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateFolder(Guid id, [FromBody] UpdateFolderCommand command)
    {
        var userId = GetCurrentUserId();
        command.Id = id;
        command.ModifiedBy = userId;
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<object>.Ok(result, "Folder updated successfully"));
    }

    // ✅ DELETE /api/file/v1/folders/{id} - Delete folder
    [PerAuth("flm.company.folders.del")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFolder(Guid id)
    {
        var userId = GetCurrentUserId();

        var command = new DeleteFolderCommand
        {
            Id = id,
            DeletedBy = userId
        };

        var result = await _mediator.Send(command);
        return Ok(ApiResponse<object>.Ok(result, "Folder deleted successfully"));
    }

    // ✅ POST /api/file/v1/folders/{id}/move - Move folder
    [PerAuth("flm.company.folders.move")]
    [HttpPost("{id}/move")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MoveFolder(Guid id, [FromBody] MoveFolderCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<object>.Ok(result, "Folder moved successfully"));
    }

    // ✅ POST /api/file/v1/folders/{id}/share - Share a folder
    [PerAuth("flm.company.folders.mod")]
    [HttpPost("{id}/share")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ShareFolder(Guid id, [FromBody] FolderShareCreateDto dto)
    {
        var userId = GetCurrentUserId();

        var command = new ShareFolderCommand
        {
            FolderId = id,
            SharedWithId = dto.SharedWithId,
            SharedWithType = dto.SharedWithType,
            Permission = dto.Permission,
            CanDownload = dto.CanDownload,
            CanDelete = dto.CanDelete,
            ExpiresAt = dto.ExpiresAt,
            SharedBy = userId
        };

        var result = await _mediator.Send(command);
        return Ok(ApiResponse<object>.Ok(result, "Folder shared successfully"));
    }

    // ✅ GET /api/file/v1/folders/{id}/shares - Get folder shares
    [PerAuth("flm.company.folders.view")]
    [HttpGet("{id}/shares")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFolderShares(Guid id)
    {
        var query = new GetFolderSharesQuery { FolderId = id };
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<object>.Ok(result));
    }

    // ✅ DELETE /api/file/v1/folders/{id}/share/{shareId} - Remove folder share
    [PerAuth("flm.company.folders.mod")]
    [HttpDelete("{id}/share/{shareId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveFolderShare(Guid id, Guid shareId)
    {
        var userId = GetCurrentUserId();

        var command = new RemoveFolderShareCommand
        {
            FolderId = id,
            ShareId = shareId,
            RemovedBy = userId
        };

        var result = await _mediator.Send(command);
        return Ok(ApiResponse<object>.Ok(result, "Share removed successfully"));
    }
}