using Microsoft.EntityFrameworkCore;
using Profile.Domain.Entities;
using Profile.Domain.Entities.Local; // ? ADD THIS
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

        // ? Configure Local Copy Tables
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

        // Position (from Cor.HRMM)
        modelBuilder.Entity<LocalPosition>(entity =>
        {
            entity.ToTable("Positions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.NameAm).IsRequired().HasMaxLength(200);
            entity.Property(e => e.NoOfPosition).IsRequired();
            entity.Property(e => e.IsVacant).HasMaxLength(10);
            entity.Property(e => e.DepartmentId).IsRequired();
            entity.Property(e => e.JobGradeId);
            entity.Property(e => e.SyncedAt).IsRequired();
            entity.HasIndex(e => e.DepartmentId).HasDatabaseName("IX_Positions_DepartmentId");
            entity.HasIndex(e => e.JobGradeId).HasDatabaseName("IX_Positions_JobGradeId");
            entity.HasIndex(e => e.IsDeleted).HasDatabaseName("IX_Positions_IsDeleted");
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // JobGrade (from Cor.HRMM)
        modelBuilder.Entity<LocalJobGrade>(entity =>
        {
            entity.ToTable("JobGrades");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.StartSalary).HasColumnType("numeric(18,2)");
            entity.Property(e => e.MaxSalary).HasColumnType("numeric(18,2)");
            entity.Property(e => e.SyncedAt).IsRequired();
            entity.HasIndex(e => e.IsDeleted).HasDatabaseName("IX_JobGrades_IsDeleted");
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // JgStep (from Cor.HRMM)
        modelBuilder.Entity<LocalJgStep>(entity =>
        {
            entity.ToTable("JgStep");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Salary).HasColumnType("numeric(18,2)");
            entity.Property(e => e.Currency).IsRequired().HasMaxLength(10);
            entity.Property(e => e.SalaryPayFreq).IsRequired().HasMaxLength(20);
            entity.Property(e => e.JobGradeId).IsRequired();
            entity.Property(e => e.SyncedAt).IsRequired();
            entity.HasIndex(e => e.JobGradeId).HasDatabaseName("IX_JgSteps_JobGradeId");
            entity.HasIndex(e => e.IsDeleted).HasDatabaseName("IX_JgSteps_IsDeleted");
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
    public DbSet<Address> Address { get; set; }
    public DbSet<EmergencyContact> EmergencyContact { get; set; }
    public DbSet<EmpBio> EmpBio { get; set; }
    public DbSet<EmpCert> EmpCert { get; set; }
    public DbSet<EmpCertBirth> EmpCertBirth { get; set; }
    public DbSet<EmpCertMarriage> EmpCertMarriage { get; set; }
    public DbSet<EmpFamily> EmpFamily { get; set; }
    public DbSet<EmpFinance> EmpFinance { get; set; }
    public DbSet<EmpGuarantor> EmpGuarantor { get; set; }
    public DbSet<EmpEducation> EmpEducation { get; set; }
    public DbSet<EmpExperience> EmpExperience { get; set; }
    public DbSet<EmpGuarantorFile> EmpGuarantorFile { get; set; }
    public DbSet<EmpGuarantorFileBlob> EmpGuarantorFileBlob { get; set; }
    public DbSet<Employee> Employee { get; set; }
    public DbSet<EmpPensionCard> EmpPensionCard { get; set; }
    public DbSet<EmpPhoto> EmpPhoto { get; set; }
    public DbSet<EmpPhotoBlob> EmpPhotoBlob { get; set; }
    public DbSet<EmpPhotoThumbnail> EmpPhotoThumbnail { get; set; }
    public DbSet<EmpSalary> EmpSalary { get; set; }
    public DbSet<EmpContract> EmpContract { get; set; }
    public DbSet<EmpPromotion> EmpPromotion { get; set; }
    public DbSet<EmpTransfer> EmpTransfer { get; set; }
    public DbSet<EmpSign> EmpSign { get; set; }
    public DbSet<EmpSignBlob> EmpSignBlob { get; set; }
    public DbSet<EmpStamp> EmpStamp { get; set; }
    public DbSet<EmpStampBlob> EmpStampBlob { get; set; }
    public DbSet<FileMetaData> FileMetaData { get; set; }
    public DbSet<Person> Person { get; set; }

    // ? NEW Local Copy Entities
    public DbSet<LocalCompany> LocalCompanies { get; set; }
    public DbSet<LocalBranch> LocalBranches { get; set; }
    public DbSet<LocalDepartment> LocalDepartments { get; set; }
    public DbSet<LocalPosition> LocalPositions { get; set; }
    public DbSet<LocalJobGrade> LocalJobGrades { get; set; }
    public DbSet<LocalJgStep> LocalJgStep { get; set; }  // Singular
}