// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Queries\DocumentQueries.cs

using MediatR;
using Cor.FileManagement.Models.DTOs;

namespace Cor.FileManagement.Queries;

public class GetDocumentsQuery : IRequest<List<FileDocumentDto>>
{
    public string? Module { get; set; }
    public Guid? ReferenceId { get; set; }
    public string? Category { get; set; }
    public string? DocumentType { get; set; }
    public Guid? FolderId { get; set; }
    public bool IncludeArchived { get; set; } = false;
    public bool IncludeDeleted { get; set; } = false;
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "UploadedAt";
    public bool SortDescending { get; set; } = true;
}

public class GetDocumentByIdQuery : IRequest<FileDocumentDto>
{
    public Guid Id { get; set; }
}

public class GetDocumentsByModuleQuery : IRequest<List<FileDocumentDto>>
{
    public string Module { get; set; }
    public Guid? ReferenceId { get; set; }
    public string? Category { get; set; }
}

public class GetDocumentsByFolderQuery : IRequest<List<FileDocumentDto>>
{
    public Guid FolderId { get; set; }
}

public class GetFavoriteDocumentsQuery : IRequest<List<FileDocumentDto>>
{
    public Guid UserId { get; set; }
}

public class GetRecentDocumentsQuery : IRequest<List<FileDocumentDto>>
{
    public Guid UserId { get; set; }
    public int Limit { get; set; } = 10;
}

public class PublicDownloadQuery : IRequest<PublicDownloadResult>
{
    public string Token { get; set; } = string.Empty;
}
public class PublicDownloadResult
{
    public byte[]? FileBytes { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
}
public class DebugTokenQuery : IRequest<DebugTokenResultDto>
{
    public string Token { get; set; } = string.Empty;
}
public class GetDocumentsByModuleAndReferenceQuery : IRequest<List<FileDocumentDto>>
{
    public string Module { get; set; }
    public Guid ReferenceId { get; set; }
    public string? Category { get; set; }
}

