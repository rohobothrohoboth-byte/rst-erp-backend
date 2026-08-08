// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Queries\FileDocumentQueries.cs

using MediatR;
using Cor.FileManagement.Models.DTOs;
using System.Collections.Generic;
using Cor.FileManagement.Models.Entities;
namespace Cor.FileManagement.Queries;
public class GetFileQuery : IRequest<FileDocumentDto>
{
    public Guid Id { get; set; }
}
public class GetPublicFileInfoQuery : IRequest<PublicFileInfoDto>
{
    public string Token { get; set; } = string.Empty;
}
public class GetFilesQuery : IRequest<List<FileDocumentDto>>
{
    public string Module { get; set; }
    public Guid? ReferenceId { get; set; }
    public string Category { get; set; }
    public string DocumentType { get; set; }
    public Guid? FolderId { get; set; }
    public bool IncludeArchived { get; set; } = false;
    public bool IncludeDeleted { get; set; } = false;
    public string SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class GetFileByReferenceQuery : IRequest<List<FileDocumentDto>>
{
    public string Module { get; set; }
    public Guid ReferenceId { get; set; }
    public string Category { get; set; }
}

public class GetRecentFilesQuery : IRequest<List<FileDocumentDto>>
{
    public Guid UserId { get; set; }
    public int Count { get; set; } = 10;
}

public class GetTokenInfoQuery : IRequest<FileShareToken?>
{
    public string Token { get; set; } = string.Empty;
}