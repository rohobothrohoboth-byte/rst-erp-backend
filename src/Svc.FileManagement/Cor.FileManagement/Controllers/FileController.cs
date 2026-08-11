// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Controllers\DocumentController.cs

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
[Route("api/file/v{version:apiVersion}/documents")]
[ApiVersion("1.0")]
[Authorize]
public class DocumentController : ControllerBase
{
    private readonly IMediator _mediator;

    public DocumentController(IMediator mediator)
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

    private string GetCurrentUserIdAsString()
    {
        var userIdClaim = User?.FindFirst("userId")?.Value
            ?? User?.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            throw new UnauthorizedAccessException("User not authenticated");

        return userIdClaim;
    }

    // ✅ GET /api/file/v1/documents/all - Get all documents with filtering
    [PerAuth("flm.company.view")]
    [HttpGet("all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllDocuments([FromQuery] GetDocumentsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<object>.Ok(result));
    }

    // ✅ GET /api/file/v1/documents/favorites - Get favorite documents
    [PerAuth("flm.company.view")]
    [HttpGet("favorites")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFavorites()
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new GetFavoriteDocumentsQuery { UserId = userId });
        return Ok(ApiResponse<object>.Ok(result));
    }

    // ✅ GET /api/file/v1/documents/recent - Get recent documents
    [PerAuth("flm.company.view")]
    [HttpGet("recent")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecent([FromQuery] int limit = 10)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new GetRecentDocumentsQuery { UserId = userId, Limit = limit });
        return Ok(ApiResponse<object>.Ok(result));
    }

    // ✅ GET /api/file/v1/documents/archived - Get archived documents
    [PerAuth("flm.company.view")]
    [HttpGet("archived")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetArchived([FromQuery] string? search = null)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new GetArchivedDocumentsQuery { UserId = userId, SearchTerm = search });
        return Ok(ApiResponse<object>.Ok(result));
    }

    // ✅ GET /api/file/v1/documents/{id} - Get document by ID
    [PerAuth("flm.company.view")]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDocumentById(Guid id)
    {
        var result = await _mediator.Send(new GetDocumentByIdQuery { Id = id });
        return Ok(ApiResponse<object>.Ok(result));
    }

    // In DocumentController.cs - UploadDocument method

    [PerAuth("flm.company.manage")]
    [HttpPost("upload")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadDocument([FromForm] DocumentUploadRequestDto dto)
    {
        try
        {
            Console.WriteLine("=== Upload Request Started ===");
            Console.WriteLine($"DTO - file: {dto.file?.FileName ?? "null"}");
            Console.WriteLine($"DTO - module: {dto.module ?? "null"}");
            Console.WriteLine($"DTO - fileName: {dto.fileName ?? "null"}");
            Console.WriteLine($"DTO - category: {dto.category ?? "null"}");
            Console.WriteLine($"DTO - documentType: {dto.documentType ?? "null"}");

            if (dto.file == null)
            {
                return BadRequest(new { success = false, message = "No file uploaded" });
            }

            if (string.IsNullOrEmpty(dto.module))
            {
                return BadRequest(new { success = false, message = "Module is required" });
            }

            var userId = GetCurrentUserId();

            var command = new UploadDocumentCommand
            {
                File = dto.file,
                Module = dto.module,
                FileName = string.IsNullOrEmpty(dto.fileName) ? dto.file.FileName : dto.fileName,
                ReferenceId = dto.referenceId,
                Category = dto.category,
                Description = dto.description,
                DocumentType = dto.documentType ?? "Other",
                IsPublic = dto.isPublic,
                IsShared = dto.isShared,
                SharingLevel = dto.sharingLevel,
                FolderId = string.IsNullOrEmpty(dto.folderId) ? null : Guid.Parse(dto.folderId),
                UploadedBy = userId
            };

            var result = await _mediator.Send(command);
            Console.WriteLine($"Upload successful: {result?.Id}");

            // ✅ ADD THIS: Log the paths from the result
            if (result != null)
            {
                Console.WriteLine($"📁 [Controller] Result FilePath: '{result.FilePath}'");
                Console.WriteLine($"📁 [Controller] Result ThumbnailPath: '{result.ThumbnailPath}'");
            }

            return Ok(ApiResponse<object>.Ok(result, "Document uploaded successfully"));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Upload Error: {ex.Message}");
            return BadRequest(new
            {
                success = false,
                message = ex.Message,
                data = (object?)null,
                errors = new[] { ex.Message },
                statusCode = 400,
                timestamp = DateTime.UtcNow
            });
        }
    }

    // ✅ PUT /api/file/v1/documents/{id} - Update document
    [PerAuth("flm.company.manage")]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDocument(Guid id, [FromBody] UpdateDocumentCommand command)
    {
        var userId = GetCurrentUserId();
        command.Id = id;
        command.ModifiedBy = userId;
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<object>.Ok(result, "Document updated successfully"));
    }

    // ✅ DELETE /api/file/v1/documents/{id} - Delete document
    [PerAuth("flm.company.manage")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDocument(Guid id, [FromQuery] bool permanent = false)
    {
        var userId = GetCurrentUserId();

        var command = new DeleteDocumentCommand
        {
            Id = id,
            DeletedBy = userId,
            PermanentlyDelete = permanent
        };

        var result = await _mediator.Send(command);
        return Ok(ApiResponse<object>.Ok(result, permanent ? "Document permanently deleted" : "Document moved to trash"));
    }

    // ✅ POST /api/file/v1/documents/{id}/restore - Restore document
    [PerAuth("flm.company.manage")]
    [HttpPost("{id}/restore")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreDocument(Guid id)
    {
        var userId = GetCurrentUserId();

        var command = new RestoreDocumentCommand
        {
            Id = id,
            RestoredBy = userId
        };

        var result = await _mediator.Send(command);
        return Ok(ApiResponse<object>.Ok(result, "Document restored successfully"));
    }

    // ✅ POST /api/file/v1/documents/{id}/archive - Toggle archive status
    [PerAuth("flm.company.manage")]
    [HttpPost("{id}/archive")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ArchiveDocument(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();

            // First, get the current document to check its archive status
            var document = await _mediator.Send(new GetDocumentByIdQuery { Id = id });

            if (document == null)
                return NotFound(ApiResponse<object>.Error("Document not found"));

            // Toggle the archive status
            var command = new ArchiveDocumentCommand
            {
                Id = id,
                IsArchived = !document.IsArchived, // Toggle the status
                ArchivedBy = userId
            };

            var result = await _mediator.Send(command);

            return Ok(ApiResponse<object>.Ok(result,
                document.IsArchived ? "Document unarchived successfully" : "Document archived successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message,
                errors = new[] { ex.Message },
                statusCode = 400,
                timestamp = DateTime.UtcNow
            });
        }
    }

    // ✅ PUT /api/file/v1/documents/{id}/archive - Set archive status explicitly
    [PerAuth("flm.company.manage")]
    [HttpPut("{id}/archive")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetArchiveStatus(Guid id, [FromBody] ArchiveRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();

            // First, get the current document to check if it exists
            var document = await _mediator.Send(new GetDocumentByIdQuery { Id = id });

            if (document == null)
                return NotFound(ApiResponse<object>.Error("Document not found"));

            var command = new ArchiveDocumentCommand
            {
                Id = id,
                IsArchived = request.IsArchived,
                ArchivedBy = userId
            };

            var result = await _mediator.Send(command);

            return Ok(ApiResponse<object>.Ok(result,
                request.IsArchived ? "Document archived successfully" : "Document unarchived successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message,
                errors = new[] { ex.Message },
                statusCode = 400,
                timestamp = DateTime.UtcNow
            });
        }
    }





 [PerAuth("flm.company.manage")]
    [HttpPost("{id}/generate-share-link")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GenerateShareLink(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();

            // ✅ Get user name from JWT claims
            var userName = GetUserNameFromClaims();

            var command = new GenerateShareLinkCommand
            {
                DocumentId = id,
                UserId = userId,
                CreatedByName = userName
            };

            var result = await _mediator.Send(command);

            // Generate the full URL
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var shareUrl = $"{baseUrl}/api/file/v1/public/share/{result.Token}";

            var response = new
            {
                shareUrl = shareUrl,
                token = result.Token,
                expiresAt = result.ExpiresAt,
                uploadedBy = result.CreatedByName
            };

            return Ok(ApiResponse<object>.Ok(response, "Share link generated successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }



    // ✅ Get user name from JWT claims - Updated for your token structure
    private string GetUserNameFromClaims()
    {
        // The token has "userName" claim
        var userName = User?.FindFirst("userName")?.Value ??
                       User?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value ??
                       User?.FindFirst("name")?.Value ??
                       User?.FindFirst("unique_name")?.Value;

        // If no userName found, try email or fallback
        if (string.IsNullOrEmpty(userName))
        {
            userName = User?.FindFirst("email")?.Value ??
                       User?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value ??
                       "User";
        }

        return userName;
    }


    // ✅ POST /api/file/v1/documents/{id}/favorite - Toggle favorite
    [PerAuth("flm.company.view")]
    [HttpPost("{id}/favorite")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleFavorite(Guid id)
    {
        var userId = GetCurrentUserId();

        var command = new ToggleFavoriteCommand
        {
            DocumentId = id,
            UserId = userId
        };

        var result = await _mediator.Send(command);
        return Ok(ApiResponse<object>.Ok(result, "Favorite toggled"));
    }

    // ✅ POST /api/file/v1/documents/{id}/move - Move document
    [PerAuth("flm.company.manage")]
    [HttpPost("{id}/move")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MoveDocument(Guid id, [FromBody] MoveDocumentCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<object>.Ok(result, "Document moved successfully"));
    }

    // ✅ GET /api/file/v1/documents/{id}/download - Download document
    [PerAuth("flm.company.view")]
    [HttpGet("{id}/download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadDocument(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();

            var command = new DownloadDocumentCommand
            {
                Id = id,
                UserId = userId
            };

            var result = await _mediator.Send(command);

            if (result.FileBytes == null || result.FileBytes.Length == 0)
                return NotFound("File not found");

            return File(result.FileBytes, result.ContentType, result.FileName);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
}

// ✅ DTO for archive request
public class ArchiveRequest
{
    public bool IsArchived { get; set; }
}