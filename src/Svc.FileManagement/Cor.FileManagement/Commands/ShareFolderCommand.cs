// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Commands\ShareFolderCommand.cs

using MediatR;
using Cor.FileManagement.Models.DTOs;

namespace Cor.FileManagement.Commands;

public class ShareFolderCommand : IRequest<FolderShareDto>
{
    public Guid FolderId { get; set; }
    public string SharedWithId { get; set; } = string.Empty;
    public string SharedWithType { get; set; } = "User";
    public string Permission { get; set; } = "Read";
    public bool CanDownload { get; set; }
    public bool CanDelete { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public Guid SharedBy { get; set; }
}