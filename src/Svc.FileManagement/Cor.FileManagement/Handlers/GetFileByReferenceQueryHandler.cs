// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Handlers\GetFileByReferenceQueryHandler.cs

using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.FileManagement.Models.DTOs;
using Cor.FileManagement.Persistence;
using Cor.FileManagement.Models.Entities;
using Cor.FileManagement.Commands;
using Cor.FileManagement.Queries;
namespace Cor.FileManagement.Handlers;

public class GetFileByReferenceQueryHandler : IRequestHandler<GetFileByReferenceQuery, List<FileDocumentDto>>
{
    private readonly FileDbContext _context;

    public GetFileByReferenceQueryHandler(FileDbContext context)
    {
        _context = context;
    }

    public async Task<List<FileDocumentDto>> Handle(GetFileByReferenceQuery request, CancellationToken cancellationToken)
    {
        var query = _context.FileDocuments
            .Where(f => !f.IsDeleted &&
                        f.Module == request.Module &&
                        f.ReferenceId == request.ReferenceId);

        if (!string.IsNullOrEmpty(request.Category))
            query = query.Where(f => f.Category == request.Category);

        query = query.OrderByDescending(f => f.UploadedAt);

        var items = await query.ToListAsync(cancellationToken);

        return items.Select(MapToDto).ToList();
    }

    private FileDocumentDto MapToDto(FileDocument document)
    {
        return new FileDocumentDto
        {
            Id = document.Id,
            FileName = document.FileName,
            OriginalFileName = document.OriginalFileName,
            FileSize = document.FileSize,
            FileSizeFormatted = FormatFileSize(document.FileSize),
            FileType = document.FileType,
            FileExtension = document.FileExtension,
            FilePath = document.FilePath,
            ThumbnailPath = document.ThumbnailPath,
            Description = document.Description,
            Module = document.Module,
            ReferenceId = document.ReferenceId,
            Category = document.Category,
            DocumentType = document.DocumentType,
            IsPublic = document.IsPublic,
            IsShared = document.IsShared,
            SharingLevel = document.SharingLevel,
            IsArchived = document.IsArchived,
            Version = document.Version,
            FolderId = document.FolderId,
            UploadedBy = document.UploadedBy.ToString(),
            UploadedAt = document.UploadedAt,
            UploadedAtFormatted = document.UploadedAt.ToString("yyyy-MM-dd HH:mm"),
            Icon = GetFileIcon(document.FileType),
            CanEdit = true,
            CanDelete = true,
            CanDownload = true,
            CanShare = true
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

    private string GetFileIcon(string fileType)
    {
        if (string.IsNullOrEmpty(fileType)) return "📄";
        if (fileType.Contains("pdf")) return "📄";
        if (fileType.Contains("image")) return "🖼️";
        if (fileType.Contains("word") || fileType.Contains("document")) return "📝";
        if (fileType.Contains("excel") || fileType.Contains("sheet")) return "📊";
        if (fileType.Contains("powerpoint") || fileType.Contains("presentation")) return "📑";
        if (fileType.Contains("text")) return "📃";
        if (fileType.Contains("zip") || fileType.Contains("archive")) return "📦";
        return "📄";
    }
}