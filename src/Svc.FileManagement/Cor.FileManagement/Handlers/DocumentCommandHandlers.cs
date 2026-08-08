// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Handlers\DocumentCommandHandlers.cs

using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.FileManagement.Models.Entities;
using Cor.FileManagement.Models.DTOs;
using Cor.FileManagement.Persistence;
using Cor.FileManagement.Commands;
using Cor.FileManagement.Services;
using Cor.FileManagement.Queries;

namespace Cor.FileManagement.Handlers;

public class UploadDocumentCommandHandler : IRequestHandler<UploadDocumentCommand, FileDocumentDto>
{
    private readonly FileDbContext _context;
    private readonly IFileStorageService _storageService;
    private readonly IFileValidationService _validationService;
    private readonly IFileThumbnailService _thumbnailService;
    private readonly IConfiguration _configuration;

    public UploadDocumentCommandHandler(
        FileDbContext context,
        IFileStorageService storageService,
        IFileValidationService validationService,
        IFileThumbnailService thumbnailService,
        IConfiguration configuration)
    {
        _context = context;
        _storageService = storageService;
        _validationService = validationService;
        _thumbnailService = thumbnailService;
        _configuration = configuration;
    }

    public async Task<FileDocumentDto> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        // Validate file
        if (!_validationService.ValidateFile(request.File, out string errorMessage))
            throw new Exception(errorMessage);

        // Check if folder exists
        if (request.FolderId.HasValue)
        {
            var folder = await _context.FileFolders
                .FirstOrDefaultAsync(f => f.Id == request.FolderId.Value && !f.IsDeleted, cancellationToken);

            if (folder == null)
                throw new Exception("Folder not found");
        }

        // Save file
        var uploadPath = _configuration["FileStorage:UploadPath"] ?? "uploads";
        var filePath = await _storageService.SaveFileAsync(
            request.File.OpenReadStream(),
            request.File.FileName,
            uploadPath);

        // Generate thumbnail if image
        string? thumbnailPath = null;
        try
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath);
            var thumbnailDir = Path.Combine("wwwroot", _configuration["FileStorage:ThumbnailPath"] ?? "thumbnails");
            var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff" };
            var extension = Path.GetExtension(request.File.FileName).ToLower();

            if (imageExtensions.Contains(extension))
            {
                thumbnailPath = await _thumbnailService.GenerateThumbnailAsync(fullPath, thumbnailDir);
                if (!string.IsNullOrEmpty(thumbnailPath))
                    thumbnailPath = Path.GetRelativePath(Directory.GetCurrentDirectory(), thumbnailPath);
            }
        }
        catch
        {
            thumbnailPath = null;
        }

        // Create document
        var document = new FileDocument
        {
            Id = Guid.NewGuid(),
            FileName = request.FileName ?? request.File.FileName,
            OriginalFileName = request.File.FileName,
            FileSize = request.File.Length,
            FileType = request.File.ContentType,
            FileExtension = Path.GetExtension(request.File.FileName),
            FilePath = filePath,
            ThumbnailPath = thumbnailPath,
             DocumentType = request.DocumentType ?? "Other",
            Module = request.Module,
            ReferenceId = request.ReferenceId,
             Description = request.Description ?? string.Empty,
              Category = request.Category ?? string.Empty,
            IsPublic = request.IsPublic,
            IsShared = request.IsShared,
            SharingLevel = request.SharingLevel ?? "Private",
            FolderId = request.FolderId,
            UploadedBy = request.UploadedBy,
            UploadedAt = DateTime.UtcNow,
            DateAdd = DateTime.UtcNow,
            DateMod = DateTime.UtcNow,
            Version = 1,
            IsDeleted = false,
            IsArchived = false,
            StorageProvider = "Local"
        };

        await _context.FileDocuments.AddAsync(document, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

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

// ✅ Download Document Command Handler
public class DownloadDocumentCommandHandler : IRequestHandler<DownloadDocumentCommand, DownloadResultDto>
{
    private readonly FileDbContext _context;
    private readonly IFileStorageService _storageService;

    public DownloadDocumentCommandHandler(FileDbContext context, IFileStorageService storageService)
    {
        _context = context;
        _storageService = storageService;
    }

    public async Task<DownloadResultDto> Handle(DownloadDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await _context.FileDocuments
            .FirstOrDefaultAsync(f => f.Id == request.Id && !f.IsDeleted, cancellationToken);

        if (document == null)
            throw new Exception("Document not found");

        // ✅ Read the file from storage
        var fileBytes = await _storageService.GetFileAsync(document.FilePath);

        return new DownloadResultDto
        {
            FileBytes = fileBytes,
            ContentType = document.FileType ?? "application/octet-stream",
            FileName = document.FileName
        };
    }
}

// ✅ Update Document Command Handler
public class UpdateDocumentCommandHandler : IRequestHandler<UpdateDocumentCommand, FileDocumentDto>
{
    private readonly FileDbContext _context;
    private readonly IFileStorageService _storageService;
    private readonly IFileValidationService _validationService;

    public UpdateDocumentCommandHandler(
        FileDbContext context,
        IFileStorageService storageService,
        IFileValidationService validationService)
    {
        _context = context;
        _storageService = storageService;
        _validationService = validationService;
    }

    public async Task<FileDocumentDto> Handle(UpdateDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await _context.FileDocuments
            .FirstOrDefaultAsync(f => f.Id == request.Id && !f.IsDeleted, cancellationToken);

        if (document == null)
            throw new Exception("Document not found");

        // Update metadata
        if (!string.IsNullOrEmpty(request.FileName))
            document.FileName = request.FileName;

        if (!string.IsNullOrEmpty(request.Description))
            document.Description = request.Description;

        if (!string.IsNullOrEmpty(request.Category))
            document.Category = request.Category;

        document.IsPublic = request.IsPublic;
        document.IsShared = request.IsShared;

         document.SharingLevel = request.SharingLevel ?? document.SharingLevel ?? "Private";
        document.FolderId = request.FolderId;
        document.LastModifiedAt = DateTime.UtcNow;
        document.LastModifiedBy = request.ModifiedBy;
        document.DateMod = DateTime.UtcNow;
        document.RowVersion++;

        await _context.SaveChangesAsync(cancellationToken);
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
public class ArchiveDocumentCommandHandler : IRequestHandler<ArchiveDocumentCommand, bool>
{
    private readonly FileDbContext _context;
    private readonly IFileStorageService _storageService;

    public ArchiveDocumentCommandHandler(FileDbContext context, IFileStorageService storageService)
    {
        _context = context;
        _storageService = storageService;
    }

    public async Task<bool> Handle(ArchiveDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await _context.FileDocuments
            .FirstOrDefaultAsync(f => f.Id == request.Id && !f.IsDeleted, cancellationToken);

        if (document == null)
            throw new Exception("Document not found");

        // Toggle archive status
        document.IsArchived = request.IsArchived;
        document.ArchivedAt = request.IsArchived ? DateTime.UtcNow : (DateTime?)null;
        document.ArchivedBy = request.ArchivedBy;
        document.DateMod = DateTime.UtcNow;
        document.RowVersion++;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
// ✅ Delete Document Command Handler
public class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand, bool>
{
    private readonly FileDbContext _context;
    private readonly IFileStorageService _storageService;

    public DeleteDocumentCommandHandler(FileDbContext context, IFileStorageService storageService)
    {
        _context = context;
        _storageService = storageService;
    }

    public async Task<bool> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await _context.FileDocuments
            .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken);

        if (document == null)
            throw new Exception("File not found");

        if (request.PermanentlyDelete)
        {
            // Permanently delete from storage
            await _storageService.DeleteFileAsync(document.FilePath);

            // Delete thumbnail if exists
            if (!string.IsNullOrEmpty(document.ThumbnailPath))
                await _storageService.DeleteFileAsync(document.ThumbnailPath);

            // Remove from database
            _context.FileDocuments.Remove(document);
        }
        else
        {
            // Soft delete
            document.IsDeleted = true;
            document.DeletedAt = DateTime.UtcNow;
            document.DeletedBy = request.DeletedBy;
            document.DateMod = DateTime.UtcNow;
            document.RowVersion++;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}