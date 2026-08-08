// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Models\DTOs\FileDocumentDto.cs

using System;
using System.Collections.Generic;

namespace Cor.FileManagement.Models.DTOs;

public class FileDocumentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string? FileSizeFormatted { get; set; }
    public string FileType { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string? ThumbnailPath { get; set; }
    public string? Description { get; set; }
    public string Module { get; set; } = string.Empty;
    public Guid? ReferenceId { get; set; }
    public string? Category { get; set; }
    public string? DocumentType { get; set; }
    public bool IsPublic { get; set; }
    public bool IsShared { get; set; }
    public string? SharingLevel { get; set; }
    public bool IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }

    public int Version { get; set; }
    public Guid? FolderId { get; set; }
    public string? FolderName { get; set; }
    public string UploadedBy { get; set; } = string.Empty;
    public string? UploadedByName { get; set; }
    public DateTime UploadedAt { get; set; }
    public string? UploadedAtFormatted { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    public bool CanDownload { get; set; }
    public bool CanShare { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public DateTime DateAdd { get; set; }
     public bool IsFavorite { get; set; } = false;
}
public class ShareLinkResponseDto
{
    public string ShareUrl { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
     public string? CreatedByName { get; set; }
}
public class PublicDownloadResultDto
{
    public byte[] FileBytes { get; set; } = Array.Empty<byte>();
    public string ContentType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
}

public class PublicFileInfoDto
{
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FileSizeFormatted { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string UploadedBy { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
     public string CreatedByName { get; set; } = string.Empty;
}
public class FileDocumentCreateDto
{
    public string FileName { get; set; }
    public string Description { get; set; }
    public string Module { get; set; }
    public Guid? ReferenceId { get; set; }
    public string Category { get; set; }
    public string DocumentType { get; set; }
    public bool IsPublic { get; set; } = false;
    public bool IsShared { get; set; } = false;
    public string SharingLevel { get; set; } = "Private";
    public Guid? FolderId { get; set; }
    public Microsoft.AspNetCore.Http.IFormFile File { get; set; }
}

public class FileDocumentUpdateDto
{
    public string FileName { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public bool IsPublic { get; set; }
    public bool IsShared { get; set; }
    public string SharingLevel { get; set; }
    public Guid? FolderId { get; set; }
}


public class DebugTokenResultDto
{
    public string Token { get; set; } = string.Empty;
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    public Guid? DocumentId { get; set; }
    public string? FileName { get; set; }
    public string? OriginalFileName { get; set; }
    public string? FilePath { get; set; }
    public long? FileSize { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? UploadedBy { get; set; }
    public string? BasePath { get; set; }
    public string? ExpectedFullPath { get; set; }
    public bool FileExists { get; set; }
    public List<PathCheckResult> PathChecks { get; set; } = new();
}

public class PathCheckResult
{
    public string Path { get; set; } = string.Empty;
    public bool Exists { get; set; }
}

public class DocumentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string? OriginalFileName { get; set; }
    public long FileSize { get; set; }
    public string? FileType { get; set; }
    public string? FileExtension { get; set; }
    public string? FilePath { get; set; }
    public string? ThumbnailPath { get; set; }
    public string? Description { get; set; }
    public string? Module { get; set; }
    public string? ReferenceId { get; set; }
    public string? Category { get; set; }
    public string? DocumentType { get; set; }
    public bool IsPublic { get; set; }
    public bool IsShared { get; set; }
    public string? SharingLevel { get; set; }
    public bool IsArchived { get; set; }
    public int Version { get; set; }
    public Guid? FolderId { get; set; }
    public string? UploadedBy { get; set; }
    public DateTime UploadedAt { get; set; }
    public bool IsDeleted { get; set; }

    // ✅ Add this property
    public bool IsFavorite { get; set; } = false;
}