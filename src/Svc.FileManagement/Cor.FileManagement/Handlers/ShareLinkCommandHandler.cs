// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Handlers\GenerateShareLinkCommandHandler.cs

using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.FileManagement.Persistence;
using Cor.FileManagement.Commands;
using Cor.FileManagement.Models.Entities;

namespace Cor.FileManagement.Handlers;

public class GenerateShareLinkCommandHandler : IRequestHandler<GenerateShareLinkCommand, GenerateShareLinkResult>
{
    private readonly FileDbContext _context;

    public GenerateShareLinkCommandHandler(FileDbContext context)
    {
        _context = context;
    }

    public async Task<GenerateShareLinkResult> Handle(GenerateShareLinkCommand request, CancellationToken cancellationToken)
    {
        // Check if document exists and is not deleted
        var document = await _context.FileDocuments
            .FirstOrDefaultAsync(d => d.Id == request.DocumentId && !d.IsDeleted, cancellationToken);

        if (document == null)
            throw new Exception("Document not found");

        // Generate a unique token
        var token = Guid.NewGuid().ToString("N");

        // Set expiration (e.g., 7 days from now)
        var expiresAt = DateTime.UtcNow.AddDays(7);

        // Create share token record
        var shareToken = new FileShareToken
        {
            Id = Guid.NewGuid(),
            DocumentId = request.DocumentId,
            Token = token,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt,
            CreatedBy = request.UserId,
            CreatedByName = request.CreatedByName ?? request.UserId.ToString(), // ✅ Store the name
            IsActive = true,
            DownloadCount = 0
        };

        _context.FileShareTokens.Add(shareToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new GenerateShareLinkResult
        {
            Token = token,
            ExpiresAt = expiresAt,
            CreatedByName = shareToken.CreatedByName // ✅ Return the name
        };
    }
}