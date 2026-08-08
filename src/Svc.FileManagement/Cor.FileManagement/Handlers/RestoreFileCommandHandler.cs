// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Handlers\RestoreFileCommandHandler.cs

using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.FileManagement.Persistence;
using Cor.FileManagement.Models.Entities;
using Cor.FileManagement.Commands;
using Cor.FileManagement.Queries;
namespace Cor.FileManagement.Handlers;

public class RestoreFileCommandHandler : IRequestHandler<RestoreFileCommand, bool>
{
    private readonly FileDbContext _context;

    public RestoreFileCommandHandler(FileDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(RestoreFileCommand request, CancellationToken cancellationToken)
    {
        var document = await _context.FileDocuments
            .FirstOrDefaultAsync(f => f.Id == request.Id && f.IsDeleted, cancellationToken);

        if (document == null)
            throw new Exception("File not found in trash");

        document.IsDeleted = false;
        document.DeletedAt = null;
        document.DeletedBy = null;
        document.DateMod = DateTime.UtcNow;
        document.RowVersion++;

        await _context.SaveChangesAsync(cancellationToken);

        // Log access
        await LogFileAccess(document.Id, request.RestoredBy, "Restore", cancellationToken);

        return true;
    }

    private async Task LogFileAccess(Guid documentId, Guid userId, string action, CancellationToken cancellationToken)
    {
        await _context.FileAccessLogs.AddAsync(new FileAccessLog
        {
            Id = Guid.NewGuid(),
            DocumentId = documentId,
            UserId = userId,
            Action = action,
            ActionType = "Write",
            AccessedAt = DateTime.UtcNow,
            DateAdd = DateTime.UtcNow
        }, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}