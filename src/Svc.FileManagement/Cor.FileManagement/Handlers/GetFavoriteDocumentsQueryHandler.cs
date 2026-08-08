// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Handlers\GetFavoriteDocumentsQueryHandler.cs

using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.FileManagement.Models.DTOs;
using Cor.FileManagement.Persistence;
using Cor.FileManagement.Queries;

namespace Cor.FileManagement.Handlers;

public class GetFavoriteDocumentsQueryHandler : IRequestHandler<GetFavoriteDocumentsQuery, List<FileDocumentDto>>
{
    private readonly FileDbContext _context;

    public GetFavoriteDocumentsQueryHandler(FileDbContext context)
    {
        _context = context;
    }

    public async Task<List<FileDocumentDto>> Handle(GetFavoriteDocumentsQuery request, CancellationToken cancellationToken)
    {
        // ✅ Get favorite document IDs for the user
        var favoriteIds = await _context.FileFavorites
            .Where(f => f.UserId == request.UserId)
            .Select(f => f.DocumentId)
            .ToListAsync(cancellationToken);

        // If no favorites, return empty list
        if (!favoriteIds.Any())
            return new List<FileDocumentDto>();

        // ✅ Get the actual documents that are favorited
        var documents = await _context.FileDocuments
            .Where(d => favoriteIds.Contains(d.Id) && !d.IsDeleted && !d.IsArchived)
            .OrderByDescending(d => d.DateAdd)
            .ToListAsync(cancellationToken);

        // ✅ Map to DTOs
        return documents.Select(d => new FileDocumentDto
        {
            Id = d.Id,
            FileName = d.FileName,
            OriginalFileName = d.OriginalFileName,
            FileSize = d.FileSize,
            FileSizeFormatted = FormatFileSize(d.FileSize),
            FileType = d.FileType,
            FileExtension = d.FileExtension,
            FilePath = d.FilePath,
            ThumbnailPath = d.ThumbnailPath,
            Description = d.Description,
            Module = d.Module,
            ReferenceId = d.ReferenceId,
            Category = d.Category,
            DocumentType = d.DocumentType,
            IsPublic = d.IsPublic,
            IsShared = d.IsShared,
            SharingLevel = d.SharingLevel,
            IsArchived = d.IsArchived,
            Version = d.Version,
            FolderId = d.FolderId,
            UploadedBy = d.UploadedBy.ToString(),
            UploadedAt = d.UploadedAt,
            UploadedAtFormatted = d.UploadedAt.ToString("yyyy-MM-dd HH:mm") ?? "",
            Icon = GetFileIcon(d.FileType),
            IsFavorite = true,
            CanEdit = true,
            CanDelete = true,
            CanDownload = true,
            CanShare = true
        }).ToList();
    }

    private string FormatFileSize(long bytes)
    {
        if (bytes <= 0) return "0 B";
        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
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