using Microsoft.EntityFrameworkCore;
using Svc.HRM.Training.Models.DTOs;
using Svc.HRM.Training.Models.Entities;
using Svc.HRM.Training.Persistence;

namespace Svc.HRM.Training.Services;

public class TrainingService : ITrainingService
{
    private readonly TrainingDbContext _db;

    public TrainingService(TrainingDbContext db)
    {
        _db = db;
    }

    // ============================================================
    // PROGRAMS
    // ============================================================
    public async Task<List<TrainingProgram>> GetProgramsAsync(CancellationToken ct = default)
        => await _db.TrainingPrograms.AsNoTracking().ToListAsync(ct);

    public async Task<TrainingProgram?> GetProgramAsync(Guid id, CancellationToken ct = default)
        => await _db.TrainingPrograms.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<TrainingProgram> CreateProgramAsync(TrainingProgramCreateDto dto, CancellationToken ct = default)
    {
        var program = new TrainingProgram
        {
            Name = dto.Name,
            Description = dto.Description,
            Category = dto.Category,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = dto.Status
        };

        _db.TrainingPrograms.Add(program);
        await _db.SaveChangesAsync(ct);
        return program;
    }

    public async Task<TrainingProgram?> UpdateProgramAsync(Guid id, TrainingProgramCreateDto dto, CancellationToken ct = default)
    {
        var program = await _db.TrainingPrograms.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (program is null) return null;

        program.Name = dto.Name;
        program.Description = dto.Description;
        program.Category = dto.Category;
        program.StartDate = dto.StartDate;
        program.EndDate = dto.EndDate;
        program.Status = dto.Status;
        program.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return program;
    }

    public async Task<bool> DeleteProgramAsync(Guid id, CancellationToken ct = default)
    {
        var program = await _db.TrainingPrograms.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (program is null) return false;

        program.IsDeleted = true;
        program.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    // ============================================================
    // COURSES
    // ============================================================
    public async Task<List<TrainingCourse>> GetCoursesAsync(CancellationToken ct = default)
        => await _db.TrainingCourses.AsNoTracking().ToListAsync(ct);

    public async Task<TrainingCourse?> GetCourseAsync(Guid id, CancellationToken ct = default)
        => await _db.TrainingCourses.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<List<TrainingCourse>> GetCoursesByProgramAsync(Guid programId, CancellationToken ct = default)
        => await _db.TrainingCourses.AsNoTracking().Where(c => c.ProgramId == programId).ToListAsync(ct);

    public async Task<TrainingCourse> CreateCourseAsync(TrainingCourseCreateDto dto, CancellationToken ct = default)
    {
        var course = new TrainingCourse
        {
            ProgramId = dto.ProgramId,
            Title = dto.Title,
            Description = dto.Description,
            Instructor = dto.Instructor,
            DurationHours = dto.DurationHours,
            Location = dto.Location,
            Capacity = dto.Capacity,
            ScheduledDate = dto.ScheduledDate
        };

        _db.TrainingCourses.Add(course);
        await _db.SaveChangesAsync(ct);
        return course;
    }

    public async Task<TrainingCourse?> UpdateCourseAsync(Guid id, TrainingCourseCreateDto dto, CancellationToken ct = default)
    {
        var course = await _db.TrainingCourses.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (course is null) return null;

        course.ProgramId = dto.ProgramId;
        course.Title = dto.Title;
        course.Description = dto.Description;
        course.Instructor = dto.Instructor;
        course.DurationHours = dto.DurationHours;
        course.Location = dto.Location;
        course.Capacity = dto.Capacity;
        course.ScheduledDate = dto.ScheduledDate;
        course.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return course;
    }

    public async Task<bool> DeleteCourseAsync(Guid id, CancellationToken ct = default)
    {
        var course = await _db.TrainingCourses.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (course is null) return false;

        course.IsDeleted = true;
        course.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    // ============================================================
    // ENROLLMENTS
    // ============================================================
    public async Task<List<TrainingEnrollment>> GetEnrollmentsAsync(CancellationToken ct = default)
        => await _db.TrainingEnrollments.AsNoTracking().ToListAsync(ct);

    public async Task<List<TrainingEnrollment>> GetEnrollmentsByEmployeeAsync(Guid employeeId, CancellationToken ct = default)
        => await _db.TrainingEnrollments.AsNoTracking().Where(e => e.EmployeeId == employeeId).ToListAsync(ct);

    public async Task<TrainingEnrollment> CreateEnrollmentAsync(TrainingEnrollmentCreateDto dto, CancellationToken ct = default)
    {
        var enrollment = new TrainingEnrollment
        {
            CourseId = dto.CourseId,
            EmployeeId = dto.EmployeeId,
            Status = dto.Status,
            Score = dto.Score,
            Feedback = dto.Feedback,
            EnrolledAt = DateTime.UtcNow
        };

        _db.TrainingEnrollments.Add(enrollment);
        await _db.SaveChangesAsync(ct);
        return enrollment;
    }

    public async Task<TrainingEnrollment?> UpdateEnrollmentAsync(Guid id, TrainingEnrollmentCreateDto dto, CancellationToken ct = default)
    {
        var enrollment = await _db.TrainingEnrollments.FirstOrDefaultAsync(e => e.Id == id, ct);
        if (enrollment is null) return null;

        enrollment.CourseId = dto.CourseId;
        enrollment.EmployeeId = dto.EmployeeId;
        enrollment.Status = dto.Status;
        enrollment.Score = dto.Score;
        enrollment.Feedback = dto.Feedback;
        enrollment.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return enrollment;
    }

    public async Task<bool> DeleteEnrollmentAsync(Guid id, CancellationToken ct = default)
    {
        var enrollment = await _db.TrainingEnrollments.FirstOrDefaultAsync(e => e.Id == id, ct);
        if (enrollment is null) return false;

        enrollment.IsDeleted = true;
        enrollment.Status = "Cancelled";
        enrollment.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<TrainingEnrollment?> IssueCertificateAsync(Guid id, CancellationToken ct = default)
    {
        var enrollment = await _db.TrainingEnrollments.FirstOrDefaultAsync(e => e.Id == id, ct);
        if (enrollment is null) return null;

        enrollment.CertificateIssued = true;
        enrollment.Status = "Completed";
        enrollment.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return enrollment;
    }
}
