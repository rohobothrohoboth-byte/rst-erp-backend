// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Persistence\FileDbContext.cs

using Microsoft.EntityFrameworkCore;
using Cor.FileManagement.Models.Entities;
using FileShareEntity = Cor.FileManagement.Models.Entities.FileShare;
namespace Cor.FileManagement.Persistence;

public class FileDbContext : DbContext
{
    public FileDbContext(DbContextOptions<FileDbContext> options) : base(options)
    {
    }

    public DbSet<FileDocument> FileDocuments { get; set; }
    public DbSet<FileFolder> FileFolders { get; set; }
    public DbSet<FileShareEntity> FileShares { get; set; }
    public DbSet<FileAccessLog> FileAccessLogs { get; set; }
    public DbSet<FileFavorite> FileFavorites { get; set; }
    public DbSet<FileShareToken> FileShareTokens { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // FileDocument configuration
        modelBuilder.Entity<FileDocument>(entity =>
        {
            entity.ToTable("FileDocuments");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.FileName).HasMaxLength(255).IsRequired();
            entity.Property(e => e.OriginalFileName).HasMaxLength(500).IsRequired();
            entity.Property(e => e.FileType).HasMaxLength(100).IsRequired();
            entity.Property(e => e.FileExtension).HasMaxLength(100).IsRequired();
            entity.Property(e => e.FilePath).HasMaxLength(1000).IsRequired();
            entity.Property(e => e.ThumbnailPath).HasMaxLength(500);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.StorageProvider).HasMaxLength(100);
            entity.Property(e => e.Module).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.DocumentType).HasMaxLength(100);
            entity.Property(e => e.SharingLevel).HasMaxLength(100);
            entity.Property(e => e.UploadedByName).HasMaxLength(255);
            entity.Property(e => e.Hash).HasMaxLength(50);

            entity.HasIndex(e => e.Module);
            entity.HasIndex(e => e.ReferenceId);
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.FolderId);
            entity.HasIndex(e => e.UploadedBy);
            entity.HasIndex(e => e.IsDeleted);
            entity.HasIndex(e => e.IsArchived);
            entity.HasIndex(e => new { e.Module, e.ReferenceId, e.Category });
        });

        // FileFolder configuration
        modelBuilder.Entity<FileFolder>(entity =>
        {
            entity.ToTable("FileFolders");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.FolderType).HasMaxLength(50);
            entity.Property(e => e.SharingLevel).HasMaxLength(100);

            entity.HasIndex(e => e.FolderType);
            entity.HasIndex(e => e.OwnerId);
            entity.HasIndex(e => e.ParentId);
            entity.HasIndex(e => e.IsDeleted);
            entity.HasIndex(e => e.IsArchived);

            // Self-referencing relationship
            entity.HasOne(e => e.Parent)
                  .WithMany(e => e.Children)
                  .HasForeignKey(e => e.ParentId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // FileShare configuration
        modelBuilder.Entity<FileShareEntity>(entity =>
        {
            entity.ToTable("FileShares");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.SharedWithType).HasMaxLength(50);
            entity.Property(e => e.Permission).HasMaxLength(20);

            entity.HasIndex(e => e.DocumentId);
            entity.HasIndex(e => e.FolderId);
            entity.HasIndex(e => e.SharedWithId);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.ExpiresAt);
        });

        // FileAccessLog configuration
        modelBuilder.Entity<FileAccessLog>(entity =>
        {
            entity.ToTable("FileAccessLogs");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Action).HasMaxLength(50);
            entity.Property(e => e.ActionType).HasMaxLength(50);
            entity.Property(e => e.IPAddress).HasMaxLength(100);
            entity.Property(e => e.UserAgent).HasMaxLength(255);

            entity.HasIndex(e => e.DocumentId);
            entity.HasIndex(e => e.FolderId);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Action);
            entity.HasIndex(e => e.AccessedAt);
        });
    }
}