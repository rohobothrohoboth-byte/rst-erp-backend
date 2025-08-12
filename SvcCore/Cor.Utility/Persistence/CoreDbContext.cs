using Cor.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cor.Utility.Persistence;

public class CoreDbContext : DbContext
{
    public CoreDbContext(DbContextOptions<CoreDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            relationship.DeleteBehavior = DeleteBehavior.Restrict;

        modelBuilder.Entity<Company>(entity =>
        {
            entity.ToTable("Companys");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("uuid").ValueGeneratedNever();
            entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.ToTable("Branchs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("uuid").ValueGeneratedNever();
            entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Departments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("uuid").ValueGeneratedNever();
            entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
        });
        
        modelBuilder.Entity<Hierarchy>(entity =>
        {
            entity.ToTable("Hierarchys");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("uuid").ValueGeneratedNever();
            entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
        });

        modelBuilder.Entity<FiscalYear>(entity =>
        {
            entity.ToTable("FiscalYears");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("uuid").ValueGeneratedNever();
            entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
        });

    }

    public DbSet<Company> Companys { get; set; }
    public DbSet<Branch> Branchs { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<FiscalYear> FiscalYears { get; set; }
    public DbSet<Hierarchy> Hierarchys { get; set; }
}