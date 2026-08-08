// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Handlers\MoveDocumentCommandHandler.cs

using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.FileManagement.Models.DTOs;
using Cor.FileManagement.Persistence;
using Cor.FileManagement.Commands;

namespace Cor.FileManagement.Handlers;

public class MoveDocumentCommandHandler : IRequestHandler<MoveDocumentCommand, FileDocumentDto>
{
    private readonly FileDbContext _context;

    public MoveDocumentCommandHandler(FileDbContext context)
    {
        _context = context;
    }

    public async Task<FileDocumentDto> Handle(MoveDocumentCommand request, CancellationToken cancellationToken)
    {
        // Find the document
        var document = await _context.FileDocuments
            .FirstOrDefaultAsync(f => f.Id == request.Id && !f.IsDeleted, cancellationToken);

        if (document == null)
            throw new Exception("Document not found");

        // Check if target folder exists (if provided)
        if (request.TargetFolderId.HasValue)
        {
            var folder = await _context.FileFolders
                .FirstOrDefaultAsync(f => f.Id == request.TargetFolderId.Value && !f.IsDeleted, cancellationToken);

            if (folder == null)
                throw new Exception("Target folder not found");
        }

        // Update the document
        document.FolderId = request.TargetFolderId;
        document.DateMod = DateTime.UtcNow;
        document.RowVersion++;

        await _context.SaveChangesAsync(cancellationToken);

        // Return the updated document DTO
        return new FileDocumentDto
        {
            Id = document.Id,
            FileName = document.FileName,
            OriginalFileName = document.OriginalFileName,
            FileSize = document.FileSize,
            FileType = document.FileType,
            FileExtension = document.FileExtension,
            FilePath = document.FilePath,
            ThumbnailPath = document.ThumbnailPath,
            Description = document.Description,
            Module = document.Module,
            ReferenceId = document.ReferenceId,
            Category = document.Category,
            DocumentType = document.DocumentType,
            IsPublic = document.IsPublic,
            IsShared = document.IsShared,
            SharingLevel = document.SharingLevel,
            IsArchived = document.IsArchived,
            Version = document.Version,
            FolderId = document.FolderId,
            UploadedBy = document.UploadedBy.ToString(),
            UploadedAt = document.UploadedAt,
            UploadedAtFormatted = document.UploadedAt.ToString("yyyy-MM-dd HH:mm"),
            CanEdit = true,
            CanDelete = true,
            CanDownload = true,
            CanShare = true
        };
    }
}