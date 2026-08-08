// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Handlers\DeleteFileCommandHandler.cs

using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.FileManagement.Persistence;
using Cor.FileManagement.Services;
using Cor.FileManagement.Models.Entities;
using Cor.FileManagement.Commands;
using Cor.FileManagement.Queries;
namespace Cor.FileManagement.Handlers;

public class DeleteFileCommandHandler : IRequestHandler<DeleteFileCommand, bool>
{
    private readonly FileDbContext _context;
    private readonly IFileStorageService _storageService;

    public DeleteFileCommandHandler(FileDbContext context, IFileStorageService storageService)
    {
        _context = context;
        _storageService = storageService;
    }

    public async Task<bool> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
    {
        var document = await _context.FileDocuments
            .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken);

        if (document == null)
            throw new Exception("File not found");

        if (request.PermanentlyDelete)
        {
            // Permanently delete from storage
            await _storageService.DeleteFileAsync(document.FilePath);

            // Delete thumbnail if exists
            if (!string.IsNullOrEmpty(document.ThumbnailPath))
                await _storageService.DeleteFileAsync(document.ThumbnailPath);

            // Remove from database
            _context.FileDocuments.Remove(document);
        }
        else
        {
            // Soft delete
            document.IsDeleted = true;
            document.DeletedAt = DateTime.UtcNow;
            document.DeletedBy = request.DeletedBy;
            document.DateMod = DateTime.UtcNow;
            document.RowVersion++;
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Log access
        await LogFileAccess(document.Id, request.DeletedBy, request.PermanentlyDelete ? "PermanentDelete" : "SoftDelete", cancellationToken);

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
            ActionType = "Delete",
            AccessedAt = DateTime.UtcNow,
            DateAdd = DateTime.UtcNow
        }, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}