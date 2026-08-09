using Svc.HRM.Training.Models.DTOs;

namespace Svc.HRM.Training.Services;

public interface ITrainingService
{
    Task<List<TrainingProgramDto>> GetProgramsAsync(string? status = null, CancellationToken ct = default);
    Task<TrainingProgramDto?> GetProgramAsync(Guid id, CancellationToken ct = default);
    Task<TrainingProgramDto> CreateProgramAsync(TrainingProgramCreateDto dto, CancellationToken ct = default);
    Task<TrainingProgramDto> UpdateProgramAsync(Guid id, TrainingProgramUpdateDto dto, CancellationToken ct = default);
    Task DeleteProgramAsync(Guid id, CancellationToken ct = default);
    Task<TrainingProgramDto> PublishProgramAsync(Guid id, CancellationToken ct = default);
    Task<TrainingProgramDto> CancelProgramAsync(Guid id, CancellationToken ct = default);

    Task<List<TrainingCourseDto>> GetCoursesAsync(Guid? programId = null, CancellationToken ct = default);
    Task<TrainingCourseDto?> GetCourseAsync(Guid id, CancellationToken ct = default);
    Task<TrainingCourseDto> CreateCourseAsync(TrainingCourseCreateDto dto, CancellationToken ct = default);
    Task<TrainingCourseDto> UpdateCourseAsync(Guid id, TrainingCourseUpdateDto dto, CancellationToken ct = default);
    Task DeleteCourseAsync(Guid id, CancellationToken ct = default);

    Task<List<TrainingSessionDto>> GetSessionsAsync(Guid? courseId = null, DateTime? from = null, DateTime? to = null, CancellationToken ct = default);
    Task<TrainingSessionDto?> GetSessionAsync(Guid id, CancellationToken ct = default);
    Task<TrainingSessionDto> CreateSessionAsync(TrainingSessionCreateDto dto, CancellationToken ct = default);
    Task<TrainingSessionDto> UpdateSessionAsync(Guid id, TrainingSessionUpdateDto dto, CancellationToken ct = default);
    Task DeleteSessionAsync(Guid id, CancellationToken ct = default);

    Task<List<TrainingEnrollmentDto>> GetEnrollmentsAsync(Guid? programId = null, Guid? employeeId = null, Guid? sessionId = null, CancellationToken ct = default);
    Task<TrainingEnrollmentDto> EnrollAsync(TrainingEnrollmentCreateDto dto, CancellationToken ct = default);
    Task<TrainingEnrollmentDto> UpdateEnrollmentStatusAsync(Guid id, TrainingEnrollmentStatusDto dto, CancellationToken ct = default);
    Task DeleteEnrollmentAsync(Guid id, CancellationToken ct = default);

    Task<List<TrainingEvaluationDto>> GetEvaluationsAsync(Guid? programId = null, Guid? employeeId = null, CancellationToken ct = default);
    Task<TrainingEvaluationDto> SubmitEvaluationAsync(TrainingEvaluationCreateDto dto, CancellationToken ct = default);

    Task<List<TrainingCertificateDto>> GetCertificatesAsync(Guid? employeeId = null, Guid? programId = null, CancellationToken ct = default);
    Task<TrainingCertificateDto?> GetCertificateAsync(Guid id, CancellationToken ct = default);
    Task<TrainingCertificateDto> IssueCertificateAsync(TrainingCertificateIssueDto dto, CancellationToken ct = default);
    Task<TrainingCertificateDto?> VerifyCertificateAsync(string certificateNumber, CancellationToken ct = default);
}
