using Cor.Module.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Cor.Module.Persistence;

public class CoreModuleDbContext : DbContext
{
    public CoreModuleDbContext(DbContextOptions<CoreModuleDbContext> options) : base(options)
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

        modelBuilder.HasSequence<long>("bra_code_seq").StartsAt(1).IncrementsBy(1).HasMax(9999999).IsCyclic(false).HasAnnotation("Npgsql:Sequence:Cache", 100);
        modelBuilder.HasPostgresExtension("pgcrypto");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CoreModuleDbContext).Assembly);

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

    public DbSet<Company> Company { get; set; }
    public DbSet<Branch> Branch { get; set; }
    public DbSet<Department> Department { get; set; }
    public DbSet<FiscalYear> FiscalYear { get; set; }
    public DbSet<Period> Period { get; set; }
    public DbSet<Holiday> Holiday { get; set; }
}