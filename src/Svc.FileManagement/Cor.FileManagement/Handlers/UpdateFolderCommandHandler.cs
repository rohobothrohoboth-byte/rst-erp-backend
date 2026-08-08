// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Handlers\UpdateFolderCommandHandler.cs

using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.FileManagement.Models.DTOs;
using Cor.FileManagement.Persistence;
using Cor.FileManagement.Models.Entities;
using Cor.FileManagement.Commands;
using Cor.FileManagement.Queries;
namespace Cor.FileManagement.Handlers;

public class UpdateFolderCommandHandler : IRequestHandler<UpdateFolderCommand, FileFolderDto>
{
    private readonly FileDbContext _context;

    public UpdateFolderCommandHandler(FileDbContext context)
    {
        _context = context;
    }

    public async Task<FileFolderDto> Handle(UpdateFolderCommand request, CancellationToken cancellationToken)
    {
        var folder = await _context.FileFolders
            .FirstOrDefaultAsync(f => f.Id == request.Id && !f.IsDeleted, cancellationToken);

        if (folder == null)
            throw new Exception("Folder not found");

        // Check for duplicate name in same parent
        var existing = await _context.FileFolders
            .FirstOrDefaultAsync(f => f.ParentId == folder.ParentId && f.Name == request.Name && f.Id != request.Id && !f.IsDeleted, cancellationToken);

        if (existing != null)
            throw new Exception("A folder with this name already exists in this location");

        folder.Name = request.Name;
        folder.Description = request.Description;
        folder.FolderType = request.FolderType ?? folder.FolderType;
        folder.IsPublic = request.IsPublic;
        folder.IsShared = request.IsShared;
        folder.SharingLevel = request.SharingLevel ?? "Private";
        folder.Order = request.Order;
        folder.LastModifiedAt = DateTime.UtcNow;
        folder.LastModifiedBy = request.ModifiedBy;
        folder.DateMod = DateTime.UtcNow;
        folder.RowVersion++;

        await _context.SaveChangesAsync(cancellationToken);

        return new FileFolderDto
        {
            Id = folder.Id,
            Name = folder.Name,
            Description = folder.Description,
            FolderType = folder.FolderType,
            ParentId = folder.ParentId,
            IsPublic = folder.IsPublic,
            IsShared = folder.IsShared,
            SharingLevel = folder.SharingLevel,
            IsArchived = folder.IsArchived,
            Order = folder.Order,
            CreatedAt = folder.CreatedAt,
            CanEdit = true,
            CanDelete = true,
            CanShare = true
        };
    }
}