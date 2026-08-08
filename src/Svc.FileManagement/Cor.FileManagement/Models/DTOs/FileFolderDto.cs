// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Models\DTOs\FileFolderDto.cs

using System;
using System.Collections.Generic;

namespace Cor.FileManagement.Models.DTOs;

public class FileFolderDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string FolderType { get; set; }
     public string? Category { get; set; }
    public Guid? ParentId { get; set; }
    public string ParentName { get; set; }
    public bool IsPublic { get; set; }
    public bool IsShared { get; set; }
    public string SharingLevel { get; set; }
    public bool IsArchived { get; set; }
    public int Order { get; set; }
    public int DocumentCount { get; set; }
    public int SubFolderCount { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    public bool CanShare { get; set; }
    public string Icon { get; set; }
    public string Color { get; set; }



    public List<FileFolderDto> SubFolders { get; set; }
    public List<FileDocumentDto> Documents { get; set; }
}

public class FileFolderCreateDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string FolderType { get; set; }
    public Guid? ParentId { get; set; }
    public bool IsPublic { get; set; } = false;
    public bool IsShared { get; set; } = false;
    public string SharingLevel { get; set; } = "Private";
    public int Order { get; set; } = 0;

}
public class FolderContentsDto
{
    public FileFolderDto? Folder { get; set; }
    public List<FileFolderDto> SubFolders { get; set; } = new();
    public List<FileDocumentDto> Documents { get; set; } = new();
}

