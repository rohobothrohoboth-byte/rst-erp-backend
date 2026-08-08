// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Handlers\UploadFileCommandHandler.cs

using System.Security.Cryptography;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Cor.FileManagement.Models.Entities;
using Cor.FileManagement.Models.DTOs;
using Cor.FileManagement.Persistence;
using Cor.FileManagement.Services;
using Cor.FileManagement.Commands;
using Cor.FileManagement.Queries;

namespace Cor.FileManagement.Handlers;

public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, FileDocumentDto>
{
    private readonly FileDbContext _context;
    private readonly IFileStorageService _storageService;
    private readonly IFileValidationService _validationService;
    private readonly IFileThumbnailService _thumbnailService;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<UploadFileCommandHandler> _logger;

    public UploadFileCommandHandler(
        FileDbContext context,
        IFileStorageService storageService,
        IFileValidationService validationService,
        IFileThumbnailService thumbnailService,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        ILogger<UploadFileCommandHandler> logger)
    {
        _context = context;
        _storageService = storageService;
        _validationService = validationService;
        _thumbnailService = thumbnailService;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<FileDocumentDto> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
        // ✅ Use both Console.WriteLine AND ILogger
        var message = "========================================";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        message = "📤 [Upload] STARTING FILE UPLOAD";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        message = $"   File: {request.File.FileName}";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        message = $"   Size: {request.File.Length} bytes";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        message = $"   Type: {request.File.ContentType}";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        message = "========================================";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        // Validate file
        if (!_validationService.ValidateFile(request.File, out string errorMessage))
            throw new Exception(errorMessage);

        // Check if folder exists if specified
        if (request.FolderId.HasValue)
        {
            var folder = await _context.FileFolders
                .FirstOrDefaultAsync(f => f.Id == request.FolderId.Value && !f.IsDeleted, cancellationToken);

            if (folder == null)
                throw new Exception("Folder not found");
        }

        // Generate file hash
        string? fileHash = null;
        try
        {
            using var md5 = MD5.Create();
            using var stream = request.File.OpenReadStream();
            var hashBytes = await md5.ComputeHashAsync(stream, cancellationToken);
            fileHash = Convert.ToBase64String(hashBytes);
            stream.Position = 0;
        }
        catch
        {
            fileHash = null;
        }

        // ============================================================
        // STEP 1: SAVE FILE
        // ============================================================
        var uploadPath = _configuration["FileStorage:UploadPath"] ?? "uploads";
        message = $"📁 [Upload] Upload path: '{uploadPath}'";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        var filePath = await _storageService.SaveFileAsync(
            request.File.OpenReadStream(),
            request.File.FileName,
            uploadPath);

        message = $"📁 [Upload] File path from storage service: '{filePath}'";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        // ✅ FIX: Remove duplicate uploads folder if present
        if (filePath.StartsWith("uploads/uploads/") || filePath.StartsWith("uploads\\uploads\\"))
        {
            filePath = filePath.Replace("uploads/uploads/", "uploads/")
                               .Replace("uploads\\uploads\\", "uploads\\");
            message = $"✅ [Upload] Fixed duplicate path: '{filePath}'";
            Console.WriteLine(message);
            _logger.LogInformation(message);
        }

        // ✅ Ensure forward slashes
        filePath = filePath.Replace("\\", "/");
        message = $"✅ [Upload] File path after fixing slashes: '{filePath}'";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        // ============================================================
        // STEP 2: GENERATE THUMBNAIL
        // ============================================================
        string? thumbnailPath = null;
        try
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath);
            var thumbnailDir = _configuration["FileStorage:ThumbnailPath"] ?? "thumbnails";

            message = $"📁 [Upload] Full file path: '{fullPath}'";
            Console.WriteLine(message);
            _logger.LogInformation(message);

            message = $"📁 [Upload] Thumbnail directory: '{thumbnailDir}'";
            Console.WriteLine(message);
            _logger.LogInformation(message);

            message = $"📁 [Upload] File exists: {File.Exists(fullPath)}";
            Console.WriteLine(message);
            _logger.LogInformation(message);

            var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff" };
            var extension = Path.GetExtension(request.File.FileName).ToLower();

            message = $"📁 [Upload] File extension: '{extension}'";
            Console.WriteLine(message);
            _logger.LogInformation(message);

            message = $"📁 [Upload] Is image: {imageExtensions.Contains(extension)}";
            Console.WriteLine(message);
            _logger.LogInformation(message);

            if (imageExtensions.Contains(extension) && File.Exists(fullPath))
            {
                message = "🖼️ [Upload] Generating thumbnail...";
                Console.WriteLine(message);
                _logger.LogInformation(message);

                thumbnailPath = await _thumbnailService.GenerateThumbnailAsync(fullPath, thumbnailDir);

                message = $"🖼️ [Upload] Thumbnail path from service: '{thumbnailPath}'";
                Console.WriteLine(message);
                _logger.LogInformation(message);

                if (!string.IsNullOrEmpty(thumbnailPath))
                {
                    // ✅ FORCE FORWARD SLASHES
                    thumbnailPath = thumbnailPath.Replace("\\", "/");
                    message = $"✅ [Upload] Thumbnail path after fixing slashes: '{thumbnailPath}'";
                    Console.WriteLine(message);
                    _logger.LogInformation(message);
                }
            }
            else
            {
                message = "ℹ️ [Upload] Skipping thumbnail - not an image or file not found";
                Console.WriteLine(message);
                _logger.LogInformation(message);
            }
        }
        catch (Exception ex)
        {
            message = $"❌ [Upload] Thumbnail generation failed: {ex.Message}";
            Console.WriteLine(message);
            _logger.LogError(ex, message);
            thumbnailPath = null;
        }

        // ============================================================
        // STEP 3: LOG FINAL VALUES
        // ============================================================
        message = "========================================";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        message = "💾 [Upload] FINAL VALUES BEFORE SAVING:";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        message = $"   FilePath: '{filePath}'";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        message = $"   ThumbnailPath: '{thumbnailPath}'";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        message = "========================================";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        // Get user name
        string? uploadedByName = null;
        try
        {
            uploadedByName = _httpContextAccessor.HttpContext?.User?.FindFirst("userName")?.Value
                ?? _httpContextAccessor.HttpContext?.User?.FindFirst("name")?.Value
                ?? "System";
        }
        catch
        {
            uploadedByName = "System";
        }

        // Get file name
        var fileName = !string.IsNullOrEmpty(request.FileName) ? request.FileName : request.File.FileName;

        // ============================================================
        // STEP 4: CREATE ENTITY
        // ============================================================
        message = "📝 [Upload] Creating document entity...";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        var document = new FileDocument
        {
            Id = Guid.NewGuid(),
            FileName = fileName,
            OriginalFileName = request.File.FileName,
            FileSize = request.File.Length,
            FileType = request.File.ContentType,
            FileExtension = Path.GetExtension(request.File.FileName),
            FilePath = filePath,
            ThumbnailPath = thumbnailPath,
            Description = request.Description,
            Module = request.Module,
            ReferenceId = request.ReferenceId,
            Category = request.Category,
            DocumentType = _validationService.GetDocumentType(request.File.ContentType),
            IsPublic = request.IsPublic,
            IsShared = request.IsShared,
            SharingLevel = request.SharingLevel ?? "Private",
            FolderId = request.FolderId,
            UploadedBy = request.UploadedBy,
            UploadedByName = uploadedByName,
            UploadedAt = DateTime.UtcNow,
            Hash = fileHash,
            DateAdd = DateTime.UtcNow,
            DateMod = DateTime.UtcNow,
            Version = 1,
            IsDeleted = false,
            IsArchived = false,
            StorageProvider = "Local"
        };

        message = $"📝 [Upload] Document entity created:";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        message = $"   Document.FilePath: '{document.FilePath}'";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        message = $"   Document.ThumbnailPath: '{document.ThumbnailPath}'";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        // ============================================================
        // STEP 5: SAVE TO DATABASE
        // ============================================================
        message = "💾 [Upload] Saving to database...";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        await _context.FileDocuments.AddAsync(document, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        message = "✅ [Upload] Saved to database successfully!";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        message = $"   Document ID: {document.Id}";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        message = $"   Document.FilePath: '{document.FilePath}'";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        message = $"   Document.ThumbnailPath: '{document.ThumbnailPath}'";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        // Log access
        await LogFileAccess(document.Id, request.UploadedBy, "Upload", cancellationToken);

        // ============================================================
        // STEP 6: MAP TO DTO
        // ============================================================
        message = "📤 [Upload] Mapping to DTO...";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        var result = MapToDto(document);

        message = $"📤 [Upload] DTO created:";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        message = $"   DTO.FilePath: '{result.FilePath}'";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        message = $"   DTO.ThumbnailPath: '{result.ThumbnailPath}'";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        message = "========================================";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        message = "✅ [Upload] COMPLETE! File uploaded successfully.";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        message = "========================================";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        return result;
    }

    private async Task LogFileAccess(Guid documentId, Guid userId, string action, CancellationToken cancellationToken)
    {
        // ✅ Get IP Address from HttpContext
        string? ipAddress = null;
        try
        {
            ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            if (string.IsNullOrEmpty(ipAddress))
                ipAddress = "0.0.0.0";
        }
        catch
        {
            ipAddress = "0.0.0.0";
        }

        // ✅ Get User Agent
        string? userAgent = null;
        try
        {
            userAgent = _httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString();
            if (string.IsNullOrEmpty(userAgent))
                userAgent = "Unknown";
        }
        catch
        {
            userAgent = "Unknown";
        }

        await _context.FileAccessLogs.AddAsync(new FileAccessLog
        {
            Id = Guid.NewGuid(),
            DocumentId = documentId,
            UserId = userId,
            Action = action,
            ActionType = "Write",
            IPAddress = ipAddress,
            UserAgent = userAgent,
            AccessedAt = DateTime.UtcNow,
            DateAdd = DateTime.UtcNow
        }, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private FileDocumentDto MapToDto(FileDocument document)
    {
        var message = "📤 [MapToDto] Mapping document to DTO:";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        message = $"   Source FilePath: '{document.FilePath}'";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        message = $"   Source ThumbnailPath: '{document.ThumbnailPath}'";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        var dto = new FileDocumentDto
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

        message = $"📤 [MapToDto] DTO created:";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        message = $"   DTO.FilePath: '{dto.FilePath}'";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        message = $"   DTO.ThumbnailPath: '{dto.ThumbnailPath}'";
        Console.WriteLine(message);
        _logger.LogInformation(message);

        return dto;
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