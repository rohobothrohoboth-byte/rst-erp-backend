using Cor.HRMM.Models.Entities;
using Cor.HRMM.Models.Entities.Local; // ? ADD THIS
using Microsoft.EntityFrameworkCore;
using System.Data;
using Cor.HRMM.Persistence.Configurations;
namespace Cor.HRMM.Persistence;

public class coreHRMMDbContext : DbContext
{
    public coreHRMMDbContext(DbContextOptions<coreHRMMDbContext> options) : base(options)
    {
        ChangeTracker.AutoDetectChangesEnabled = false;
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        ChangeTracker.LazyLoadingEnabled = false;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(coreHRMMDbContext).Assembly);
              modelBuilder.ApplyConfiguration(new ApiKeyConfig());
                modelBuilder.ApplyConfiguration(new ApiKeyLogConfig());
        // Configure delete behavior
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            relationship.DeleteBehavior = DeleteBehavior.Restrict;

        modelBuilder.HasPostgresExtension("pgcrypto");

        // ? Configure Local Copy Tables (read-only, synced via events)
        ConfigureLocalCopyTables(modelBuilder);

    }

    private void ConfigureLocalCopyTables(ModelBuilder modelBuilder)
    {
        // Company (from Core Module)
        modelBuilder.Entity<LocalCompany>(entity =>
        {
            entity.ToTable("Companies");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.NameAm).IsRequired().HasMaxLength(200);
            entity.Property(e => e.TaxId).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.LogoUrl).HasMaxLength(500);
            entity.Property(e => e.SyncedAt).IsRequired();
            entity.HasIndex(e => e.IsDeleted).HasDatabaseName("IX_Companies_IsDeleted");
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Branch (from Core Module)
        modelBuilder.Entity<LocalBranch>(entity =>
        {
            entity.ToTable("Branches");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.NameAm).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.BranchType).HasMaxLength(50);
            entity.Property(e => e.BranchStat).HasMaxLength(20);
            entity.Property(e => e.CompId).IsRequired();
            entity.Property(e => e.SyncedAt).IsRequired();
            entity.HasIndex(e => e.CompId).HasDatabaseName("IX_Branches_CompId");
            entity.HasIndex(e => e.Code).IsUnique().HasDatabaseName("IX_Branches_Code");
            entity.HasIndex(e => e.IsDeleted).HasDatabaseName("IX_Branches_IsDeleted");
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Department (from Core Module)
        modelBuilder.Entity<LocalDepartment>(entity =>
        {
            entity.ToTable("Departments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.NameAm).IsRequired().HasMaxLength(200);
            entity.Property(e => e.DeptStat).HasMaxLength(20);
            entity.Property(e => e.BranchId).IsRequired();
            entity.Property(e => e.SyncedAt).IsRequired();
            entity.HasIndex(e => e.BranchId).HasDatabaseName("IX_Departments_BranchId");
            entity.HasIndex(e => e.IsDeleted).HasDatabaseName("IX_Departments_IsDeleted");
            entity.HasQueryFilter(e => !e.IsDeleted);
        });
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

    // ? Existing Entities
    public DbSet<BenefitSetting> BenefitSetting { get; set; }
    public DbSet<EducationQual> EducationQual { get; set; }
    public DbSet<JgStep> JgStep { get; set; }
    public DbSet<JobGrade> JobGrade { get; set; }
    public DbSet<Position> Position { get; set; }
    public DbSet<PositionBenefit> PositionBenefit { get; set; }
    public DbSet<PositionEducation> PositionEducation { get; set; }
    public DbSet<PositionExp> PositionExp { get; set; }
    public DbSet<PositionReq> PositionReq { get; set; }

    // ? NEW Local Copy Entities
    public DbSet<LocalCompany> LocalCompanies { get; set; }
    public DbSet<LocalBranch> LocalBranches { get; set; }
    public DbSet<LocalDepartment> LocalDepartments { get; set; }

public DbSet<ExternalSystem> ExternalSystems { get; set; }
     public DbSet<ApiKey> ApiKeys { get; set; }
     public DbSet<ApiKeyLog> ApiKeyLogs { get; set; }
}

