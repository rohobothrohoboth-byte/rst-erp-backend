// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Handlers\GetFilesQueryHandler.cs

using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.FileManagement.Models.DTOs;
using Cor.FileManagement.Persistence;
using Cor.FileManagement.Models.Entities;
using Cor.FileManagement.Commands;
using Cor.FileManagement.Queries;
namespace Cor.FileManagement.Handlers;

public class GetFilesQueryHandler : IRequestHandler<GetFilesQuery, List<FileDocumentDto>>
{
    private readonly FileDbContext _context;

    public GetFilesQueryHandler(FileDbContext context)
    {
        _context = context;
    }

    public async Task<List<FileDocumentDto>> Handle(GetFilesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.FileDocuments
            .Where(f => !f.IsDeleted);

        // Filters
        if (!request.IncludeArchived)
            query = query.Where(f => !f.IsArchived);

        if (!string.IsNullOrEmpty(request.Module))
            query = query.Where(f => f.Module == request.Module);

        if (request.ReferenceId.HasValue)
            query = query.Where(f => f.ReferenceId == request.ReferenceId);

        if (!string.IsNullOrEmpty(request.Category))
            query = query.Where(f => f.Category == request.Category);

        if (!string.IsNullOrEmpty(request.DocumentType))
            query = query.Where(f => f.DocumentType == request.DocumentType);

        if (request.FolderId.HasValue)
            query = query.Where(f => f.FolderId == request.FolderId);
        else if (request.FolderId == Guid.Empty)
            query = query.Where(f => f.FolderId == null);

        // Search
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var search = request.SearchTerm.ToLower();
            query = query.Where(f =>
                f.FileName.ToLower().Contains(search) ||
                f.OriginalFileName.ToLower().Contains(search) ||
                f.Description.ToLower().Contains(search) ||
                f.Category.ToLower().Contains(search));
        }

        // Order by
        query = query.OrderByDescending(f => f.UploadedAt);

        // Pagination
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

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