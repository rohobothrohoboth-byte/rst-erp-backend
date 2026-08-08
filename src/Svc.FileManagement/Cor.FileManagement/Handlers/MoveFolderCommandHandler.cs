// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Handlers\MoveFolderCommandHandler.cs

using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.FileManagement.Models.DTOs;
using Cor.FileManagement.Persistence;
using Cor.FileManagement.Models.Entities;
using Cor.FileManagement.Commands;
using Cor.FileManagement.Queries;
namespace Cor.FileManagement.Handlers;

public class MoveFolderCommandHandler : IRequestHandler<MoveFolderCommand, FileFolderDto>
{
    private readonly FileDbContext _context;

    public MoveFolderCommandHandler(FileDbContext context)
    {
        _context = context;
    }

    public async Task<FileFolderDto> Handle(MoveFolderCommand request, CancellationToken cancellationToken)
    {
        var folder = await _context.FileFolders
            .FirstOrDefaultAsync(f => f.Id == request.Id && !f.IsDeleted, cancellationToken);

        if (folder == null)
            throw new Exception("Folder not found");

        // Check if target parent exists
        if (request.TargetParentId.HasValue)
        {
            var parent = await _context.FileFolders
                .FirstOrDefaultAsync(f => f.Id == request.TargetParentId.Value && !f.IsDeleted, cancellationToken);

            if (parent == null)
                throw new Exception("Target folder not found");

            // Prevent circular reference
            if (parent.ParentId == folder.Id)
                throw new Exception("Cannot move folder into its own subfolder");
        }

        // Check for duplicate name in target parent
        var existing = await _context.FileFolders
            .FirstOrDefaultAsync(f => f.ParentId == request.TargetParentId && f.Name == folder.Name && f.Id != request.Id && !f.IsDeleted, cancellationToken);

        if (existing != null)
            throw new Exception("A folder with this name already exists in the target location");

        folder.ParentId = request.TargetParentId;
        folder.DateMod = DateTime.UtcNow;
        folder.RowVersion++;

        await _context.SaveChangesAsync(cancellationToken);

        return new FileFolderDto
        {
            Id = folder.Id,
            Name = folder.Name,
            ParentId = folder.ParentId
        };
    }
}