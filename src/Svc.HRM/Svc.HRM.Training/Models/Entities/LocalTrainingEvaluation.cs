namespace Svc.HRM.Training.Models.Entities;

public class LocalTrainingEvaluation
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid EnrollmentId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid? SessionId { get; set; }
    public Guid? ProgramId { get; set; }
    public int Rating { get; set; } // 1-5
    public string? Feedback { get; set; }
    public string? Strengths { get; set; }
    public string? Improvements { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public DateTime DateAdd { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }

    public LocalTrainingEnrollment? Enrollment { get; set; }
}
