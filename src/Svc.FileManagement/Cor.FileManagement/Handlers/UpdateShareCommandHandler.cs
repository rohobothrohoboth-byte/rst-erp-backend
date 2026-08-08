// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Handlers\UpdateShareCommandHandler.cs

using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.FileManagement.Models.DTOs;
using Cor.FileManagement.Persistence;
using Cor.FileManagement.Models.Entities;
using Cor.FileManagement.Commands;
using Cor.FileManagement.Queries;
namespace Cor.FileManagement.Handlers;

public class UpdateShareCommandHandler : IRequestHandler<UpdateShareCommand, FileShareDto>
{
    private readonly FileDbContext _context;

    public UpdateShareCommandHandler(FileDbContext context)
    {
        _context = context;
    }

    public async Task<FileShareDto> Handle(UpdateShareCommand request, CancellationToken cancellationToken)
    {
        var share = await _context.FileShares
            .FirstOrDefaultAsync(s => s.Id == request.Id && s.IsActive, cancellationToken);

        if (share == null)
            throw new Exception("Share not found");

        share.Permission = request.Permission ?? share.Permission;
        share.CanDownload = request.CanDownload;
        share.CanDelete = request.CanDelete;
        share.ExpiresAt = request.ExpiresAt;
        share.DateMod = DateTime.UtcNow;
        share.RowVersion++;

        await _context.SaveChangesAsync(cancellationToken);

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