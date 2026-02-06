using Microsoft.EntityFrameworkCore;

namespace Recruit.Utility.Persistence;

public class HrmRecruitDbContext : DbContext
{
    public HrmRecruitDbContext(DbContextOptions<HrmRecruitDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            relationship.DeleteBehavior = DeleteBehavior.Restrict;

        modelBuilder.HasPostgresExtension("pgcrypto");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HrmRecruitDbContext).Assembly);
    }

    //public DbSet<AccrualHistory> AccrualHistory { get; set; }
}