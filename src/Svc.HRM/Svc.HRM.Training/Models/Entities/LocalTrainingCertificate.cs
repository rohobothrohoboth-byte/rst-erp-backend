namespace Svc.HRM.Training.Models.Entities;

public class LocalTrainingCertificate
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid EnrollmentId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid ProgramId { get; set; }
    public string CertificateNumber { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Status { get; set; } = "Issued";
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    public string? IssuedBy { get; set; }
    public string? Notes { get; set; }
    public DateTime DateAdd { get; set; } = DateTime.UtcNow;
    public DateTime? DateMod { get; set; }
    public bool IsDeleted { get; set; }

    public LocalTrainingEnrollment? Enrollment { get; set; }
}
