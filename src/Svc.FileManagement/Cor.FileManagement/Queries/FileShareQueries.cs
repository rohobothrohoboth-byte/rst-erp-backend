// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Queries\FileShareQueries.cs

using MediatR;
using Cor.FileManagement.Models.DTOs;
using System.Collections.Generic;
namespace Cor.FileManagement.Queries;
public class GetFileSharesQuery : IRequest<List<FileShareDto>>
{
    public Guid? DocumentId { get; set; }
    public Guid? FolderId { get; set; }
    public Guid? SharedWithId { get; set; }
    public bool IncludeExpired { get; set; } = false;
}

public class GetFileSharesByUserQuery : IRequest<List<FileShareDto>>
{
    public Guid UserId { get; set; }
    public bool ActiveOnly { get; set; } = true;
}
public class GetSharedWithMeQuery : IRequest<List<SharedFileDto>>
{
    public Guid UserId { get; set; }
}

public class GetMySharesQuery : IRequest<List<SharedFileDto>>
{
    public Guid UserId { get; set; }
}

public class GetFolderSharesQuery : IRequest<List<FolderShareDto>>
{
    public Guid FolderId { get; set; }
}

