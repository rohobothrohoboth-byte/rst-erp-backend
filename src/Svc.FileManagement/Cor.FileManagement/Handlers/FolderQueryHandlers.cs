// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Handlers\FolderQueryHandlers.cs

using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.FileManagement.Models.DTOs;
using Cor.FileManagement.Models.Entities;
using Cor.FileManagement.Persistence;
using Cor.FileManagement.Queries;

namespace Cor.FileManagement.Handlers;

// ============================================================
// ✅ STATIC HELPER METHODS
// ============================================================

public static class FolderHelpers
{
    public static string GetFolderIcon(string? folderType)
    {
        return folderType?.ToLower() switch
        {
            "company" => "🏢",
            "department" => "📁",
            "personal" => "👤",
            "shared" => "🔗",
            "archive" => "📦",
            "finance" => "💰",
            "hr" => "👥",
            "team" => "👥",
            _ => "📁"
        };
    }

    public static string GetFolderColor(string? folderType)
    {
        return folderType?.ToLower() switch
        {
            "company" => "#4F46E5",
            "department" => "#7C3AED",
            "personal" => "#059669",
            "shared" => "#D97706",
            "archive" => "#6B7280",
            "finance" => "#0D9488",
            "hr" => "#DC2626",
            "team" => "#0891B2",
            _ => "#6B7280"
        };
    }

    // ✅ Add helper to format date
    public static string FormatDate(DateTime? date)
    {
        return date?.ToString("yyyy-MM-dd HH:mm") ?? "";
    }

    // ✅ Add helper to format file size
    public static string FormatFileSize(long bytes)
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
}

// ============================================================
// GET ROOT FOLDERS HANDLER - ✅ WITH COUNTS
// ============================================================

public class GetRootFoldersQueryHandler : IRequestHandler<GetRootFoldersQuery, List<FileFolderDto>>
{
    private readonly FileDbContext _context;

    public GetRootFoldersQueryHandler(FileDbContext context)
    {
        _context = context;
    }

    public async Task<List<FileFolderDto>> Handle(GetRootFoldersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.FileFolders
            .Where(f => !f.IsDeleted && !f.IsArchived && f.ParentId == null);

        if (request.UserId.HasValue)
        {
            query = query.Where(f => f.IsPublic || f.OwnerId == request.UserId);
        }

        var folders = await query
            .OrderBy(f => f.Order)
            .ThenBy(f => f.Name)
            .Select(f => new FileFolderDto
            {
                Id = f.Id,
                Name = f.Name,
                Description = f.Description,
                FolderType = f.FolderType,
                ParentId = f.ParentId,
                IsPublic = f.IsPublic,
                IsShared = f.IsShared,
                SharingLevel = f.SharingLevel,
                IsArchived = f.IsArchived,
                Order = f.Order,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.DateMod,
                // ✅ CALCULATE COUNTS
                DocumentCount = _context.FileDocuments.Count(d => d.FolderId == f.Id && !d.IsDeleted && !d.IsArchived),
                SubFolderCount = _context.FileFolders.Count(sf => sf.ParentId == f.Id && !sf.IsDeleted && !sf.IsArchived),
                CanEdit = true,
                CanDelete = true,
                CanShare = true,
                Icon = "",
                Color = ""
            })
            .ToListAsync(cancellationToken);

        // ✅ Apply icons and colors after fetching from DB
        return folders.Select(f =>
        {
            f.Icon = FolderHelpers.GetFolderIcon(f.FolderType);
            f.Color = FolderHelpers.GetFolderColor(f.FolderType);
            return f;
        }).ToList();
    }
}

// ============================================================
// GET FOLDERS HANDLER - ✅ WITH COUNTS
// ============================================================

public class GetFoldersQueryHandler : IRequestHandler<GetFoldersQuery, List<FileFolderDto>>
{
    private readonly FileDbContext _context;

    public GetFoldersQueryHandler(FileDbContext context)
    {
        _context = context;
    }

    public async Task<List<FileFolderDto>> Handle(GetFoldersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.FileFolders
            .Where(f => !f.IsDeleted);

        if (request.ParentId.HasValue)
            query = query.Where(f => f.ParentId == request.ParentId);
        else
            query = query.Where(f => f.ParentId == null);

        if (!string.IsNullOrEmpty(request.FolderType))
            query = query.Where(f => f.FolderType == request.FolderType);

        if (!request.IncludeArchived)
            query = query.Where(f => !f.IsArchived);

        var folders = await query
            .OrderBy(f => f.Order)
            .ThenBy(f => f.Name)
            .Select(f => new FileFolderDto
            {
                Id = f.Id,
                Name = f.Name,
                Description = f.Description,
                FolderType = f.FolderType,
                ParentId = f.ParentId,
                IsPublic = f.IsPublic,
                IsShared = f.IsShared,
                SharingLevel = f.SharingLevel,
                IsArchived = f.IsArchived,
                Order = f.Order,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.DateMod,
                // ✅ CALCULATE COUNTS
                DocumentCount = _context.FileDocuments.Count(d => d.FolderId == f.Id && !d.IsDeleted && !d.IsArchived),
                SubFolderCount = _context.FileFolders.Count(sf => sf.ParentId == f.Id && !sf.IsDeleted && !sf.IsArchived),
                CanEdit = true,
                CanDelete = true,
                CanShare = true,
                Icon = "",
                Color = ""
            })
            .ToListAsync(cancellationToken);

        // ✅ Apply icons and colors after fetching from DB
        return folders.Select(f =>
        {
            f.Icon = FolderHelpers.GetFolderIcon(f.FolderType);
            f.Color = FolderHelpers.GetFolderColor(f.FolderType);
            return f;
        }).ToList();
    }
}

// ============================================================
// GET FOLDER BY ID HANDLER - ✅ WITH COUNTS
// ============================================================

public class GetFolderByIdQueryHandler : IRequestHandler<GetFolderByIdQuery, FileFolderDto>
{
    private readonly FileDbContext _context;

    public GetFolderByIdQueryHandler(FileDbContext context)
    {
        _context = context;
    }

    public async Task<FileFolderDto> Handle(GetFolderByIdQuery request, CancellationToken cancellationToken)
    {
        var folder = await _context.FileFolders
            .FirstOrDefaultAsync(f => f.Id == request.Id && !f.IsDeleted, cancellationToken);

        if (folder == null)
            throw new Exception("Folder not found");

        // ✅ Get counts
        var documentCount = await _context.FileDocuments
            .CountAsync(d => d.FolderId == request.Id && !d.IsDeleted && !d.IsArchived, cancellationToken);

        var subFolderCount = await _context.FileFolders
            .CountAsync(f => f.ParentId == request.Id && !f.IsDeleted && !f.IsArchived, cancellationToken);

        return new FileFolderDto
        {
            Id = folder.Id,
            Name = folder.Name,
            Description = folder.Description,
            FolderType = folder.FolderType,
            ParentId = folder.ParentId,
            IsPublic = folder.IsPublic,
            IsShared = folder.IsShared,
            SharingLevel = folder.SharingLevel,
            IsArchived = folder.IsArchived,
            Order = folder.Order,
            CreatedAt = folder.CreatedAt,
            UpdatedAt = folder.DateMod,
            DocumentCount = documentCount,
            SubFolderCount = subFolderCount,
            CanEdit = true,
            CanDelete = true,
            CanShare = true,
            Icon = FolderHelpers.GetFolderIcon(folder.FolderType),
            Color = FolderHelpers.GetFolderColor(folder.FolderType)
        };
    }
}

// ============================================================
// ✅ GET FOLDER CONTENTS HANDLER
// ============================================================

public class GetFolderContentsQueryHandler : IRequestHandler<GetFolderContentsQuery, FolderContentsDto>
{
    private readonly FileDbContext _context;

    public GetFolderContentsQueryHandler(FileDbContext context)
    {
        _context = context;
    }

    public async Task<FolderContentsDto> Handle(GetFolderContentsQuery request, CancellationToken cancellationToken)
    {
        // Get the folder
        var folder = await _context.FileFolders
            .FirstOrDefaultAsync(f => f.Id == request.FolderId && !f.IsDeleted, cancellationToken);

        if (folder == null)
        {
            return new FolderContentsDto
            {
                Folder = null,
                SubFolders = new List<FileFolderDto>(),
                Documents = new List<FileDocumentDto>()
            };
        }

        // ✅ Get sub-folders with counts
        var subFolders = await _context.FileFolders
            .Where(f => f.ParentId == request.FolderId && !f.IsDeleted && !f.IsArchived)
            .OrderBy(f => f.Order)
            .ThenBy(f => f.Name)
            .Select(f => new FileFolderDto
            {
                Id = f.Id,
                Name = f.Name,
                Description = f.Description,
                FolderType = f.FolderType,
                ParentId = f.ParentId,
                IsPublic = f.IsPublic,
                IsShared = f.IsShared,
                SharingLevel = f.SharingLevel,
                IsArchived = f.IsArchived,
                Order = f.Order,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.DateMod,
                DocumentCount = _context.FileDocuments.Count(d => d.FolderId == f.Id && !d.IsDeleted && !d.IsArchived),
                SubFolderCount = _context.FileFolders.Count(sf => sf.ParentId == f.Id && !sf.IsDeleted && !sf.IsArchived),
                CanEdit = f.OwnerId == request.UserId,
                CanDelete = f.OwnerId == request.UserId,
                CanShare = true,
                Icon = "",
                Color = ""
            })
            .ToListAsync(cancellationToken);

        // ✅ Apply icons and colors after fetching from DB
        var subFoldersWithIcons = subFolders.Select(f =>
        {
            f.Icon = FolderHelpers.GetFolderIcon(f.FolderType);
            f.Color = FolderHelpers.GetFolderColor(f.FolderType);
            return f;
        }).ToList();

        // ✅ Get documents in the folder
        var documents = await _context.FileDocuments
            .Where(d => d.FolderId == request.FolderId && !d.IsDeleted && !d.IsArchived)
            .OrderByDescending(d => d.DateAdd)
            .Select(d => new FileDocumentDto
            {
                Id = d.Id,
                FileName = d.FileName,
                OriginalFileName = d.OriginalFileName,
                FileSize = d.FileSize,
                FileSizeFormatted = FolderHelpers.FormatFileSize(d.FileSize),
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
                UploadedAt = d.DateAdd,
                UploadedAtFormatted = FolderHelpers.FormatDate(d.DateAdd),
                CanEdit = d.UploadedBy == request.UserId,
                CanDelete = d.UploadedBy == request.UserId,
                CanDownload = true,
                CanShare = true
            })
            .ToListAsync(cancellationToken);

        return new FolderContentsDto
        {
            Folder = new FileFolderDto
            {
                Id = folder.Id,
                Name = folder.Name,
                Description = folder.Description,
                FolderType = folder.FolderType,
                ParentId = folder.ParentId,
                IsPublic = folder.IsPublic,
                IsShared = folder.IsShared,
                SharingLevel = folder.SharingLevel,
                IsArchived = folder.IsArchived,
                Order = folder.Order,
                CreatedAt = folder.CreatedAt,
                UpdatedAt = folder.DateMod,
                DocumentCount = documents.Count,
                SubFolderCount = subFolders.Count,
                CanEdit = folder.OwnerId == request.UserId,
                CanDelete = folder.OwnerId == request.UserId,
                CanShare = true,
                Icon = FolderHelpers.GetFolderIcon(folder.FolderType),
                Color = FolderHelpers.GetFolderColor(folder.FolderType)
            },
            SubFolders = subFoldersWithIcons,
            Documents = documents
        };
    }
}

// ============================================================
// ✅ GET SUB-FOLDERS HANDLER
// ============================================================

public class GetSubFoldersQueryHandler : IRequestHandler<GetSubFoldersQuery, List<FileFolderDto>>
{
    private readonly FileDbContext _context;

    public GetSubFoldersQueryHandler(FileDbContext context)
    {
        _context = context;
    }

    public async Task<List<FileFolderDto>> Handle(GetSubFoldersQuery request, CancellationToken cancellationToken)
    {
        var folders = await _context.FileFolders
            .Where(f => f.ParentId == request.FolderId && !f.IsDeleted && !f.IsArchived)
            .OrderBy(f => f.Order)
            .ThenBy(f => f.Name)
            .Select(f => new FileFolderDto
            {
                Id = f.Id,
                Name = f.Name,
                Description = f.Description,
                FolderType = f.FolderType,
                ParentId = f.ParentId,
                IsPublic = f.IsPublic,
                IsShared = f.IsShared,
                SharingLevel = f.SharingLevel,
                IsArchived = f.IsArchived,
                Order = f.Order,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.DateMod,
                DocumentCount = _context.FileDocuments.Count(d => d.FolderId == f.Id && !d.IsDeleted && !d.IsArchived),
                SubFolderCount = _context.FileFolders.Count(sf => sf.ParentId == f.Id && !sf.IsDeleted && !sf.IsArchived),
                CanEdit = f.OwnerId == request.UserId,
                CanDelete = f.OwnerId == request.UserId,
                CanShare = true,
                Icon = "",
                Color = ""
            })
            .ToListAsync(cancellationToken);

        // ✅ Apply icons and colors after fetching from DB
        return folders.Select(f =>
        {
            f.Icon = FolderHelpers.GetFolderIcon(f.FolderType);
            f.Color = FolderHelpers.GetFolderColor(f.FolderType);
            return f;
        }).ToList();
    }
}

// ============================================================
// ✅ GET FOLDER DOCUMENTS HANDLER
// ============================================================

// ============================================================
// ✅ GET FOLDER DOCUMENTS HANDLER - UPDATED WITH IsFavorite
// ============================================================

public class GetFolderDocumentsQueryHandler : IRequestHandler<GetFolderDocumentsQuery, List<FileDocumentDto>>
{
    private readonly FileDbContext _context;

    public GetFolderDocumentsQueryHandler(FileDbContext context)
    {
        _context = context;
    }

    public async Task<List<FileDocumentDto>> Handle(GetFolderDocumentsQuery request, CancellationToken cancellationToken)
    {
        var documents = await _context.FileDocuments
            .Where(d => d.FolderId == request.FolderId && !d.IsDeleted && !d.IsArchived)
            .OrderByDescending(d => d.DateAdd)
            .Select(d => new FileDocumentDto
            {
                Id = d.Id,
                FileName = d.FileName,
                OriginalFileName = d.OriginalFileName,
                FileSize = d.FileSize,
                FileSizeFormatted = FolderHelpers.FormatFileSize(d.FileSize),
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
                UploadedAt = d.DateAdd,
                UploadedAtFormatted = FolderHelpers.FormatDate(d.DateAdd),
                CanEdit = d.UploadedBy == request.UserId,
                CanDelete = d.UploadedBy == request.UserId,
                CanDownload = true,
                CanShare = true,
                // ✅ ADD IsFavorite
                IsFavorite = _context.FileFavorites.Any(f => f.DocumentId == d.Id && f.UserId == request.UserId)
            })
            .ToListAsync(cancellationToken);

        return documents;
    }
}