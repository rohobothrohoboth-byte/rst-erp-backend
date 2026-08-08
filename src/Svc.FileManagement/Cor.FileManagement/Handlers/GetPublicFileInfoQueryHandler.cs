// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Handlers\GetPublicFileInfoQueryHandler.cs

using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.FileManagement.Models.DTOs;
using Cor.FileManagement.Persistence;
using Cor.FileManagement.Queries;
using Cor.FileManagement.Models.Entities;
namespace Cor.FileManagement.Handlers;

public class GetPublicFileInfoQueryHandler : IRequestHandler<GetPublicFileInfoQuery, PublicFileInfoDto>
{
    private readonly FileDbContext _context;

    public GetPublicFileInfoQueryHandler(FileDbContext context)
    {
        _context = context;
    }

    public async Task<PublicFileInfoDto> Handle(GetPublicFileInfoQuery request, CancellationToken cancellationToken)
    {
        // Find the share token
        var shareRecord = await _context.FileShareTokens
            .FirstOrDefaultAsync(t => t.Token == request.Token && t.IsActive, cancellationToken);

        if (shareRecord == null)
        {
            return new PublicFileInfoDto
            {
                IsValid = false,
                ErrorMessage = "Invalid share link"
            };
        }

        // Check if token has expired
        if (shareRecord.ExpiresAt < DateTime.UtcNow)
        {
            return new PublicFileInfoDto
            {
                IsValid = false,
                ErrorMessage = "This share link has expired"
            };
        }

        // Get the document
        var document = await _context.FileDocuments
            .FirstOrDefaultAsync(f => f.Id == shareRecord.DocumentId && !f.IsDeleted, cancellationToken);

        if (document == null)
        {
            return new PublicFileInfoDto
            {
                IsValid = false,
                ErrorMessage = "Document not found"
            };
        }

        // Check if file exists on disk
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", document.FilePath);
        if (!System.IO.File.Exists(filePath))
        {
            return new PublicFileInfoDto
            {
                IsValid = false,
                ErrorMessage = "File not found on server"
            };
        }

        return new PublicFileInfoDto
        {
            IsValid = true,
            FileName = document.FileName,
            OriginalFileName = document.OriginalFileName ?? document.FileName,
            FileSize = document.FileSize,
            FileSizeFormatted = FormatFileSize(document.FileSize),
            ContentType = document.FileType ?? "application/octet-stream",
            Token = request.Token,
            ExpiresAt = shareRecord.ExpiresAt,
            UploadedBy = document.UploadedBy.ToString(),
             CreatedByName = shareRecord.CreatedByName ?? shareRecord.CreatedBy.ToString(),
            UploadedAt = document.UploadedAt,
            Description = document.Description ?? string.Empty
        };
    }

    private string FormatFileSize(long bytes)
    {
        string[] sizes = { "Bytes", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }


    public class GetTokenInfoQueryHandler : IRequestHandler<GetTokenInfoQuery, FileShareToken?>
    {
        private readonly FileDbContext _context;

        public GetTokenInfoQueryHandler(FileDbContext context)
        {
            _context = context;
        }

        public async Task<FileShareToken?> Handle(GetTokenInfoQuery request, CancellationToken cancellationToken)
        {
            return await _context.FileShareTokens
                .FirstOrDefaultAsync(t => t.Token == request.Token && t.IsActive, cancellationToken);
        }
    }
}