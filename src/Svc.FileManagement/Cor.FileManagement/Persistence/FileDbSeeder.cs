// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Persistence\FileDbSeeder.cs

using Cor.FileManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;
namespace Cor.FileManagement.Persistence;

public static class FileDbSeeder
{
    public static async Task SeedDefaultFolders(FileDbContext context)
    {
        // Check if any folders exist
        if (await context.FileFolders.AnyAsync())
            return;

        var defaultFolders = new List<FileFolder>
        {
            // Company Documents
            new FileFolder
            {
                Id = Guid.NewGuid(),
                Name = "Company Documents",
                Description = "Company-wide documents and policies",
                FolderType = "Company",
                IsPublic = true,
                IsShared = true,
                SharingLevel = "Company",
                Order = 0,
                CreatedBy = Guid.Empty,
                CreatedAt = DateTime.UtcNow,
                DateAdd = DateTime.UtcNow,
                DateMod = DateTime.UtcNow
            },
            // Company Folders
            new FileFolder
            {
                Id = Guid.NewGuid(),
                Name = "Company Folders",
                Description = "Department and project folders",
                FolderType = "Company",
                IsPublic = true,
                IsShared = true,
                SharingLevel = "Company",
                Order = 1,
                CreatedBy = Guid.Empty,
                CreatedAt = DateTime.UtcNow,
                DateAdd = DateTime.UtcNow,
                DateMod = DateTime.UtcNow
            },
            // Shared Documents
            new FileFolder
            {
                Id = Guid.NewGuid(),
                Name = "Shared Documents",
                Description = "Documents shared across teams",
                FolderType = "Shared",
                IsPublic = true,
                IsShared = true,
                SharingLevel = "Company",
                Order = 2,
                CreatedBy = Guid.Empty,
                CreatedAt = DateTime.UtcNow,
                DateAdd = DateTime.UtcNow,
                DateMod = DateTime.UtcNow
            },
            // Personal Documents
            new FileFolder
            {
                Id = Guid.NewGuid(),
                Name = "Personal Documents",
                Description = "Personal documents and files",
                FolderType = "Personal",
                IsPublic = false,
                IsShared = false,
                SharingLevel = "Private",
                Order = 3,
                CreatedBy = Guid.Empty,
                CreatedAt = DateTime.UtcNow,
                DateAdd = DateTime.UtcNow,
                DateMod = DateTime.UtcNow
            },
            // My Folders
            new FileFolder
            {
                Id = Guid.NewGuid(),
                Name = "My Folders",
                Description = "Personal folders",
                FolderType = "Personal",
                IsPublic = false,
                IsShared = false,
                SharingLevel = "Private",
                Order = 4,
                CreatedBy = Guid.Empty,
                CreatedAt = DateTime.UtcNow,
                DateAdd = DateTime.UtcNow,
                DateMod = DateTime.UtcNow
            },
            // Recent Files
            new FileFolder
            {
                Id = Guid.NewGuid(),
                Name = "Recent Files",
                Description = "Recently accessed files",
                FolderType = "Personal",
                IsPublic = false,
                IsShared = false,
                SharingLevel = "Private",
                Order = 5,
                CreatedBy = Guid.Empty,
                CreatedAt = DateTime.UtcNow,
                DateAdd = DateTime.UtcNow,
                DateMod = DateTime.UtcNow
            },
            // Archive
            new FileFolder
            {
                Id = Guid.NewGuid(),
                Name = "Archive",
                Description = "Archived documents",
                FolderType = "Archive",
                IsPublic = false,
                IsShared = false,
                SharingLevel = "Private",
                Order = 6,
                CreatedBy = Guid.Empty,
                CreatedAt = DateTime.UtcNow,
                DateAdd = DateTime.UtcNow,
                DateMod = DateTime.UtcNow
            },
            // Finance Department Folder
            new FileFolder
            {
                Id = Guid.NewGuid(),
                Name = "Finance",
                Description = "Finance department documents",
                FolderType = "Department",
                IsPublic = true,
                IsShared = true,
                SharingLevel = "Department",
                Order = 7,
                CreatedBy = Guid.Empty,
                CreatedAt = DateTime.UtcNow,
                DateAdd = DateTime.UtcNow,
                DateMod = DateTime.UtcNow
            },
            // HR Department Folder
            new FileFolder
            {
                Id = Guid.NewGuid(),
                Name = "HR",
                Description = "HR department documents",
                FolderType = "Department",
                IsPublic = true,
                IsShared = true,
                SharingLevel = "Department",
                Order = 8,
                CreatedBy = Guid.Empty,
                CreatedAt = DateTime.UtcNow,
                DateAdd = DateTime.UtcNow,
                DateMod = DateTime.UtcNow
            },
            // Invoices Folder
            new FileFolder
            {
                Id = Guid.NewGuid(),
                Name = "Invoices",
                Description = "Invoice documents",
                FolderType = "Finance",
                IsPublic = true,
                IsShared = true,
                SharingLevel = "Department",
                Order = 9,
                CreatedBy = Guid.Empty,
                CreatedAt = DateTime.UtcNow,
                DateAdd = DateTime.UtcNow,
                DateMod = DateTime.UtcNow
            }
        };

        await context.FileFolders.AddRangeAsync(defaultFolders);
        await context.SaveChangesAsync();
    }
}