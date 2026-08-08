// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Handlers\CreateFolderCommandHandler.cs

using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.FileManagement.Models.Entities;
using Cor.FileManagement.Models.DTOs;
using Cor.FileManagement.Persistence;
using Cor.FileManagement.Queries;
using Cor.FileManagement.Commands;
namespace Cor.FileManagement.Handlers;

public class CreateFolderCommandHandler : IRequestHandler<CreateFolderCommand, FileFolderDto>
{
    private readonly FileDbContext _context;

    public CreateFolderCommandHandler(FileDbContext context)
    {
        _context = context;
    }

    public async Task<FileFolderDto> Handle(CreateFolderCommand request, CancellationToken cancellationToken)
    {
        // Check if parent folder exists
        if (request.ParentId.HasValue)
        {
            var parent = await _context.FileFolders
                .FirstOrDefaultAsync(f => f.Id == request.ParentId.Value && !f.IsDeleted, cancellationToken);

            if (parent == null)
                throw new Exception("Parent folder not found");
        }

        // Check for duplicate name in same parent
        var existing = await _context.FileFolders
            .FirstOrDefaultAsync(f => f.ParentId == request.ParentId && f.Name == request.Name && !f.IsDeleted, cancellationToken);

        if (existing != null)
            throw new Exception("A folder with this name already exists in this location");

        var folder = new FileFolder
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            FolderType = request.FolderType ?? "Personal",
            Category = request.Category ?? "General",
            ParentId = request.ParentId,
            IsPublic = request.IsPublic,
            IsShared = request.IsShared,
            SharingLevel = request.SharingLevel ?? "Private",
            Order = request.Order,
            CreatedBy = request.CreatedBy,
            CreatedAt = DateTime.UtcNow,
            DateAdd = DateTime.UtcNow,
            DateMod = DateTime.UtcNow
        };

        await _context.FileFolders.AddAsync(folder, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new FileFolderDto
        {
            Id = folder.Id,
            Name = folder.Name,
            Description = folder.Description,
            FolderType = folder.FolderType,
            ParentId = folder.ParentId,
            IsPublic = folder.IsPublic,
            IsShared = folder.IsShared,
            SharingLevel = folder.SharingLevel,
            IsArchived = folder.IsArchived,
            Order = folder.Order,
            CreatedAt = folder.CreatedAt,
            CanEdit = true,
            CanDelete = true,
            CanShare = true,
            Icon = GetFolderIcon(folder.FolderType),
            Color = GetFolderColor(folder.FolderType)
        };
    }

    private string GetFolderIcon(string folderType)
    {
        return folderType?.ToLower() switch
        {
            "company" => "🏢",
            "department" => "📁",
            "personal" => "👤",
            "shared" => "🔗",
            "archive" => "📦",
            "finance" => "💰",
            "hr" => "👥",
            _ => "📁"
        };
    }

    private string GetFolderColor(string folderType)
    {
        return folderType?.ToLower() switch
        {
            "company" => "#4F46E5",
            "department" => "#7C3AED",
            "personal" => "#059669",
            "shared" => "#D97706",
            "archive" => "#6B7280",
            "finance" => "#0D9488",
            "hr" => "#DC2626",
            _ => "#6B7280"
        };
    }
}