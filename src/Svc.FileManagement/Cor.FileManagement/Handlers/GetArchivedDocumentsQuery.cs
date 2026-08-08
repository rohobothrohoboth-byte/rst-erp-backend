// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Queries\GetArchivedDocumentsQuery.cs

using MediatR;
using Cor.FileManagement.Models.DTOs;

namespace Cor.FileManagement.Queries;

public class GetArchivedDocumentsQuery : IRequest<List<FileDocumentDto>>
{
    public Guid UserId { get; set; }
    public string? SearchTerm { get; set; }
}