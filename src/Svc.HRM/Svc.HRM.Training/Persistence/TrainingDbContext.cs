using Microsoft.EntityFrameworkCore;
using Svc.HRM.Training.Models.Entities;

namespace Svc.HRM.Training.Persistence;

public class TrainingDbContext : DbContext
{
    public TrainingDbContext(DbContextOptions<TrainingDbContext> options) : base(options) { }

    public DbSet<LocalTrainingProgram> Programs => Set<LocalTrainingProgram>();
    public DbSet<LocalTrainingCourse> Courses => Set<LocalTrainingCourse>();
    public DbSet<LocalTrainingSession> Sessions => Set<LocalTrainingSession>();
    public DbSet<LocalTrainingEnrollment> Enrollments => Set<LocalTrainingEnrollment>();
    public DbSet<LocalTrainingEvaluation> Evaluations => Set<LocalTrainingEvaluation>();
    public DbSet<LocalTrainingCertificate> Certificates => Set<LocalTrainingCertificate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LocalTrainingProgram>(e =>
        {
            e.ToTable("TrainingPrograms");
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Code).IsUnique();
            e.HasIndex(x => x.Status);
            e.HasQueryFilter(x => !x.IsDeleted);
            e.HasMany(x => x.Courses).WithOne(x => x.Program).HasForeignKey(x => x.ProgramId);
        });

        modelBuilder.Entity<LocalTrainingCourse>(e =>
        {
            e.ToTable("TrainingCourses");
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.ProgramId);
            e.HasQueryFilter(x => !x.IsDeleted);
            e.HasMany(x => x.Sessions).WithOne(x => x.Course).HasForeignKey(x => x.CourseId);
        });

        modelBuilder.Entity<LocalTrainingSession>(e =>
        {
            e.ToTable("TrainingSessions");
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.CourseId);
            e.HasIndex(x => x.StartAt);
            e.HasQueryFilter(x => !x.IsDeleted);
            e.HasMany(x => x.Enrollments).WithOne(x => x.Session).HasForeignKey(x => x.SessionId);
        });

        modelBuilder.Entity<LocalTrainingEnrollment>(e =>
        {
            e.ToTable("TrainingEnrollments");
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.EmployeeId);
            e.HasIndex(x => x.ProgramId);
            e.HasIndex(x => new { x.ProgramId, x.EmployeeId, x.SessionId });
            e.HasQueryFilter(x => !x.IsDeleted);
            e.HasOne(x => x.Program).WithMany().HasForeignKey(x => x.ProgramId);
        });

        modelBuilder.Entity<LocalTrainingEvaluation>(e =>
        {
            e.ToTable("TrainingEvaluations");
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.EnrollmentId);
            e.HasIndex(x => x.EmployeeId);
            e.HasQueryFilter(x => !x.IsDeleted);
            e.HasOne(x => x.Enrollment).WithMany().HasForeignKey(x => x.EnrollmentId);
        });

        modelBuilder.Entity<LocalTrainingCertificate>(e =>
        {
            e.ToTable("TrainingCertificates");
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.CertificateNumber).IsUnique();
            e.HasIndex(x => x.EmployeeId);
            e.HasIndex(x => x.EnrollmentId);
            e.HasQueryFilter(x => !x.IsDeleted);
            e.HasOne(x => x.Enrollment).WithMany().HasForeignKey(x => x.EnrollmentId);
        });
    }
}
