namespace Svc.HRM.Training.Models.Entities;

public class LocalTrainingEnrollment
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid ProgramId { get; set; }
    public Guid? CourseId { get; set; }
    public Guid? SessionId { get; set; }
    public Guid EmployeeId { get; set; }
    public string Status { get; set; } = "Enrolled";
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public string? Notes { get; set; }
    public DateTime DateAdd { get; set; } = DateTime.UtcNow;
    public DateTime? DateMod { get; set; }
    public bool IsDeleted { get; set; }

    public LocalTrainingProgram? Program { get; set; }
    public LocalTrainingSession? Session { get; set; }
}
