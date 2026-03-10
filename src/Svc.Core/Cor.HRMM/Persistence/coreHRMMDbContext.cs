using Cor.HRMM.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data;

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

        modelBuilder.HasPostgresExtension("pgcrypto");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(coreHRMMDbContext).Assembly);
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

    public DbSet<BenefitSetting> BenefitSetting { get; set; }
    public DbSet<EducationQual> EducationQual { get; set; }
    public DbSet<JgStep> JgStep { get; set; }
    public DbSet<JobGrade> JobGrade { get; set; }
    public DbSet<Position> Position { get; set; }
    public DbSet<PositionBenefit> PositionBenefit { get; set; }
    public DbSet<PositionEducation> PositionEducation { get; set; }
    public DbSet<PositionExp> PositionExp { get; set; }
    public DbSet<PositionReq> PositionReq { get; set; }
}
