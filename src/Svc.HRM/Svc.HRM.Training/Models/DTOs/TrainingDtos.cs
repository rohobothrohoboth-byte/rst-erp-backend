namespace Svc.HRM.Training.Models.DTOs;

public class TrainingProgramDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string Category { get; set; } = default!;
    public string Status { get; set; } = default!;
    public int? DurationHours { get; set; }
    public string? Provider { get; set; }
    public bool IsMandatory { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int CourseCount { get; set; }
}

public class TrainingProgramCreateDto
{
    public string Code { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string Category { get; set; } = "General";
    public int? DurationHours { get; set; }
    public string? Provider { get; set; }
    public bool IsMandatory { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class TrainingProgramUpdateDto
{
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string Category { get; set; } = "General";
    public string Status { get; set; } = "Draft";
    public int? DurationHours { get; set; }
    public string? Provider { get; set; }
    public bool IsMandatory { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class TrainingCourseDto
{
    public Guid Id { get; set; }
    public Guid ProgramId { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string? Objectives { get; set; }
    public int Sequence { get; set; }
    public int? DurationHours { get; set; }
}

public class TrainingCourseCreateDto
{
    public Guid ProgramId { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string? Objectives { get; set; }
    public int Sequence { get; set; }
    public int? DurationHours { get; set; }
}

public class TrainingCourseUpdateDto
{
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string? Objectives { get; set; }
    public int Sequence { get; set; }
    public int? DurationHours { get; set; }
}

public class TrainingSessionDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string Title { get; set; } = default!;
    public string Status { get; set; } = default!;
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public string? Location { get; set; }
    public string? Mode { get; set; }
    public string? TrainerName { get; set; }
    public Guid? TrainerEmployeeId { get; set; }
    public int? Capacity { get; set; }
    public int EnrollmentCount { get; set; }
    public string? Notes { get; set; }
}

public class TrainingSessionCreateDto
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = default!;
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public string? Location { get; set; }
    public string? Mode { get; set; } = "InPerson";
    public string? TrainerName { get; set; }
    public Guid? TrainerEmployeeId { get; set; }
    public int? Capacity { get; set; }
    public string? Notes { get; set; }
}

public class TrainingSessionUpdateDto
{
    public string Title { get; set; } = default!;
    public string Status { get; set; } = "Scheduled";
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public string? Location { get; set; }
    public string? Mode { get; set; }
    public string? TrainerName { get; set; }
    public Guid? TrainerEmployeeId { get; set; }
    public int? Capacity { get; set; }
    public string? Notes { get; set; }
}

public class TrainingEnrollmentDto
{
    public Guid Id { get; set; }
    public Guid ProgramId { get; set; }
    public Guid? CourseId { get; set; }
    public Guid? SessionId { get; set; }
    public Guid EmployeeId { get; set; }
    public string Status { get; set; } = default!;
    public DateTime EnrolledAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Notes { get; set; }
}

public class TrainingEnrollmentCreateDto
{
    public Guid ProgramId { get; set; }
    public Guid? CourseId { get; set; }
    public Guid? SessionId { get; set; }
    public Guid EmployeeId { get; set; }
    public string? Notes { get; set; }
}

public class TrainingEnrollmentStatusDto
{
    public string Status { get; set; } = default!;
    public string? Notes { get; set; }
}

public class TrainingEvaluationDto
{
    public Guid Id { get; set; }
    public Guid EnrollmentId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid? SessionId { get; set; }
    public Guid? ProgramId { get; set; }
    public int Rating { get; set; }
    public string? Feedback { get; set; }
    public string? Strengths { get; set; }
    public string? Improvements { get; set; }
    public DateTime SubmittedAt { get; set; }
}

public class TrainingEvaluationCreateDto
{
    public Guid EnrollmentId { get; set; }
    public int Rating { get; set; }
    public string? Feedback { get; set; }
    public string? Strengths { get; set; }
    public string? Improvements { get; set; }
}

public class TrainingCertificateDto
{
    public Guid Id { get; set; }
    public Guid EnrollmentId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid ProgramId { get; set; }
    public string CertificateNumber { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Status { get; set; } = default!;
    public DateTime IssuedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? IssuedBy { get; set; }
    public string? Notes { get; set; }
}

public class TrainingCertificateIssueDto
{
    public Guid EnrollmentId { get; set; }
    public string? Title { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? IssuedBy { get; set; }
    public string? Notes { get; set; }
}
