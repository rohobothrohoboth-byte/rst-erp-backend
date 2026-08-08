// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Handlers\ToggleFavoriteCommandHandler.cs

using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.FileManagement.Persistence;
using Cor.FileManagement.Models.Entities;
using Cor.FileManagement.Commands;

namespace Cor.FileManagement.Handlers;

public class ToggleFavoriteCommandHandler : IRequestHandler<ToggleFavoriteCommand, bool>
{
    private readonly FileDbContext _context;

    public ToggleFavoriteCommandHandler(FileDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ToggleFavoriteCommand request, CancellationToken cancellationToken)
    {
        var document = await _context.FileDocuments
            .FirstOrDefaultAsync(f => f.Id == request.DocumentId && !f.IsDeleted, cancellationToken);

        if (document == null)
            throw new Exception("Document not found");

        // Check if already favorited
        var favorite = await _context.FileFavorites
            .FirstOrDefaultAsync(f => f.DocumentId == request.DocumentId && f.UserId == request.UserId, cancellationToken);

        if (favorite != null)
        {
            // Remove favorite
            _context.FileFavorites.Remove(favorite);
            await _context.SaveChangesAsync(cancellationToken);
            return false; // Unfavorited
        }
        else
        {
            // Add favorite
            var newFavorite = new FileFavorite
            {
                Id = Guid.NewGuid(),
                DocumentId = request.DocumentId,
                UserId = request.UserId,
                CreatedAt = DateTime.UtcNow,
                DateAdd = DateTime.UtcNow
            };

            await _context.FileFavorites.AddAsync(newFavorite, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return true; // Favorited
        }
    }
}