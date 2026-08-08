// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Models\DTOs\FileShareDto.cs

using System;

namespace Cor.FileManagement.Models.DTOs;

public class FileShareDto
{
    public Guid Id { get; set; }
    public Guid? DocumentId { get; set; }
    public string DocumentName { get; set; }
    public Guid? FolderId { get; set; }
    public string FolderName { get; set; }
    public Guid SharedWithId { get; set; }
    public string SharedWithName { get; set; }
    public string SharedWithType { get; set; }
    public string Permission { get; set; }
    public bool CanDownload { get; set; }
    public bool CanDelete { get; set; }
    public bool IsActive { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string SharedBy { get; set; }
    public DateTime SharedAt { get; set; }
}

public class FileShareCreateDto
{
    public Guid? DocumentId { get; set; }
    public Guid? FolderId { get; set; }
    public Guid SharedWithId { get; set; }
    public string SharedWithType { get; set; } = "User";
    public string Permission { get; set; } = "Read";
    public bool CanDownload { get; set; } = true;
    public bool CanDelete { get; set; } = false;
    public DateTime? ExpiresAt { get; set; }
}



public class FolderShareCreateDto
{
    public string SharedWithId { get; set; } = string.Empty;
    public string SharedWithType { get; set; } = "user";
    public string Permission { get; set; } = "view";
    public bool CanDownload { get; set; }
    public bool CanDelete { get; set; }
    public DateTime? ExpiresAt { get; set; }
}



public class FolderShareDto
{
    public Guid Id { get; set; }
    public Guid FolderId { get; set; }
    public string FolderName { get; set; } = string.Empty;
    public string SharedWithId { get; set; } = string.Empty;
    public string SharedWithName { get; set; } = string.Empty;
    public string SharedWithType { get; set; } = string.Empty;
    public string Permission { get; set; } = string.Empty;
    public bool CanDownload { get; set; }
    public bool CanDelete { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime SharedAt { get; set; }
    public string SharedBy { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class SharedFileDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // file or folder
    public string SharedBy { get; set; } = string.Empty;
    public string Permission { get; set; } = string.Empty;
    public DateTime SharedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public bool CanDownload { get; set; }
    public bool CanDelete { get; set; }
}