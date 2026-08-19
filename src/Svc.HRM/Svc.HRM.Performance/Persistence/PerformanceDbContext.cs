using Microsoft.EntityFrameworkCore;
using Svc.HRM.Performance.Models.Entities;

namespace Svc.HRM.Performance.Persistence;

public class PerformanceDbContext : DbContext
{
    public PerformanceDbContext(DbContextOptions<PerformanceDbContext> options) : base(options)
    {
    }

    public DbSet<Kpi> Kpis { get; set; }
    public DbSet<Goal> Goals { get; set; }
    public DbSet<PerformanceReview> PerformanceReviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Kpi>(entity =>
        {
            entity.ToTable("Kpis");
            entity.HasKey(e => e.Id);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        modelBuilder.Entity<Goal>(entity =>
        {
            entity.ToTable("Goals");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.EmployeeId);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        modelBuilder.Entity<PerformanceReview>(entity =>
        {
            entity.ToTable("PerformanceReviews");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.EmployeeId);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });
    }
}
