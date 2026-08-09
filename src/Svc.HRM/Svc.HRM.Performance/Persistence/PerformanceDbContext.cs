using Microsoft.EntityFrameworkCore;
using Svc.HRM.Performance.Models.Entities;

namespace Svc.HRM.Performance.Persistence;

public class PerformanceDbContext : DbContext
{
    public PerformanceDbContext(DbContextOptions<PerformanceDbContext> options) : base(options) { }

    public DbSet<LocalGoal> Goals => Set<LocalGoal>();
    public DbSet<LocalKPI> KPIs => Set<LocalKPI>();
    public DbSet<LocalPerformanceReview> Reviews => Set<LocalPerformanceReview>();
    public DbSet<LocalFeedback> Feedbacks => Set<LocalFeedback>();
    public DbSet<LocalReviewTemplate> ReviewTemplates => Set<LocalReviewTemplate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LocalGoal>(e =>
        {
            e.ToTable("Goals");
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.EmployeeId);
            e.HasIndex(x => x.Status);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<LocalKPI>(e =>
        {
            e.ToTable("KPIs");
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.EmployeeId);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<LocalPerformanceReview>(e =>
        {
            e.ToTable("PerformanceReviews");
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.EmployeeId);
            e.HasIndex(x => x.Status);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<LocalFeedback>(e =>
        {
            e.ToTable("Feedbacks");
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.EmployeeId);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<LocalReviewTemplate>(e =>
        {
            e.ToTable("ReviewTemplates");
            e.HasKey(x => x.Id);
            e.HasQueryFilter(x => !x.IsDeleted);
        });
    }
}
