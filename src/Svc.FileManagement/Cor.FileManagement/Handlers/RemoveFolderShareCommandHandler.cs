// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Handlers\RemoveFolderShareCommandHandler.cs

using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.FileManagement.Persistence;
using Cor.FileManagement.Commands;

namespace Cor.FileManagement.Handlers;

public class RemoveFolderShareCommandHandler : IRequestHandler<RemoveFolderShareCommand, bool>
{
    private readonly FileDbContext _context;

    public RemoveFolderShareCommandHandler(FileDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(RemoveFolderShareCommand request, CancellationToken cancellationToken)
    {
        var share = await _context.FileShares
            .FirstOrDefaultAsync(s => s.Id == request.ShareId && s.FolderId == request.FolderId, cancellationToken);

        if (share == null)
            throw new Exception("Share not found");

        share.IsActive = false;
        share.DateMod = DateTime.UtcNow;
        share.RowVersion++;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}