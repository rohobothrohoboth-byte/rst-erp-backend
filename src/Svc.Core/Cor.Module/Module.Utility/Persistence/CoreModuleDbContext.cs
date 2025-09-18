using Module.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Module.Utility.Persistence;

public class CoreModuleDbContext : DbContext
{
    public CoreModuleDbContext(DbContextOptions<CoreModuleDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            relationship.DeleteBehavior = DeleteBehavior.Restrict;

        //modelBuilder.AddInboxStateEntity();
        //modelBuilder.AddOutboxMessageEntity();
        //modelBuilder.AddOutboxStateEntity();
        modelBuilder.HasPostgresExtension("pgcrypto");
        
        modelBuilder.Entity<Company>(entity =>
        {
            //entity.HasKey(e => e.Id);
            //entity.Property(e => e.Id).HasColumnType("uuid").ValueGeneratedNever();
            entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
        });

        modelBuilder.Entity<Branch>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });

        modelBuilder.Entity<Department>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });

        modelBuilder.Entity<Hierarchy>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });

        modelBuilder.Entity<FiscalYear>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });

    }

    public DbSet<Company> Company { get; set; }
    public DbSet<Branch> Branch { get; set; }
    public DbSet<Department> Department { get; set; }
    public DbSet<FiscalYear> FiscalYear { get; set; }
    public DbSet<Hierarchy> Hierarchy { get; set; }
}