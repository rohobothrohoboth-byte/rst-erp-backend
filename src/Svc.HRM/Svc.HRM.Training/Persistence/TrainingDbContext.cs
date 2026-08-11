using Microsoft.EntityFrameworkCore;
using Svc.HRM.Training.Models.Entities;

namespace Svc.HRM.Training.Persistence;

public class TrainingDbContext : DbContext
{
    public TrainingDbContext(DbContextOptions<TrainingDbContext> options) : base(options)
    {
    }

    public DbSet<TrainingProgram> TrainingPrograms { get; set; }
    public DbSet<TrainingCourse> TrainingCourses { get; set; }
    public DbSet<TrainingEnrollment> TrainingEnrollments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TrainingProgram>(entity =>
        {
            entity.ToTable("TrainingPrograms");
            entity.HasKey(e => e.Id);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        modelBuilder.Entity<TrainingCourse>(entity =>
        {
            entity.ToTable("TrainingCourses");
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Program)
                .WithMany(p => p.Courses)
                .HasForeignKey(e => e.ProgramId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        modelBuilder.Entity<TrainingEnrollment>(entity =>
        {
            entity.ToTable("TrainingEnrollments");
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Course)
                .WithMany()
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });
    }
}
