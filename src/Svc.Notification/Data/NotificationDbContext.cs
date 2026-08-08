using Microsoft.EntityFrameworkCore;
using Svc.Notification.Models.Entities;

namespace Svc.Notification.Data;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
    {
    }

    public DbSet<NotificationEntity> Notifications { get; set; }  // Changed from Notification to NotificationEntity

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<NotificationEntity>(entity =>  // Changed to NotificationEntity
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.ToTable("Notifications");

            entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Message).IsRequired().HasMaxLength(4000);
            entity.Property(e => e.Type).HasMaxLength(50).HasDefaultValue("info");
            entity.Property(e => e.Priority).HasMaxLength(50).HasDefaultValue("medium");

            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.DateAdd).IsRequired();
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.IsRead);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.IsDeleted);
            entity.HasIndex(e => e.Priority);
        });
    }
}