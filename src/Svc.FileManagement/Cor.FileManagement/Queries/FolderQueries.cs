// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Queries\FolderQueries.cs

using MediatR;
using Cor.FileManagement.Models.DTOs;

namespace Cor.FileManagement.Queries;

public class GetRootFoldersQuery : IRequest<List<FileFolderDto>>
{
    public Guid? UserId { get; set; }
}

public class GetFoldersQuery : IRequest<List<FileFolderDto>>
{
    public Guid? ParentId { get; set; }
    public string? FolderType { get; set; }
    public bool IncludeArchived { get; set; } = false;
}

public class GetFolderByIdQuery : IRequest<FileFolderDto>
{
    public Guid Id { get; set; }
}

// ✅ NEW: Get folder contents (sub-folders and documents)
public class GetFolderContentsQuery : IRequest<FolderContentsDto>
{
    public Guid FolderId { get; set; }
    public Guid? UserId { get; set; }
}

// ✅ NEW: Get sub-folders only
public class GetSubFoldersQuery : IRequest<List<FileFolderDto>>
{
    public Guid FolderId { get; set; }
    public Guid? UserId { get; set; }
}

// ✅ NEW: Get documents in folder
public class GetFolderDocumentsQuery : IRequest<List<FileDocumentDto>>
{
    public Guid FolderId { get; set; }
    public Guid? UserId { get; set; }
}