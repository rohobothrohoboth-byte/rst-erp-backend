// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Handlers\GetFileSharesQueryHandler.cs

using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.FileManagement.Models.DTOs;
using Cor.FileManagement.Persistence;
using Cor.FileManagement.Models.Entities;
using Cor.FileManagement.Commands;
using Cor.FileManagement.Queries;
namespace Cor.FileManagement.Handlers;

public class GetFileSharesQueryHandler : IRequestHandler<GetFileSharesQuery, List<FileShareDto>>
{
    private readonly FileDbContext _context;

    public GetFileSharesQueryHandler(FileDbContext context)
    {
        _context = context;
    }

    public async Task<List<FileShareDto>> Handle(GetFileSharesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.FileShares.AsQueryable();

        if (request.DocumentId.HasValue)
            query = query.Where(s => s.DocumentId == request.DocumentId);

        if (request.FolderId.HasValue)
            query = query.Where(s => s.FolderId == request.FolderId);

        if (request.SharedWithId.HasValue)
            query = query.Where(s => s.SharedWithId == request.SharedWithId);

        if (!request.IncludeExpired)
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