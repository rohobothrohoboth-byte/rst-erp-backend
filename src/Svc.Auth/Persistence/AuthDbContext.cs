using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Svc.Auth.Models.Entities;
using Svc.Auth.Models.Entities.Local;
using System.Data;

namespace Svc.Auth.Persistence;

public class AuthDbContext : IdentityDbContext<AppUser, AppRole, string>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
        // ✅ Allow tracking for update operations
        // These are now configurable per query using AsTracking()/AsNoTracking()
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);

        // Configure Identity tables
        modelBuilder.Entity<AppUser>().ToTable("AppUser");
        modelBuilder.Entity<AppRole>().ToTable("AppRole");
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaim");
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRole");
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogin");
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaim");
        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserToken");

        // Configure cross-module relationships (shadow properties)
        ConfigureCrossModuleRelationships(modelBuilder);

        // Configure local copy tables (read-only, synced via events)
        ConfigureLocalCopyTables(modelBuilder);

        // Configure delete behavior
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            relationship.DeleteBehavior = DeleteBehavior.Restrict;

        modelBuilder.HasPostgresExtension("pg_trgm");
    }

    private void ConfigureCrossModuleRelationships(ModelBuilder modelBuilder)
    {
        // AppUser has BranchId, DepartmentId, PositionId as shadow properties
        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.Property<Guid?>("BranchId")
                .HasColumnName("BranchId");

            entity.Property<Guid?>("DepartmentId")
                .HasColumnName("DepartmentId");

            entity.Property<Guid?>("PositionId")
                .HasColumnName("PositionId");

            entity.HasIndex("BranchId").HasDatabaseName("IX_AppUser_BranchId");
            entity.HasIndex("DepartmentId").HasDatabaseName("IX_AppUser_DepartmentId");
            entity.HasIndex("PositionId").HasDatabaseName("IX_AppUser_PositionId");
        });

        // AppRole has PositionId as shadow property
        modelBuilder.Entity<AppRole>(entity =>
        {
            entity.Property<Guid?>("PositionId")
                .HasColumnName("PositionId");

            entity.HasIndex("PositionId").HasDatabaseName("IX_AppRole_PositionId");
        });

        // Position Permission Tables
        modelBuilder.Entity<PositionPerModule>(entity =>
        {
            entity.ToTable("PositionPerModule");
            entity.Property(e => e.PositionId).IsRequired();
            entity.Property(e => e.PerModuleId).IsRequired();
            entity.HasIndex(e => e.PositionId).HasDatabaseName("IX_PositionPerModule_PositionId");
            entity.HasIndex(e => e.PerModuleId).HasDatabaseName("IX_PositionPerModule_PerModuleId");
            entity.HasIndex(e => new { e.PositionId, e.PerModuleId })
                .IsUnique()
                .HasDatabaseName("IX_PositionPerModule_Unique");
        });

        modelBuilder.Entity<PositionPerMenu>(entity =>
        {
            entity.ToTable("PositionPerMenu");
            entity.Property(e => e.PositionId).IsRequired();
            entity.Property(e => e.PerMenuId).IsRequired();
            entity.HasIndex(e => e.PositionId).HasDatabaseName("IX_PositionPerMenu_PositionId");
            entity.HasIndex(e => e.PerMenuId).HasDatabaseName("IX_PositionPerMenu_PerMenuId");
            entity.HasIndex(e => new { e.PositionId, e.PerMenuId })
                .IsUnique()
                .HasDatabaseName("IX_PositionPerMenu_Unique");
        });

        modelBuilder.Entity<PositionPerApi>(entity =>
        {
            entity.ToTable("PositionPerApi");
            entity.Property(e => e.PositionId).IsRequired();
            entity.Property(e => e.PerApiId).IsRequired();
            entity.HasIndex(e => e.PositionId).HasDatabaseName("IX_PositionPerApi_PositionId");
            entity.HasIndex(e => e.PerApiId).HasDatabaseName("IX_PositionPerApi_PerApiId");
            entity.HasIndex(e => new { e.PositionId, e.PerApiId })
                .IsUnique()
                .HasDatabaseName("IX_PositionPerApi_Unique");
        });
    }

    private void ConfigureLocalCopyTables(ModelBuilder modelBuilder)
    {
        // ============================================
        // LOCAL COPY TABLES (read-only, synced via events)
        // These are copies of data from Core/HRM services
        // ============================================

        // Company
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

        // Branch
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

            entity.HasOne(e => e.Company)
                .WithMany(e => e.Branches)
                .HasForeignKey(e => e.CompId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.CompId).HasDatabaseName("IX_Branches_CompId");
            entity.HasIndex(e => e.Code).IsUnique().HasDatabaseName("IX_Branches_Code");
            entity.HasIndex(e => e.IsDeleted).HasDatabaseName("IX_Branches_IsDeleted");
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Department
        modelBuilder.Entity<LocalDepartment>(entity =>
        {
            entity.ToTable("Departments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.NameAm).IsRequired().HasMaxLength(200);
            entity.Property(e => e.DeptStat).HasMaxLength(20);
            entity.Property(e => e.BranchId).IsRequired();
            entity.Property(e => e.SyncedAt).IsRequired();

            entity.HasOne(e => e.Branch)
                .WithMany(e => e.Departments)
                .HasForeignKey(e => e.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.BranchId).HasDatabaseName("IX_Departments_BranchId");
            entity.HasIndex(e => new { e.BranchId, e.Name }).IsUnique().HasDatabaseName("IX_Departments_BranchId_Name");
            entity.HasIndex(e => e.IsDeleted).HasDatabaseName("IX_Departments_IsDeleted");
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Position
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

            entity.HasOne(e => e.Department)
                .WithMany()
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.DepartmentId).HasDatabaseName("IX_Positions_DepartmentId");
            entity.HasIndex(e => e.JobGradeId).HasDatabaseName("IX_Positions_JobGradeId");
            entity.HasIndex(e => e.IsDeleted).HasDatabaseName("IX_Positions_IsDeleted");
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // JobGrade
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

        // Employee
        modelBuilder.Entity<LocalEmployee>(entity =>
        {
            entity.ToTable("Employees");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.EmploymentType).HasMaxLength(50);
            entity.Property(e => e.EmploymentNature).HasMaxLength(50);
            entity.Property(e => e.WorkArrangement).HasMaxLength(50);
            entity.Property(e => e.EmpState).HasMaxLength(50);
            entity.Property(e => e.PersonId).IsRequired();
            entity.Property(e => e.JobGradeId).IsRequired();
            entity.Property(e => e.PositionId).IsRequired();
            entity.Property(e => e.DepartmentId).IsRequired();
            entity.Property(e => e.AppUserId).HasMaxLength(50);

            // Person details (denormalized)
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.FirstNameAm).HasMaxLength(100);
            entity.Property(e => e.MiddleName).HasMaxLength(100);
            entity.Property(e => e.MiddleNameAm).HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastNameAm).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(20);
            entity.Property(e => e.Nationality).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.SyncedAt).IsRequired();

            entity.HasOne(e => e.Position)
                .WithMany()
                .HasForeignKey(e => e.PositionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Department)
                .WithMany()
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.JobGrade)
                .WithMany()
                .HasForeignKey(e => e.JobGradeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.PositionId).HasDatabaseName("IX_Employees_PositionId");
            entity.HasIndex(e => e.DepartmentId).HasDatabaseName("IX_Employees_DepartmentId");
            entity.HasIndex(e => e.JobGradeId).HasDatabaseName("IX_Employees_JobGradeId");
            entity.HasIndex(e => e.AppUserId).HasDatabaseName("IX_Employees_AppUserId");
            entity.HasIndex(e => e.Code).IsUnique().HasDatabaseName("IX_Employees_Code");
            entity.HasIndex(e => e.IsDeleted).HasDatabaseName("IX_Employees_IsDeleted");
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

    // Auth tables
    public DbSet<PerModule> PerModule { get; set; }
    public DbSet<PerMenu> PerMenu { get; set; }
    public DbSet<PerApi> PerApi { get; set; }
    public DbSet<RefreshToken> RefreshToken { get; set; }
    public DbSet<UserPerModule> UserPerModule { get; set; }
    public DbSet<UserPerMenu> UserPerMenu { get; set; }
    public DbSet<UserPerApi> UserPerApi { get; set; }
    public DbSet<PositionPerModule> PositionPerModule { get; set; }
    public DbSet<PositionPerMenu> PositionPerMenu { get; set; }
    public DbSet<PositionPerApi> PositionPerApi { get; set; }

    // Local copy tables (read-only, synced via events)
    public DbSet<LocalCompany> Companies { get; set; }
    public DbSet<LocalBranch> Branches { get; set; }
    public DbSet<LocalDepartment> Departments { get; set; }
    public DbSet<LocalPosition> Positions { get; set; }
    public DbSet<LocalJobGrade> JobGrades { get; set; }
    public DbSet<LocalEmployee> Employees { get; set; }
}