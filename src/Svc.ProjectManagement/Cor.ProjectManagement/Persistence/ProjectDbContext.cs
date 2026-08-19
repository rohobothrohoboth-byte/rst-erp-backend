// Persistence/ProjectDbContext.cs
using Microsoft.EntityFrameworkCore;
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Persistence
{
    public class ProjectDbContext : DbContext
    {
        // ✅ FIX: Use DbContextOptions<ProjectDbContext> (generic)
        public ProjectDbContext(DbContextOptions<ProjectDbContext> options)
            : base(options)
        {
        }

        // ✅ For design-time migrations (parameterless)
        public ProjectDbContext() { }

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Project configuration
            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasIndex(e => e.Code).IsUnique();
                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.ProjectManagerId);
                entity.HasIndex(e => e.StartDate);
                entity.HasIndex(e => e.EndDate);
                entity.HasIndex(e => e.CustomerId);
                entity.HasIndex(e => e.IsDeleted);

                entity.Property(e => e.CustomFields).HasColumnType("jsonb");
                entity.Property(e => e.Metadata).HasColumnType("jsonb");
            });

            // ProjectTask configuration
            modelBuilder.Entity<ProjectTask>(entity =>
            {
                entity.HasIndex(e => e.ProjectId);
                entity.HasIndex(e => e.AssigneeId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.Priority);
                entity.HasIndex(e => e.DueDate);
                entity.HasIndex(e => e.IsDeleted);

                entity.HasOne(e => e.ParentTask)
                    .WithMany(e => e.SubTasks)
                    .HasForeignKey(e => e.ParentTaskId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.CustomFields).HasColumnType("jsonb");
            });

            // Timesheet configuration
            modelBuilder.Entity<Timesheet>(entity =>
            {
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.ProjectId);
                entity.HasIndex(e => e.TaskId);
                entity.HasIndex(e => e.Date);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.IsDeleted);
                entity.HasIndex(e => new { e.UserId, e.Date });
            });

            // ProjectResource configuration
            modelBuilder.Entity<ProjectResource>(entity =>
            {
                entity.HasIndex(e => e.ProjectId);
                entity.HasIndex(e => e.ResourceId);
                entity.HasIndex(e => e.Type);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.IsDeleted);
            });

            // ProjectMilestone configuration
            modelBuilder.Entity<ProjectMilestone>(entity =>
            {
                entity.HasIndex(e => e.ProjectId);
                entity.HasIndex(e => e.DueDate);
                entity.HasIndex(e => e.IsCompleted);
                entity.HasIndex(e => e.IsDeleted);
            });

            // ProjectBudget configuration
            modelBuilder.Entity<ProjectBudget>(entity =>
            {
                entity.HasIndex(e => e.ProjectId);
                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => e.IsApproved);
                entity.HasIndex(e => e.IsDeleted);

                entity.Property(e => e.CustomFields).HasColumnType("jsonb");
            });

            // ProjectRisk configuration
            modelBuilder.Entity<ProjectRisk>(entity =>
            {
                entity.HasIndex(e => e.ProjectId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.Severity);
                entity.HasIndex(e => e.AssignedToId);
                entity.HasIndex(e => e.IsDeleted);

                entity.Property(e => e.CustomFields).HasColumnType("jsonb");
            });

            // ProjectIssue configuration
            modelBuilder.Entity<ProjectIssue>(entity =>
            {
                entity.HasIndex(e => e.ProjectId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.Priority);
                entity.HasIndex(e => e.AssignedToId);
                entity.HasIndex(e => e.IsDeleted);

                entity.Property(e => e.CustomFields).HasColumnType("jsonb");
            });

            // ProjectChange configuration
            modelBuilder.Entity<ProjectChange>(entity =>
            {
                entity.HasIndex(e => e.ProjectId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.IsDeleted);

                entity.Property(e => e.CustomFields).HasColumnType("jsonb");
            });

            // ProjectDocument configuration
            modelBuilder.Entity<ProjectDocument>(entity =>
            {
                entity.HasIndex(e => e.ProjectId);
                entity.HasIndex(e => e.Type);
                entity.HasIndex(e => e.IsApproved);
                entity.HasIndex(e => e.IsArchived);
                entity.HasIndex(e => e.IsDeleted);

                entity.Property(e => e.Metadata).HasColumnType("jsonb");
            });

            // ProjectComment configuration
            modelBuilder.Entity<ProjectComment>(entity =>
            {
                entity.HasIndex(e => e.ProjectId);
                entity.HasIndex(e => e.TaskId);
                entity.HasIndex(e => e.AuthorId);
                entity.HasIndex(e => e.IsDeleted);
            });

            // ProjectNotification configuration
            modelBuilder.Entity<ProjectNotification>(entity =>
            {
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.IsRead);
                entity.HasIndex(e => e.Type);
                entity.HasIndex(e => e.IsDeleted);

                entity.Property(e => e.Metadata).HasColumnType("jsonb");
            });

            // ProjectAuditLog configuration
            modelBuilder.Entity<ProjectAuditLog>(entity =>
            {
                entity.HasIndex(e => e.ProjectId);
                entity.HasIndex(e => e.Action);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.CreatedAt);

                entity.Property(e => e.OldValues).HasColumnType("jsonb");
                entity.Property(e => e.NewValues).HasColumnType("jsonb");
                entity.Property(e => e.Metadata).HasColumnType("jsonb");
            });

            // Configure base entity properties
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    entityType.FindProperty("CreatedAt")?.SetDefaultValueSql("CURRENT_TIMESTAMP");
                    entityType.FindProperty("IsDeleted")?.SetDefaultValue(false);
                    entityType.FindProperty("Version")?.SetDefaultValue(1);
                }
            }
        }
    }
}