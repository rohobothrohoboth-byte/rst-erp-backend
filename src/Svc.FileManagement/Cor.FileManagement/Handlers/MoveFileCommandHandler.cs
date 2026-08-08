// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Handlers\MoveFileCommandHandler.cs

using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.FileManagement.Models.DTOs;
using Cor.FileManagement.Persistence;
using Cor.FileManagement.Models.Entities;
using Cor.FileManagement.Commands;
using Cor.FileManagement.Queries;
namespace Cor.FileManagement.Handlers;

public class MoveFileCommandHandler : IRequestHandler<MoveFileCommand, FileDocumentDto>
{
    private readonly FileDbContext _context;

    public MoveFileCommandHandler(FileDbContext context)
    {
        _context = context;
    }

    public async Task<FileDocumentDto> Handle(MoveFileCommand request, CancellationToken cancellationToken)
    {
        var document = await _context.FileDocuments
            .FirstOrDefaultAsync(f => f.Id == request.Id && !f.IsDeleted, cancellationToken);

        if (document == null)
            throw new Exception("File not found");

        // Check if target folder exists
        if (request.TargetFolderId.HasValue)
        {
            var folder = await _context.FileFolders
                .FirstOrDefaultAsync(f => f.Id == request.TargetFolderId.Value && !f.IsDeleted, cancellationToken);

            if (folder == null)
                throw new Exception("Target folder not found");
        }

        document.FolderId = request.TargetFolderId;
        document.DateMod = DateTime.UtcNow;
        document.RowVersion++;

        await _context.SaveChangesAsync(cancellationToken);

        return new FileDocumentDto
        {
            Id = document.Id,
            FileName = document.FileName,
            FolderId = document.FolderId
        };
    }
}