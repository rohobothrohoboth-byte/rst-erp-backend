// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Commands\MoveFileCommand.cs

using MediatR;
using Cor.FileManagement.Models.DTOs;

namespace Cor.FileManagement.Commands;

public class MoveFileCommand : IRequest<FileDocumentDto>
{
    public Guid Id { get; set; }
    public Guid? TargetFolderId { get; set; }
}