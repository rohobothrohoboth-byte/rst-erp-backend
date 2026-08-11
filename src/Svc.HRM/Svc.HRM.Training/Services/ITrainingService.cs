using Svc.HRM.Training.Models.DTOs;
using Svc.HRM.Training.Models.Entities;

namespace Svc.HRM.Training.Services;

public interface ITrainingService
{
    // Programs
    Task<List<TrainingProgram>> GetProgramsAsync(CancellationToken ct = default);
    Task<TrainingProgram?> GetProgramAsync(Guid id, CancellationToken ct = default);
    Task<TrainingProgram> CreateProgramAsync(TrainingProgramCreateDto dto, CancellationToken ct = default);
    Task<TrainingProgram?> UpdateProgramAsync(Guid id, TrainingProgramCreateDto dto, CancellationToken ct = default);
    Task<bool> DeleteProgramAsync(Guid id, CancellationToken ct = default);

    // Courses
    Task<List<TrainingCourse>> GetCoursesAsync(CancellationToken ct = default);
    Task<TrainingCourse?> GetCourseAsync(Guid id, CancellationToken ct = default);
    Task<List<TrainingCourse>> GetCoursesByProgramAsync(Guid programId, CancellationToken ct = default);
    Task<TrainingCourse> CreateCourseAsync(TrainingCourseCreateDto dto, CancellationToken ct = default);
    Task<TrainingCourse?> UpdateCourseAsync(Guid id, TrainingCourseCreateDto dto, CancellationToken ct = default);
    Task<bool> DeleteCourseAsync(Guid id, CancellationToken ct = default);

    // Enrollments
    Task<List<TrainingEnrollment>> GetEnrollmentsAsync(CancellationToken ct = default);
    Task<List<TrainingEnrollment>> GetEnrollmentsByEmployeeAsync(Guid employeeId, CancellationToken ct = default);
    Task<TrainingEnrollment> CreateEnrollmentAsync(TrainingEnrollmentCreateDto dto, CancellationToken ct = default);
    Task<TrainingEnrollment?> UpdateEnrollmentAsync(Guid id, TrainingEnrollmentCreateDto dto, CancellationToken ct = default);
    Task<bool> DeleteEnrollmentAsync(Guid id, CancellationToken ct = default);
    Task<TrainingEnrollment?> IssueCertificateAsync(Guid id, CancellationToken ct = default);
}
