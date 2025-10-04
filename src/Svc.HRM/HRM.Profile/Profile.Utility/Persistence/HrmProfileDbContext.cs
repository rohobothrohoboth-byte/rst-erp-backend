using Microsoft.EntityFrameworkCore;
using Profile.Domain.Entities;

namespace Profile.Utility.Persistence;

public class HrmProfileDbContext : DbContext
{
    public HrmProfileDbContext(DbContextOptions<HrmProfileDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            relationship.DeleteBehavior = DeleteBehavior.Restrict;

        modelBuilder.HasPostgresExtension("pgcrypto");

        modelBuilder.Entity<EmergencyContact>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<EmpBio>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<EmpFamily>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<EmpFinance>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<EmpGuarantor>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<EmpGuarantorFile>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<Employee>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<EmpPensionCard>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<EmpPhoto>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<EmpPhotoBlob>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<EmpPhotoThumbnail>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<EmpSign>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<EmpSignBlob>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<EmpStamp>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<EmpStampBlob>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<EmpState>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<FileMetaData>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<Person>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
    }

    public DbSet<EmergencyContact> EmergencyContact { get; set; }
    public DbSet<EmpBio> EmpBio { get; set; }
    public DbSet<EmpFamily> EmpFamily { get; set; }
    public DbSet<EmpFinance> EmpFinance { get; set; }
    public DbSet<EmpGuarantor> EmpGuarantor { get; set; }
    public DbSet<EmpGuarantorFile> EmpGuarantorFile { get; set; }
    public DbSet<Employee> Employee { get; set; }
    public DbSet<EmpPensionCard> EmpPensionCard { get; set; }
    public DbSet<EmpPhoto> EmpPhoto { get; set; }
    public DbSet<EmpPhotoBlob> EmpPhotoBlob { get; set; }
    public DbSet<EmpPhotoThumbnail> EmpPhotoThumbnail { get; set; }
    public DbSet<EmpSign> EmpSign { get; set; }
    public DbSet<EmpSignBlob> EmpSignBlob { get; set; }
    public DbSet<EmpStamp> EmpStamp { get; set; }
    public DbSet<EmpStampBlob> EmpStampBlob { get; set; }
    public DbSet<EmpState> EmpState { get; set; }
    public DbSet<FileMetaData> FileMetaData { get; set; }
    public DbSet<Person> Person { get; set; }

}