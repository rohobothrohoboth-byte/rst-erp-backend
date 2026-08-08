// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Handlers\ShareFileCommandHandler.cs

using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.FileManagement.Models.Entities;
using Cor.FileManagement.Models.DTOs;
using Cor.FileManagement.Persistence;
using FileShareEntity = Cor.FileManagement.Models.Entities.FileShare;
using Cor.FileManagement.Commands;
using Cor.FileManagement.Queries;
namespace Cor.FileManagement.Handlers;

public class ShareFileCommandHandler : IRequestHandler<ShareFileCommand, FileShareDto>
{
    private readonly FileDbContext _context;

    public ShareFileCommandHandler(FileDbContext context)
    {
        _context = context;
    }

    public async Task<FileShareDto> Handle(ShareFileCommand request, CancellationToken cancellationToken)
    {
        // Validate: At least one of DocumentId or FolderId must be provided
        if (!request.DocumentId.HasValue && !request.FolderId.HasValue)
            throw new Exception("Either DocumentId or FolderId must be provided");

        // Check if document/folder exists
        if (request.DocumentId.HasValue)
        {
            var document = await _context.FileDocuments
                .FirstOrDefaultAsync(f => f.Id == request.DocumentId.Value && !f.IsDeleted, cancellationToken);

            if (document == null)
                throw new Exception("Document not found");
        }

        if (request.FolderId.HasValue)
        {
            var folder = await _context.FileFolders
                .FirstOrDefaultAsync(f => f.Id == request.FolderId.Value && !f.IsDeleted, cancellationToken);

            if (folder == null)
                throw new Exception("Folder not found");
        }

        // Check if share already exists
        var existingShare = await _context.FileShares
            .FirstOrDefaultAsync(s => s.DocumentId == request.DocumentId &&
                                      s.FolderId == request.FolderId &&
                                      s.SharedWithId == request.SharedWithId &&
                                      s.IsActive,
                              cancellationToken);

        if (existingShare != null)
            throw new Exception("This file/folder is already shared with this user");

        var share = new FileShareEntity
        {
            Id = Guid.NewGuid(),
            DocumentId = request.DocumentId,
            FolderId = request.FolderId,
            SharedWithId = request.SharedWithId,
            SharedWithType = request.SharedWithType ?? "User",
            Permission = request.Permission ?? "Read",
            CanDownload = request.CanDownload,
            CanDelete = request.CanDelete,
            IsActive = true,
            ExpiresAt = request.ExpiresAt,
            SharedBy = request.SharedBy,
            SharedAt = DateTime.UtcNow,
            DateAdd = DateTime.UtcNow,
            DateMod = DateTime.UtcNow
        };

        await _context.FileShares.AddAsync(share, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(share);
    }

    private FileShareDto MapToDto(FileShareEntity share)
    {
        return new FileShareDto
        {
            Id = share.Id,
            DocumentId = share.DocumentId,
            FolderId = share.FolderId,
            SharedWithId = share.SharedWithId,
            SharedWithType = share.SharedWithType,
            Permission = share.Permission,
            CanDownload = share.CanDownload,
            CanDelete = share.CanDelete,
            IsActive = share.IsActive,
            ExpiresAt = share.ExpiresAt,
            SharedAt = share.SharedAt
        };
    }
}