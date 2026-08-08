// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Commands\FileFolderCommands.cs

using MediatR;
using Cor.FileManagement.Models.DTOs;
namespace Cor.FileManagement.Commands;
public class CreateFolderCommand : IRequest<FileFolderDto>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string FolderType { get; set; }
     public string? Category { get; set; }
    public Guid? ParentId { get; set; }
    public bool IsPublic { get; set; }
    public bool IsShared { get; set; }
    public string SharingLevel { get; set; }
    public int Order { get; set; }
    public Guid CreatedBy { get; set; }

}

public class UpdateFolderCommand : IRequest<FileFolderDto>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string FolderType { get; set; }
    public bool IsPublic { get; set; }
    public bool IsShared { get; set; }
    public string SharingLevel { get; set; }
    public int Order { get; set; }
    public Guid ModifiedBy { get; set; }
}

public class DeleteFolderCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public Guid DeletedBy { get; set; }
}

