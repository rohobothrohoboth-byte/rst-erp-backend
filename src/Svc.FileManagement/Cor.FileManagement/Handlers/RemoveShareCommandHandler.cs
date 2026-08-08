// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Handlers\RemoveShareCommandHandler.cs

using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.FileManagement.Persistence;
using Cor.FileManagement.Models.Entities;
using Cor.FileManagement.Commands;
using Cor.FileManagement.Queries;
namespace Cor.FileManagement.Handlers;

public class RemoveShareCommandHandler : IRequestHandler<RemoveShareCommand, bool>
{
    private readonly FileDbContext _context;

    public RemoveShareCommandHandler(FileDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(RemoveShareCommand request, CancellationToken cancellationToken)
    {
        var share = await _context.FileShares
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (share == null)
            throw new Exception("Share not found");

        share.IsActive = false;
        share.DateMod = DateTime.UtcNow;
        share.RowVersion++;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}