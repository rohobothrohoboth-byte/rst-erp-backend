// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Commands\ShareFileCommand.cs

using MediatR;
using Cor.FileManagement.Models.DTOs;
namespace Cor.FileManagement.Commands;

public class ShareFileCommand : IRequest<FileShareDto>
{
    public Guid? DocumentId { get; set; }
    public Guid? FolderId { get; set; }
    public Guid SharedWithId { get; set; }
    public string SharedWithType { get; set; } = "User";
    public string Permission { get; set; } = "Read";
    public bool CanDownload { get; set; } = true;
    public bool CanDelete { get; set; } = false;
    public DateTime? ExpiresAt { get; set; }
    public Guid SharedBy { get; set; }
}