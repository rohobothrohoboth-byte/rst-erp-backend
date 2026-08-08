// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Commands\RemoveFolderShareCommand.cs

using MediatR;

namespace Cor.FileManagement.Commands;

public class RemoveFolderShareCommand : IRequest<bool>
{
    public Guid FolderId { get; set; }
    public Guid ShareId { get; set; }
    public Guid RemovedBy { get; set; }
}