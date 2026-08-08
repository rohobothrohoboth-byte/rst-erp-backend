// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Handlers\UpdateFileCommandHandler.cs

using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.FileManagement.Models.DTOs;
using Cor.FileManagement.Persistence;
using Cor.FileManagement.Services;
using Cor.FileManagement.Models.Entities;
using Cor.FileManagement.Commands;
using Cor.FileManagement.Queries;
namespace Cor.FileManagement.Handlers;

public class UpdateFileCommandHandler : IRequestHandler<UpdateFileCommand, FileDocumentDto>
{
    private readonly FileDbContext _context;
    private readonly IFileStorageService _storageService;
    private readonly IFileValidationService _validationService;
    private readonly IFileThumbnailService _thumbnailService;
    private readonly IConfiguration _configuration;

    public UpdateFileCommandHandler(
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

    public async Task<FileDocumentDto> Handle(UpdateFileCommand request, CancellationToken cancellationToken)
    {
        var document = await _context.FileDocuments
            .FirstOrDefaultAsync(f => f.Id == request.Id && !f.IsDeleted, cancellationToken);

        if (document == null)
            throw new Exception("File not found");

        // Update metadata
        if (!string.IsNullOrEmpty(request.FileName))
            document.FileName = request.FileName;

        if (!string.IsNullOrEmpty(request.Description))
            document.Description = request.Description;

        if (!string.IsNullOrEmpty(request.Category))
            document.Category = request.Category;

        document.IsPublic = request.IsPublic;
        document.IsShared = request.IsShared;
        document.SharingLevel = request.SharingLevel;
        document.FolderId = request.FolderId;
        document.LastModifiedAt = DateTime.UtcNow;
        document.LastModifiedBy = request.ModifiedBy;
        document.DateMod = DateTime.UtcNow;
        document.RowVersion++;

        // If new file is provided, replace the file
        if (request.File != null)
        {
            // Validate new file
            if (!_validationService.ValidateFile(request.File, out string errorMessage))
                throw new Exception(errorMessage);

            // Delete old file
            await _storageService.DeleteFileAsync(document.FilePath);

            // Save new file
            var uploadPath = _configuration["FileStorage:UploadPath"] ?? "uploads";
            var filePath = await _storageService.SaveFileAsync(
                request.File.OpenReadStream(),
                request.File.FileName,
                uploadPath);

            document.FilePath = filePath;
            document.FileSize = request.File.Length;
            document.FileType = request.File.ContentType;
            document.FileExtension = Path.GetExtension(request.File.FileName);
            document.DocumentType = _validationService.GetDocumentType(request.File.ContentType);
            document.Version++;

            // Generate new thumbnail
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath);
            var thumbnailDir = Path.Combine("wwwroot", _configuration["FileStorage:ThumbnailPath"] ?? "thumbnails");
            var thumbnailPath = await _thumbnailService.GenerateThumbnailAsync(fullPath, thumbnailDir);
            document.ThumbnailPath = thumbnailPath != null ? Path.GetRelativePath(Directory.GetCurrentDirectory(), thumbnailPath) : null;
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Log access
        await LogFileAccess(document.Id, request.ModifiedBy, "Update", cancellationToken);

        return MapToDto(document);
    }

    private async Task LogFileAccess(Guid documentId, Guid userId, string action, CancellationToken cancellationToken)
    {
        await _context.FileAccessLogs.AddAsync(new FileAccessLog
        {
            Id = Guid.NewGuid(),
            DocumentId = documentId,
            UserId = userId,
            Action = action,
            ActionType = "Write",
            AccessedAt = DateTime.UtcNow,
            DateAdd = DateTime.UtcNow
        }, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
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