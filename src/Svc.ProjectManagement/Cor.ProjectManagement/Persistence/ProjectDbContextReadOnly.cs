// Persistence/ProjectDbContextReadOnly.cs
using Microsoft.EntityFrameworkCore;
using Cor.ProjectManagement.Models.Entities;
namespace Cor.ProjectManagement.Persistence
{
    // ✅ Rename class to ProjectDbContextReadOnly (not ProjectDbContext)
    public class ProjectDbContextReadOnly : DbContext
    {
        public ProjectDbContextReadOnly(DbContextOptions<ProjectDbContextReadOnly> options)
            : base(options)
        {
        }

        // Copy all DbSets from ProjectDbContext
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectPhase> ProjectPhases { get; set; }
        public DbSet<ProjectTask> ProjectTasks { get; set; }
        public DbSet<ProjectMilestone> ProjectMilestones { get; set; }
        public DbSet<ProjectResource> ProjectResources { get; set; }
        public DbSet<Timesheet> Timesheets { get; set; }
        public DbSet<ProjectBudget> ProjectBudgets { get; set; }
        public DbSet<ProjectRisk> ProjectRisks { get; set; }
        public DbSet<ProjectIssue> ProjectIssues { get; set; }
        public DbSet<ProjectChange> ProjectChanges { get; set; }
        public DbSet<ProjectDocument> ProjectDocuments { get; set; }
        public DbSet<ProjectComment> ProjectComments { get; set; }
        public DbSet<ProjectNotification> ProjectNotifications { get; set; }
        public DbSet<ProjectAuditLog> ProjectAuditLogs { get; set; }

        public override int SaveChanges()
        {
            throw new InvalidOperationException("This context is read-only");
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("This context is read-only");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Copy configuration from ProjectDbContext if needed
        }
    }
}