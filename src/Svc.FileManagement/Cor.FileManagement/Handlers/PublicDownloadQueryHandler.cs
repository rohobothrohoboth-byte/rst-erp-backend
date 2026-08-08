using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.FileManagement.Persistence;
using Cor.FileManagement.Queries;

namespace Cor.FileManagement.Handlers;

public class PublicDownloadQueryHandler : IRequestHandler<PublicDownloadQuery, PublicDownloadResult>
{
    private readonly FileDbContext _context;

    public PublicDownloadQueryHandler(FileDbContext context)
    {
        _context = context;
    }

    public async Task<PublicDownloadResult> Handle(PublicDownloadQuery request, CancellationToken cancellationToken)
    {
        // Find the share token
        var shareRecord = await _context.FileShareTokens
            .FirstOrDefaultAsync(t => t.Token == request.Token && t.IsActive, cancellationToken);

        if (shareRecord == null)
            return new PublicDownloadResult { FileBytes = null };

        // Check expiration
        if (shareRecord.ExpiresAt < DateTime.UtcNow)
            return new PublicDownloadResult { FileBytes = null };

        // Get the document
        var document = await _context.FileDocuments
            .FirstOrDefaultAsync(f => f.Id == shareRecord.DocumentId && !f.IsDeleted, cancellationToken);

        if (document == null)
            return new PublicDownloadResult { FileBytes = null };

        // Read the file
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var filePath = Path.Combine(basePath, document.FilePath ?? "");

        if (!System.IO.File.Exists(filePath))
            return new PublicDownloadResult { FileBytes = null };

        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath, cancellationToken);

        return new PublicDownloadResult
        {
            FileBytes = fileBytes,
            ContentType = document.FileType ?? "application/octet-stream",
            FileName = document.OriginalFileName ?? document.FileName ?? "download"
        };
    }
}