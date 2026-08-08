// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Commands\FileDocumentCommands.cs

using MediatR;
using Microsoft.AspNetCore.Http;
using Cor.FileManagement.Models.DTOs;
namespace Cor.FileManagement.Commands;
public class UploadFileCommand : IRequest<FileDocumentDto>
{
    public string Module { get; set; }
    public Guid? ReferenceId { get; set; }
    public string Category { get; set; }
    public string DocumentType { get; set; }
    public string Description { get; set; }
    public bool IsPublic { get; set; }
    public bool IsShared { get; set; }
    public string SharingLevel { get; set; }
    public Guid? FolderId { get; set; }
     public string FileName { get; set; }
    public IFormFile File { get; set; }
    public Guid UploadedBy { get; set; }
    public string? UploadedByName { get; set; }
}
public class GenerateShareLinkCommand : IRequest<GenerateShareLinkResult>
{
    public Guid DocumentId { get; set; }
    public Guid UserId { get; set; }
    public string? CreatedByName { get; set; } // ✅ Add this property
}


public class GenerateShareLinkResult
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string? CreatedByName { get; set; } // ✅ Add this property
}
public class UpdateFileCommand : IRequest<FileDocumentDto>
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public bool IsPublic { get; set; }
    public bool IsShared { get; set; }
    public string SharingLevel { get; set; }
    public Guid? FolderId { get; set; }
    public IFormFile File { get; set; } // Optional - for replacing file
    public Guid ModifiedBy { get; set; }
}

public class DeleteFileCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public Guid DeletedBy { get; set; }
    public bool PermanentlyDelete { get; set; } = false;
}

public class RestoreFileCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public Guid RestoredBy { get; set; }
}

public class ArchiveFileCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public Guid ArchivedBy { get; set; }
}



public class CopyFileCommand : IRequest<FileDocumentDto>
{
    public Guid Id { get; set; }
    public Guid? TargetFolderId { get; set; }
}