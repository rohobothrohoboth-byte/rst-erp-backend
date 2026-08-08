using Microsoft.EntityFrameworkCore;
using TaskEntity = Svc.Task.Models.Entities.Task;

namespace Svc.Task.Data;

public class TaskDbContext : DbContext
{
    public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options)
    {
    }

    public DbSet<TaskEntity> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaskEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("pending");

            entity.Property(e => e.Priority)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("medium");

            entity.Property(e => e.DueDate).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.DateAdd).IsRequired();
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);

            // Indexes for better performance
            entity.HasIndex(e => e.AssignedTo);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.IsDeleted);
            entity.HasIndex(e => e.DueDate);
            entity.HasIndex(e => e.Module);
        });
    }
}