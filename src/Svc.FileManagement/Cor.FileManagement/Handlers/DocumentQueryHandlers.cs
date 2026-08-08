// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Handlers\DocumentQueryHandlers.cs

using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.FileManagement.Models.DTOs;
using Cor.FileManagement.Models.Entities;
using Cor.FileManagement.Persistence;
using Cor.FileManagement.Queries;
using Cor.FileManagement.Commands;

namespace Cor.FileManagement.Handlers;

public class GetDocumentsQueryHandler : IRequestHandler<GetDocumentsQuery, List<FileDocumentDto>>
{
    private readonly FileDbContext _context;

    public GetDocumentsQueryHandler(FileDbContext context)
    {
        _context = context;
    }

    public async Task<List<FileDocumentDto>> Handle(GetDocumentsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.FileDocuments
            .Where(f => !f.IsDeleted);

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

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var search = request.SearchTerm.ToLower();
            query = query.Where(f =>
                f.FileName.ToLower().Contains(search) ||
                f.OriginalFileName.ToLower().Contains(search) ||
                f.Description.ToLower().Contains(search) ||
                f.Category.ToLower().Contains(search));
        }

        // Apply sorting
        query = request.SortBy?.ToLower() switch
        {
            "name" => request.SortDescending ? query.OrderByDescending(f => f.FileName) : query.OrderBy(f => f.FileName),
            "size" => request.SortDescending ? query.OrderByDescending(f => f.FileSize) : query.OrderBy(f => f.FileSize),
            "type" => request.SortDescending ? query.OrderByDescending(f => f.FileType) : query.OrderBy(f => f.FileType),
            "owner" => request.SortDescending ? query.OrderByDescending(f => f.UploadedByName) : query.OrderBy(f => f.UploadedByName),
            _ => request.SortDescending ? query.OrderByDescending(f => f.UploadedAt) : query.OrderBy(f => f.UploadedAt)
        };

        var documents = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return documents.Select(MapToDto).ToList();
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
}

public class GetRecentDocumentsQueryHandler : IRequestHandler<GetRecentDocumentsQuery, List<FileDocumentDto>>
{
    private readonly FileDbContext _context;

    public GetRecentDocumentsQueryHandler(FileDbContext context)
    {
        _context = context;
    }

    public async Task<List<FileDocumentDto>> Handle(GetRecentDocumentsQuery request, CancellationToken cancellationToken)
    {
        var documents = await _context.FileDocuments
            .Where(f => !f.IsDeleted && !f.IsArchived)
            .OrderByDescending(f => f.UploadedAt)
            .Take(request.Limit)
            .ToListAsync(cancellationToken);

        return documents.Select(MapToDto).ToList();
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
}

public class GetDocumentsByModuleAndReferenceQueryHandler : IRequestHandler<GetDocumentsByModuleAndReferenceQuery, List<FileDocumentDto>>
{
    private readonly FileDbContext _context;

    public GetDocumentsByModuleAndReferenceQueryHandler(FileDbContext context)
    {
        _context = context;
    }

    public async Task<List<FileDocumentDto>> Handle(GetDocumentsByModuleAndReferenceQuery request, CancellationToken cancellationToken)
    {
        var query = _context.FileDocuments
            .Where(f => !f.IsDeleted && !f.IsArchived && f.Module == request.Module && f.ReferenceId == request.ReferenceId);

        if (!string.IsNullOrEmpty(request.Category))
            query = query.Where(f => f.Category == request.Category);

        var documents = await query
            .OrderByDescending(f => f.UploadedAt)
            .ToListAsync(cancellationToken);

        return documents.Select(MapToDto).ToList();
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
}

public class GetDocumentByIdQueryHandler : IRequestHandler<GetDocumentByIdQuery, FileDocumentDto>
{
    private readonly FileDbContext _context;

    public GetDocumentByIdQueryHandler(FileDbContext context)
    {
        _context = context;
    }

    public async Task<FileDocumentDto> Handle(GetDocumentByIdQuery request, CancellationToken cancellationToken)
    {
        var document = await _context.FileDocuments
            .FirstOrDefaultAsync(f => f.Id == request.Id && !f.IsDeleted, cancellationToken);

        if (document == null)
            throw new Exception("Document not found");

        return MapToDto(document);
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
}



public class GetDocumentsByModuleQueryHandler : IRequestHandler<GetDocumentsByModuleQuery, List<FileDocumentDto>>
{
    private readonly FileDbContext _context;

    public GetDocumentsByModuleQueryHandler(FileDbContext context)
    {
        _context = context;
    }

    public async Task<List<FileDocumentDto>> Handle(GetDocumentsByModuleQuery request, CancellationToken cancellationToken)
    {
        var query = _context.FileDocuments
            .Where(f => !f.IsDeleted && !f.IsArchived && f.Module == request.Module);

        // Filter by category if provided
        if (!string.IsNullOrEmpty(request.Category))
            query = query.Where(f => f.Category == request.Category);

        // Filter by reference ID if provided
        if (request.ReferenceId.HasValue)
            query = query.Where(f => f.ReferenceId == request.ReferenceId);

        var documents = await query
            .OrderByDescending(f => f.UploadedAt)
            .ToListAsync(cancellationToken);

        return documents.Select(MapToDto).ToList();
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
}


// ✅ ADD THIS - GetArchivedDocumentsQueryHandler
public class GetArchivedDocumentsQueryHandler : IRequestHandler<GetArchivedDocumentsQuery, List<FileDocumentDto>>
{
    private readonly FileDbContext _context;

    public GetArchivedDocumentsQueryHandler(FileDbContext context)
    {
        _context = context;
    }

    public async Task<List<FileDocumentDto>> Handle(GetArchivedDocumentsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.FileDocuments
            .Where(f => !f.IsDeleted && f.IsArchived);

        // Apply search filter if provided
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var search = request.SearchTerm.ToLower();
            query = query.Where(f =>
                f.FileName.ToLower().Contains(search) ||
                f.OriginalFileName.ToLower().Contains(search) ||
                (f.Description != null && f.Description.ToLower().Contains(search))
            );
        }

        // Order by archived date (most recent first)
        query = query.OrderByDescending(f => f.ArchivedAt ?? f.DateMod);

        var documents = await query.ToListAsync(cancellationToken);

        return documents.Select(MapToDto).ToList();
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
            ArchivedAt = document.ArchivedAt, // ✅ Include ArchivedAt
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
}