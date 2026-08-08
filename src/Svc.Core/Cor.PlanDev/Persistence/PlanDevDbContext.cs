using Cor.PlanDev.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cor.PlanDev.Persistence;

public class PlanDevDbContext : DbContext
{
    public PlanDevDbContext(DbContextOptions<PlanDevDbContext> options)
        : base(options)
    {
    }

    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectTask> Tasks { get; set; }
    public DbSet<Milestone> Milestones { get; set; }
    public DbSet<Budget> Budgets { get; set; }
    public DbSet<Resource> Resources { get; set; }
    public DbSet<Risk> Risks { get; set; }
public DbSet<Timeline> Timelines { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Project indexes
        modelBuilder.Entity<Project>()
            .HasIndex(p => p.Code)
            .IsUnique();

        modelBuilder.Entity<Project>()
            .HasIndex(p => p.Status);

        modelBuilder.Entity<Project>()
            .HasIndex(p => p.ManagerId);

        // Task indexes
        modelBuilder.Entity<ProjectTask>()
            .HasIndex(t => t.ProjectId);

        modelBuilder.Entity<ProjectTask>()
            .HasIndex(t => t.AssignedToUserId);

        modelBuilder.Entity<ProjectTask>()
            .HasIndex(t => t.Status);

        modelBuilder.Entity<ProjectTask>()
            .HasIndex(t => t.ParentTaskId);

        // Milestone indexes
        modelBuilder.Entity<Milestone>()
            .HasIndex(m => m.ProjectId);

        modelBuilder.Entity<Milestone>()
            .HasIndex(m => m.Status);

        // Budget indexes
        modelBuilder.Entity<Budget>()
            .HasIndex(b => b.ProjectId);

        modelBuilder.Entity<Budget>()
            .HasIndex(b => b.Category);

        // Resource indexes
        modelBuilder.Entity<Resource>()
            .HasIndex(r => r.ProjectId);

        modelBuilder.Entity<Resource>()
            .HasIndex(r => r.ResourceUserId);

        // Risk indexes
        modelBuilder.Entity<Risk>()
            .HasIndex(r => r.ProjectId);

        modelBuilder.Entity<Risk>()
            .HasIndex(r => r.Status);

        // Soft delete filter
        modelBuilder.Entity<Project>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<ProjectTask>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Milestone>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Budget>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Resource>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Risk>().HasQueryFilter(e => !e.IsDeleted);
    }
}