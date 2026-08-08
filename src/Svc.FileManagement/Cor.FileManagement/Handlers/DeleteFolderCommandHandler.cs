// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Handlers\DeleteFolderCommandHandler.cs

using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.FileManagement.Persistence;
using Cor.FileManagement.Models.Entities;
using Cor.FileManagement.Commands;
using Cor.FileManagement.Queries;
using Cor.FileManagement.Handlers;

public class DeleteFolderCommandHandler : IRequestHandler<DeleteFolderCommand, bool>
{
    private readonly FileDbContext _context;

    public DeleteFolderCommandHandler(FileDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteFolderCommand request, CancellationToken cancellationToken)
    {
        var folder = await _context.FileFolders
            .Include(f => f.Children)
            .Include(f => f.Documents)
            .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken);

        if (folder == null)
            throw new Exception("Folder not found");

        // Check if folder has subfolders or documents
        if (folder.Children.Any(c => !c.IsDeleted) || folder.Documents.Any(d => !d.IsDeleted))
            throw new Exception("Cannot delete folder with contents. Please empty the folder first.");

        folder.IsDeleted = true;
        folder.DeletedAt = DateTime.UtcNow;
        folder.DeletedBy = request.DeletedBy;
        folder.DateMod = DateTime.UtcNow;
        folder.RowVersion++;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}