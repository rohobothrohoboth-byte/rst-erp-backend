// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Handlers\GetFileSharesByUserQueryHandler.cs

using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.FileManagement.Models.DTOs;
using Cor.FileManagement.Persistence;
using Cor.FileManagement.Models.Entities;
using Cor.FileManagement.Commands;
using Cor.FileManagement.Queries;
namespace Cor.FileManagement.Handlers;

public class GetFileSharesByUserQueryHandler : IRequestHandler<GetFileSharesByUserQuery, List<FileShareDto>>
{
    private readonly FileDbContext _context;

    public GetFileSharesByUserQueryHandler(FileDbContext context)
    {
        _context = context;
    }

    public async Task<List<FileShareDto>> Handle(GetFileSharesByUserQuery request, CancellationToken cancellationToken)
    {
        var query = _context.FileShares
            .Where(s => s.SharedWithId == request.UserId);

        if (request.ActiveOnly)
            query = query.Where(s => s.IsActive && (s.ExpiresAt == null || s.ExpiresAt > DateTime.UtcNow));

        var shares = await query
            .OrderByDescending(s => s.SharedAt)
            .ToListAsync(cancellationToken);

        return shares.Select(s => new FileShareDto
        {
            Id = s.Id,
            DocumentId = s.DocumentId,
            FolderId = s.FolderId,
            SharedWithId = s.SharedWithId,
            SharedWithType = s.SharedWithType,
            Permission = s.Permission,
            CanDownload = s.CanDownload,
            CanDelete = s.CanDelete,
            IsActive = s.IsActive,
            ExpiresAt = s.ExpiresAt,
            SharedAt = s.SharedAt
        }).ToList();
    }
}