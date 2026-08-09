// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Controllers\PublicController.cs

using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Cor.FileManagement.Queries;
using Cor.FileManagement.Commands;

namespace Cor.FileManagement.Controllers;

[ApiController]
[Route("api/file/v{version:apiVersion}/public")]
[ApiVersion("1.0")]
[AllowAnonymous] // ✅ Allow public access
public class PublicController : ControllerBase
{
    private readonly IMediator _mediator;

    public PublicController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ✅ GET /api/file/v1/public/info/{token} - Get file info for public page
    [HttpGet("info/{token}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    public async Task<IActionResult> GetPublicFileInfo(string token)
    {
        try
        {
            var query = new GetPublicFileInfoQuery { Token = token };
            var result = await _mediator.Send(query);

            if (!result.IsValid)
            {
                return BadRequest(new { success = false, message = result.ErrorMessage });
            }

            return Ok(ApiResponse<object>.Ok(result));
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    // ✅ GET /api/file/v1/public/download/{token} - Download file (no auth required)
    [HttpGet("download/{token}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    public async Task<IActionResult> PublicDownload(string token)
    {
        try
        {
            var query = new PublicDownloadQuery { Token = token };
            var result = await _mediator.Send(query);

            if (result.FileBytes == null || result.FileBytes.Length == 0)
                return NotFound("File not found");

            // Return the file for download
            return File(result.FileBytes, result.ContentType, result.FileName);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
// ✅ GET /api/file/v1/public/view/{token} - View file (no auth required)
[HttpGet("view/{token}")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status410Gone)]
public async Task<IActionResult> PublicView(string token)
{
    try
    {
        var query = new PublicDownloadQuery { Token = token };
        var result = await _mediator.Send(query);

        if (result.FileBytes == null || result.FileBytes.Length == 0)
            return NotFound("File not found");

        // Determine content type for viewing
        var contentType = result.ContentType;
        var fileName = result.FileName;
        var extension = Path.GetExtension(fileName).ToLower();

        // ✅ For images, PDFs, videos - set inline
        var viewableTypes = new[] { ".jpg", ".jpeg", ".png", ".gif", ".svg", ".webp", ".bmp", ".pdf", ".mp4", ".webm", ".ogg", ".mp3", ".wav" };

        if (viewableTypes.Contains(extension))
        {
            // Inline display
            Response.Headers["Content-Disposition"] = $"inline; filename=\"{fileName}\"";
        }
        else
        {
            // Force download for non-viewable types
            Response.Headers["Content-Disposition"] = $"attachment; filename=\"{fileName}\"";
        }

        return File(result.FileBytes, contentType);
    }
    catch (Exception ex)
    {
        return BadRequest(new { success = false, message = ex.Message });
    }
}
    // In PublicController.cs - Enhanced debug endpoint

    [HttpGet("debug/token/{token}")]
    [AllowAnonymous]
    public async Task<IActionResult> DebugToken(string token)
    {
        try
        {
            // Get token info
            var tokenQuery = new GetTokenInfoQuery { Token = token };
            var tokenInfo = await _mediator.Send(tokenQuery);

            if (tokenInfo == null)
            {
                return Ok(new
                {
                    success = false,
                    message = "Token not found",
                    token = token
                });
            }

            // Get document info
            var documentQuery = new GetDocumentByIdQuery { Id = tokenInfo.DocumentId };
            var document = await _mediator.Send(documentQuery);

            if (document == null)
            {
                return Ok(new
                {
                    success = false,
                    message = "Document not found",
                    token = token,
                    documentId = tokenInfo.DocumentId
                });
            }

            // ✅ Check file existence with multiple paths
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var dbPath = document.FilePath ?? "";
            var fileName = Path.GetFileName(dbPath);
            var originalFileName = document.OriginalFileName ?? "";
            var fileType = document.FileType ?? "";

            // Build all possible paths
            var pathsToCheck = new List<PathCheckInfo>
            {
                new PathCheckInfo { Path = Path.Combine(basePath, dbPath), Description = "Full DB Path" },
                new PathCheckInfo { Path = Path.Combine(basePath, "uploads", fileName), Description = "Uploads + FileName" },
                new PathCheckInfo { Path = Path.Combine(basePath, "uploads", originalFileName), Description = "Uploads + OriginalFileName" },
                new PathCheckInfo { Path = Path.Combine(basePath, "uploads", document.FileName ?? ""), Description = "Uploads + FileName (from doc)" },
                new PathCheckInfo { Path = Path.Combine(Directory.GetCurrentDirectory(), dbPath), Description = "Without wwwroot" },
                new PathCheckInfo { Path = Path.Combine(basePath, "uploads"), Description = "Uploads folder (directory)" },
            };

            // Check each path
            var pathResults = pathsToCheck.Select(p => new
            {
                p.Description,
                p.Path,
                Exists = System.IO.File.Exists(p.Path),
                IsDirectory = Directory.Exists(p.Path)
            }).ToList();

            // Get all files in uploads folder
            var uploadsFolder = Path.Combine(basePath, "uploads");
            var filesInUploads = new List<string>();
            if (Directory.Exists(uploadsFolder))
            {
                filesInUploads = Directory.GetFiles(uploadsFolder)
                    .Select(f => Path.GetFileName(f))
                    .ToList();
            }

            // Try to find a file that might match
            var similarFiles = filesInUploads
                .Where(f => f.Contains("1.png") || f.Contains("e80347c7") || f.Contains("png"))
                .ToList();

            return Ok(new
            {
                success = true,
                token = token,
                tokenInfo = new
                {
                    tokenInfo.DocumentId,
                    tokenInfo.ExpiresAt,
                    tokenInfo.CreatedAt,
                    tokenInfo.IsActive,
                    tokenInfo.DownloadCount
                },
                document = new
                {
                    document.Id,
                    document.FileName,
                    document.OriginalFileName,
                    document.FilePath,
                    document.FileSize,
                    document.FileType,
                    document.IsArchived,
                    document.IsPublic,
                    document.IsShared
                },
                fileSystem = new
                {
                    basePath = basePath,
                    uploadsFolder = uploadsFolder,
                    uploadsFolderExists = Directory.Exists(uploadsFolder),
                    totalFilesInUploads = filesInUploads.Count,
                    allFilesInUploads = filesInUploads,
                    similarFiles = similarFiles,
                    pathsChecked = pathResults,
                    currentDirectory = Directory.GetCurrentDirectory()
                }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message, stackTrace = ex.StackTrace });
        }
    }

    // Helper class
    public class PathCheckInfo
    {
        public string Path { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}