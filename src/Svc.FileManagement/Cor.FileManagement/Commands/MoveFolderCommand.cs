// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Commands\MoveFolderCommand.cs

using MediatR;
using Cor.FileManagement.Models.DTOs;

namespace Cor.FileManagement.Commands;

public class MoveFolderCommand : IRequest<FileFolderDto>
{
    public Guid Id { get; set; }
    public Guid? TargetParentId { get; set; }
}