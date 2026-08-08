// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Commands\DocumentCommands.cs

using MediatR;
using Microsoft.AspNetCore.Http;
using Cor.FileManagement.Models.DTOs;

namespace Cor.FileManagement.Commands;

public class UploadDocumentCommand : IRequest<FileDocumentDto>
{
    public IFormFile? File { get; set; }
    public string Module { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public Guid? ReferenceId { get; set; }
    public string? Category { get; set; }
     public string? DocumentType { get; set; }
    public string? Description { get; set; }
    public bool IsPublic { get; set; } = false;
    public bool IsShared { get; set; } = false;
    public string? SharingLevel { get; set; } = "Private";
    public Guid? FolderId { get; set; }
    public Guid UploadedBy { get; set; }
}
public class ArchiveDocumentCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public bool IsArchived { get; set; }
     public Guid ArchivedBy { get; set; }
}
public class UpdateDocumentCommand : IRequest<FileDocumentDto>
{
    public Guid Id { get; set; }
    public string? FileName { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public bool IsPublic { get; set; }
    public bool IsShared { get; set; }
    public string? SharingLevel { get; set; }
    public Guid? FolderId { get; set; }
    public Guid ModifiedBy { get; set; }
}

public class DeleteDocumentCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public Guid DeletedBy { get; set; }
    public bool PermanentlyDelete { get; set; } = false;
}

public class RestoreDocumentCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public Guid RestoredBy { get; set; }
}



public class ToggleFavoriteCommand : IRequest<bool>
{
    public Guid DocumentId { get; set; }
    public Guid UserId { get; set; }
}



// ✅ FIXED: DownloadDocumentCommand implements IRequest<DownloadResultDto>
public class DownloadDocumentCommand : IRequest<DownloadResultDto>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}