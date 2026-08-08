using Microsoft.EntityFrameworkCore;
using Svc.HRM.Training.Models.DTOs;
using Svc.HRM.Training.Models.Entities;
using Svc.HRM.Training.Models.Enums;
using Svc.HRM.Training.Persistence;

namespace Svc.HRM.Training.Services;

public class TrainingService : ITrainingService
{
    private readonly TrainingDbContext _db;

    public TrainingService(TrainingDbContext db) => _db = db;

    public async Task<List<TrainingProgramDto>> GetProgramsAsync(string? status = null, CancellationToken ct = default)
    {
        var q = _db.Programs.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(status))
            q = q.Where(x => x.Status == status);

        var items = await q.OrderByDescending(x => x.DateAdd).ToListAsync(ct);
        var counts = await _db.Courses.AsNoTracking()
            .GroupBy(x => x.ProgramId)
            .Select(g => new { ProgramId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.ProgramId, x => x.Count, ct);

        return items.Select(x => MapProgram(x, counts.GetValueOrDefault(x.Id))).ToList();
    }

    public async Task<TrainingProgramDto?> GetProgramAsync(Guid id, CancellationToken ct = default)
    {
        var item = await _db.Programs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (item == null) return null;
        var count = await _db.Courses.CountAsync(x => x.ProgramId == id, ct);
        return MapProgram(item, count);
    }

    public async Task<TrainingProgramDto> CreateProgramAsync(TrainingProgramCreateDto dto, CancellationToken ct = default)
    {
        var exists = await _db.Programs.AnyAsync(x => x.Code == dto.Code, ct);
        if (exists) throw new InvalidOperationException($"Program code '{dto.Code}' already exists.");

        var e = new LocalTrainingProgram
        {
            Code = dto.Code.Trim(),
            Title = dto.Title.Trim(),
            Description = dto.Description,
            Category = dto.Category,
            DurationHours = dto.DurationHours,
            Provider = dto.Provider,
            IsMandatory = dto.IsMandatory,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = ProgramStatus.Draft
        };
        _db.Programs.Add(e);
        await _db.SaveChangesAsync(ct);
        return MapProgram(e, 0);
    }

    public async Task<TrainingProgramDto> UpdateProgramAsync(Guid id, TrainingProgramUpdateDto dto, CancellationToken ct = default)
    {
        var e = await _db.Programs.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Program not found.");
        e.Title = dto.Title.Trim();
        e.Description = dto.Description;
        e.Category = dto.Category;
        e.Status = dto.Status;
        e.DurationHours = dto.DurationHours;
        e.Provider = dto.Provider;
        e.IsMandatory = dto.IsMandatory;
        e.StartDate = dto.StartDate;
        e.EndDate = dto.EndDate;
        e.DateMod = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        var count = await _db.Courses.CountAsync(x => x.ProgramId == id, ct);
        return MapProgram(e, count);
    }

    public async Task DeleteProgramAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _db.Programs.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Program not found.");
        e.IsDeleted = true;
        e.DateMod = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<TrainingProgramDto> PublishProgramAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _db.Programs.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Program not found.");
        e.Status = ProgramStatus.Published;
        e.DateMod = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        var count = await _db.Courses.CountAsync(x => x.ProgramId == id, ct);
        return MapProgram(e, count);
    }

    public async Task<TrainingProgramDto> CancelProgramAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _db.Programs.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Program not found.");
        e.Status = ProgramStatus.Cancelled;
        e.DateMod = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        var count = await _db.Courses.CountAsync(x => x.ProgramId == id, ct);
        return MapProgram(e, count);
    }

    public async Task<List<TrainingCourseDto>> GetCoursesAsync(Guid? programId = null, CancellationToken ct = default)
    {
        var q = _db.Courses.AsNoTracking().AsQueryable();
        if (programId.HasValue) q = q.Where(x => x.ProgramId == programId);
        var items = await q.OrderBy(x => x.Sequence).ToListAsync(ct);
        return items.Select(MapCourse).ToList();
    }

    public async Task<TrainingCourseDto?> GetCourseAsync(Guid id, CancellationToken ct = default)
    {
        var item = await _db.Courses.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return item == null ? null : MapCourse(item);
    }

    public async Task<TrainingCourseDto> CreateCourseAsync(TrainingCourseCreateDto dto, CancellationToken ct = default)
    {
        _ = await _db.Programs.FirstOrDefaultAsync(x => x.Id == dto.ProgramId, ct)
            ?? throw new KeyNotFoundException("Program not found.");

        var e = new LocalTrainingCourse
        {
            ProgramId = dto.ProgramId,
            Title = dto.Title.Trim(),
            Description = dto.Description,
            Objectives = dto.Objectives,
            Sequence = dto.Sequence,
            DurationHours = dto.DurationHours
        };
        _db.Courses.Add(e);
        await _db.SaveChangesAsync(ct);
        return MapCourse(e);
    }

    public async Task<TrainingCourseDto> UpdateCourseAsync(Guid id, TrainingCourseUpdateDto dto, CancellationToken ct = default)
    {
        var e = await _db.Courses.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Course not found.");
        e.Title = dto.Title.Trim();
        e.Description = dto.Description;
        e.Objectives = dto.Objectives;
        e.Sequence = dto.Sequence;
        e.DurationHours = dto.DurationHours;
        e.DateMod = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return MapCourse(e);
    }

    public async Task DeleteCourseAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _db.Courses.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Course not found.");
        e.IsDeleted = true;
        e.DateMod = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<TrainingSessionDto>> GetSessionsAsync(Guid? courseId = null, DateTime? from = null, DateTime? to = null, CancellationToken ct = default)
    {
        var q = _db.Sessions.AsNoTracking().AsQueryable();
        if (courseId.HasValue) q = q.Where(x => x.CourseId == courseId);
        if (from.HasValue) q = q.Where(x => x.StartAt >= from.Value);
        if (to.HasValue) q = q.Where(x => x.StartAt <= to.Value);

        var sessions = await q.OrderBy(x => x.StartAt).ToListAsync(ct);
        var counts = await _db.Enrollments.AsNoTracking()
            .Where(x => x.SessionId != null)
            .GroupBy(x => x.SessionId!.Value)
            .Select(g => new { SessionId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.SessionId, x => x.Count, ct);

        return sessions.Select(x => MapSession(x, counts.GetValueOrDefault(x.Id))).ToList();
    }

    public async Task<TrainingSessionDto?> GetSessionAsync(Guid id, CancellationToken ct = default)
    {
        var item = await _db.Sessions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (item == null) return null;
        var count = await _db.Enrollments.CountAsync(x => x.SessionId == id, ct);
        return MapSession(item, count);
    }

    public async Task<TrainingSessionDto> CreateSessionAsync(TrainingSessionCreateDto dto, CancellationToken ct = default)
    {
        _ = await _db.Courses.FirstOrDefaultAsync(x => x.Id == dto.CourseId, ct)
            ?? throw new KeyNotFoundException("Course not found.");
        if (dto.EndAt <= dto.StartAt)
            throw new InvalidOperationException("Session EndAt must be after StartAt.");

        var e = new LocalTrainingSession
        {
            CourseId = dto.CourseId,
            Title = dto.Title.Trim(),
            StartAt = dto.StartAt,
            EndAt = dto.EndAt,
            Location = dto.Location,
            Mode = dto.Mode,
            TrainerName = dto.TrainerName,
            TrainerEmployeeId = dto.TrainerEmployeeId,
            Capacity = dto.Capacity,
            Notes = dto.Notes,
            Status = SessionStatus.Scheduled
        };
        _db.Sessions.Add(e);
        await _db.SaveChangesAsync(ct);
        return MapSession(e, 0);
    }

    public async Task<TrainingSessionDto> UpdateSessionAsync(Guid id, TrainingSessionUpdateDto dto, CancellationToken ct = default)
    {
        var e = await _db.Sessions.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Session not found.");
        if (dto.EndAt <= dto.StartAt)
            throw new InvalidOperationException("Session EndAt must be after StartAt.");

        e.Title = dto.Title.Trim();
        e.Status = dto.Status;
        e.StartAt = dto.StartAt;
        e.EndAt = dto.EndAt;
        e.Location = dto.Location;
        e.Mode = dto.Mode;
        e.TrainerName = dto.TrainerName;
        e.TrainerEmployeeId = dto.TrainerEmployeeId;
        e.Capacity = dto.Capacity;
        e.Notes = dto.Notes;
        e.DateMod = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        var count = await _db.Enrollments.CountAsync(x => x.SessionId == id, ct);
        return MapSession(e, count);
    }

    public async Task DeleteSessionAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _db.Sessions.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Session not found.");
        e.IsDeleted = true;
        e.DateMod = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<TrainingEnrollmentDto>> GetEnrollmentsAsync(Guid? programId = null, Guid? employeeId = null, Guid? sessionId = null, CancellationToken ct = default)
    {
        var q = _db.Enrollments.AsNoTracking().AsQueryable();
        if (programId.HasValue) q = q.Where(x => x.ProgramId == programId);
        if (employeeId.HasValue) q = q.Where(x => x.EmployeeId == employeeId);
        if (sessionId.HasValue) q = q.Where(x => x.SessionId == sessionId);
        var items = await q.OrderByDescending(x => x.EnrolledAt).ToListAsync(ct);
        return items.Select(MapEnrollment).ToList();
    }

    public async Task<TrainingEnrollmentDto> EnrollAsync(TrainingEnrollmentCreateDto dto, CancellationToken ct = default)
    {
        _ = await _db.Programs.FirstOrDefaultAsync(x => x.Id == dto.ProgramId, ct)
            ?? throw new KeyNotFoundException("Program not found.");

        if (dto.SessionId.HasValue)
        {
            var session = await _db.Sessions.FirstOrDefaultAsync(x => x.Id == dto.SessionId, ct)
                ?? throw new KeyNotFoundException("Session not found.");
            if (session.Capacity.HasValue)
            {
                var enrolled = await _db.Enrollments.CountAsync(x =>
                    x.SessionId == dto.SessionId &&
                    x.Status != EnrollmentStatus.Cancelled, ct);
                if (enrolled >= session.Capacity.Value)
                    throw new InvalidOperationException("Session is at capacity.");
            }
        }

        var duplicate = await _db.Enrollments.AnyAsync(x =>
            x.ProgramId == dto.ProgramId &&
            x.EmployeeId == dto.EmployeeId &&
            x.SessionId == dto.SessionId &&
            x.Status != EnrollmentStatus.Cancelled, ct);
        if (duplicate)
            throw new InvalidOperationException("Employee is already enrolled.");

        var e = new LocalTrainingEnrollment
        {
            ProgramId = dto.ProgramId,
            CourseId = dto.CourseId,
            SessionId = dto.SessionId,
            EmployeeId = dto.EmployeeId,
            Notes = dto.Notes,
            Status = EnrollmentStatus.Enrolled
        };
        _db.Enrollments.Add(e);
        await _db.SaveChangesAsync(ct);
        return MapEnrollment(e);
    }

    public async Task<TrainingEnrollmentDto> UpdateEnrollmentStatusAsync(Guid id, TrainingEnrollmentStatusDto dto, CancellationToken ct = default)
    {
        var e = await _db.Enrollments.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Enrollment not found.");
        e.Status = dto.Status;
        e.Notes = dto.Notes ?? e.Notes;
        e.DateMod = DateTime.UtcNow;
        if (dto.Status is EnrollmentStatus.Completed or EnrollmentStatus.Attended)
            e.CompletedAt ??= DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return MapEnrollment(e);
    }

    public async Task DeleteEnrollmentAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _db.Enrollments.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Enrollment not found.");
        e.IsDeleted = true;
        e.DateMod = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<TrainingEvaluationDto>> GetEvaluationsAsync(Guid? programId = null, Guid? employeeId = null, CancellationToken ct = default)
    {
        var q = _db.Evaluations.AsNoTracking().AsQueryable();
        if (programId.HasValue) q = q.Where(x => x.ProgramId == programId);
        if (employeeId.HasValue) q = q.Where(x => x.EmployeeId == employeeId);
        var items = await q.OrderByDescending(x => x.SubmittedAt).ToListAsync(ct);
        return items.Select(MapEvaluation).ToList();
    }

    public async Task<TrainingEvaluationDto> SubmitEvaluationAsync(TrainingEvaluationCreateDto dto, CancellationToken ct = default)
    {
        if (dto.Rating is < 1 or > 5)
            throw new InvalidOperationException("Rating must be between 1 and 5.");

        var enrollment = await _db.Enrollments.FirstOrDefaultAsync(x => x.Id == dto.EnrollmentId, ct)
            ?? throw new KeyNotFoundException("Enrollment not found.");

        var e = new LocalTrainingEvaluation
        {
            EnrollmentId = dto.EnrollmentId,
            EmployeeId = enrollment.EmployeeId,
            SessionId = enrollment.SessionId,
            ProgramId = enrollment.ProgramId,
            Rating = dto.Rating,
            Feedback = dto.Feedback,
            Strengths = dto.Strengths,
            Improvements = dto.Improvements
        };
        _db.Evaluations.Add(e);
        await _db.SaveChangesAsync(ct);
        return MapEvaluation(e);
    }

    public async Task<List<TrainingCertificateDto>> GetCertificatesAsync(Guid? employeeId = null, Guid? programId = null, CancellationToken ct = default)
    {
        var q = _db.Certificates.AsNoTracking().AsQueryable();
        if (employeeId.HasValue) q = q.Where(x => x.EmployeeId == employeeId);
        if (programId.HasValue) q = q.Where(x => x.ProgramId == programId);
        var items = await q.OrderByDescending(x => x.IssuedAt).ToListAsync(ct);
        return items.Select(MapCertificate).ToList();
    }

    public async Task<TrainingCertificateDto?> GetCertificateAsync(Guid id, CancellationToken ct = default)
    {
        var item = await _db.Certificates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return item == null ? null : MapCertificate(item);
    }

    public async Task<TrainingCertificateDto> IssueCertificateAsync(TrainingCertificateIssueDto dto, CancellationToken ct = default)
    {
        var enrollment = await _db.Enrollments.FirstOrDefaultAsync(x => x.Id == dto.EnrollmentId, ct)
            ?? throw new KeyNotFoundException("Enrollment not found.");

        var program = await _db.Programs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == enrollment.ProgramId, ct)
            ?? throw new KeyNotFoundException("Program not found.");

        var existing = await _db.Certificates.FirstOrDefaultAsync(x =>
            x.EnrollmentId == dto.EnrollmentId && x.Status == CertificateStatus.Issued, ct);
        if (existing != null)
            return MapCertificate(existing);

        var e = new LocalTrainingCertificate
        {
            EnrollmentId = dto.EnrollmentId,
            EmployeeId = enrollment.EmployeeId,
            ProgramId = enrollment.ProgramId,
            CertificateNumber = $"TRN-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}",
            Title = string.IsNullOrWhiteSpace(dto.Title) ? program.Title : dto.Title.Trim(),
            ExpiresAt = dto.ExpiresAt,
            IssuedBy = dto.IssuedBy,
            Notes = dto.Notes,
            Status = CertificateStatus.Issued
        };
        _db.Certificates.Add(e);

        enrollment.Status = EnrollmentStatus.Completed;
        enrollment.CompletedAt ??= DateTime.UtcNow;
        enrollment.DateMod = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return MapCertificate(e);
    }

    public async Task<TrainingCertificateDto?> VerifyCertificateAsync(string certificateNumber, CancellationToken ct = default)
    {
        var item = await _db.Certificates.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CertificateNumber == certificateNumber, ct);
        return item == null ? null : MapCertificate(item);
    }

    private static TrainingProgramDto MapProgram(LocalTrainingProgram x, int courseCount) => new()
    {
        Id = x.Id,
        Code = x.Code,
        Title = x.Title,
        Description = x.Description,
        Category = x.Category,
        Status = x.Status,
        DurationHours = x.DurationHours,
        Provider = x.Provider,
        IsMandatory = x.IsMandatory,
        StartDate = x.StartDate,
        EndDate = x.EndDate,
        CourseCount = courseCount
    };

    private static TrainingCourseDto MapCourse(LocalTrainingCourse x) => new()
    {
        Id = x.Id,
        ProgramId = x.ProgramId,
        Title = x.Title,
        Description = x.Description,
        Objectives = x.Objectives,
        Sequence = x.Sequence,
        DurationHours = x.DurationHours
    };

    private static TrainingSessionDto MapSession(LocalTrainingSession x, int enrollmentCount) => new()
    {
        Id = x.Id,
        CourseId = x.CourseId,
        Title = x.Title,
        Status = x.Status,
        StartAt = x.StartAt,
        EndAt = x.EndAt,
        Location = x.Location,
        Mode = x.Mode,
        TrainerName = x.TrainerName,
        TrainerEmployeeId = x.TrainerEmployeeId,
        Capacity = x.Capacity,
        EnrollmentCount = enrollmentCount,
        Notes = x.Notes
    };

    private static TrainingEnrollmentDto MapEnrollment(LocalTrainingEnrollment x) => new()
    {
        Id = x.Id,
        ProgramId = x.ProgramId,
        CourseId = x.CourseId,
        SessionId = x.SessionId,
        EmployeeId = x.EmployeeId,
        Status = x.Status,
        EnrolledAt = x.EnrolledAt,
        CompletedAt = x.CompletedAt,
        Notes = x.Notes
    };

    private static TrainingEvaluationDto MapEvaluation(LocalTrainingEvaluation x) => new()
    {
        Id = x.Id,
        EnrollmentId = x.EnrollmentId,
        EmployeeId = x.EmployeeId,
        SessionId = x.SessionId,
        ProgramId = x.ProgramId,
        Rating = x.Rating,
        Feedback = x.Feedback,
        Strengths = x.Strengths,
        Improvements = x.Improvements,
        SubmittedAt = x.SubmittedAt
    };

    private static TrainingCertificateDto MapCertificate(LocalTrainingCertificate x) => new()
    {
        Id = x.Id,
        EnrollmentId = x.EnrollmentId,
        EmployeeId = x.EmployeeId,
        ProgramId = x.ProgramId,
        CertificateNumber = x.CertificateNumber,
        Title = x.Title,
        Status = x.Status,
        IssuedAt = x.IssuedAt,
        ExpiresAt = x.ExpiresAt,
        IssuedBy = x.IssuedBy,
        Notes = x.Notes
    };
}
