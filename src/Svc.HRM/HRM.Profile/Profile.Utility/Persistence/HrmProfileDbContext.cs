using Microsoft.EntityFrameworkCore;
using Profile.Domain.Entities;
using System.Data;

namespace Profile.Utility.Persistence;

public class HrmProfileDbContext : DbContext
{
    public HrmProfileDbContext(DbContextOptions<HrmProfileDbContext> options) : base(options)
    {
        ChangeTracker.AutoDetectChangesEnabled = false;
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        ChangeTracker.LazyLoadingEnabled = false;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            relationship.DeleteBehavior = DeleteBehavior.Restrict;

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var indexes = entityType.GetIndexes().Where(i => i.IsUnique);
            foreach (var index in indexes)
            {
                index.SetFilter("\"IsDeleted\" = false");
            }
        }

        modelBuilder.HasSequence<long>("emp_code_seq").StartsAt(1).IncrementsBy(1).HasMax(9999999).IsCyclic(false).HasAnnotation("Npgsql:Sequence:Cache", 100);
        modelBuilder.HasPostgresExtension("pgcrypto");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HrmProfileDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.DateAdd = now;
                    break;
                case EntityState.Modified:
                    entry.Entity.DateMod = now;
                    break;
            }
        }

        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new DBConcurrencyException("The record was modified by another transaction.", ex);
        }
    }


    public DbSet<Address> Address { get; set; }
    public DbSet<EmergencyContact> EmergencyContact { get; set; }
    public DbSet<EmpBio> EmpBio { get; set; }
    public DbSet<EmpFamily> EmpFamily { get; set; }
    public DbSet<EmpFinance> EmpFinance { get; set; }
    public DbSet<EmpGuarantor> EmpGuarantor { get; set; }
    public DbSet<EmpGuarantorFile> EmpGuarantorFile { get; set; }
    public DbSet<EmpGuarantorFileBlob> EmpGuarantorFileBlob { get; set; }
    public DbSet<Employee> Employee { get; set; }
    public DbSet<EmpPensionCard> EmpPensionCard { get; set; }
    public DbSet<EmpPhoto> EmpPhoto { get; set; }
    public DbSet<EmpPhotoBlob> EmpPhotoBlob { get; set; }
    public DbSet<EmpPhotoThumbnail> EmpPhotoThumbnail { get; set; }
    public DbSet<EmpSalary> EmpSalary { get; set; }
    public DbSet<EmpSign> EmpSign { get; set; }
    public DbSet<EmpSignBlob> EmpSignBlob { get; set; }
    public DbSet<EmpStamp> EmpStamp { get; set; }
    public DbSet<EmpStampBlob> EmpStampBlob { get; set; }
    public DbSet<FileMetaData> FileMetaData { get; set; }
    public DbSet<Person> Person { get; set; }

}