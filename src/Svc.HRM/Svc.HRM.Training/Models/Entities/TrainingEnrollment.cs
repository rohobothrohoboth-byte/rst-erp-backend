namespace Svc.HRM.Training.Models.Entities;

public class TrainingEnrollment : BaseEntity
{
    public Guid CourseId { get; set; }
    public TrainingCourse? Course { get; set; }

    public Guid EmployeeId { get; set; }
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Enrolled";
    public decimal? Score { get; set; }
    public bool CertificateIssued { get; set; }
    public string? Feedback { get; set; }
}
