// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Handlers\ArchiveFileCommandHandler.cs

using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.FileManagement.Persistence;
using Cor.FileManagement.Models.Entities;
using Cor.FileManagement.Commands;
using Cor.FileManagement.Queries;
namespace Cor.FileManagement.Handlers;

public class ArchiveFileCommandHandler : IRequestHandler<ArchiveFileCommand, bool>
{
    private readonly FileDbContext _context;

    public ArchiveFileCommandHandler(FileDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ArchiveFileCommand request, CancellationToken cancellationToken)
    {
        var document = await _context.FileDocuments
            .FirstOrDefaultAsync(f => f.Id == request.Id && !f.IsDeleted, cancellationToken);

        if (document == null)
            throw new Exception("File not found");

        document.IsArchived = !document.IsArchived;
        document.ArchivedAt = document.IsArchived ? DateTime.UtcNow : null;
        document.ArchivedBy = document.IsArchived ? request.ArchivedBy : null;
        document.DateMod = DateTime.UtcNow;
        document.RowVersion++;

        await _context.SaveChangesAsync(cancellationToken);

        // Log access
        await LogFileAccess(document.Id, request.ArchivedBy, document.IsArchived ? "Archive" : "Unarchive", cancellationToken);

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