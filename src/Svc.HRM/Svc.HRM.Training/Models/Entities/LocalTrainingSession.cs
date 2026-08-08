namespace Svc.HRM.Training.Models.Entities;

public class LocalTrainingSession
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid CourseId { get; set; }
    public string Title { get; set; } = default!;
    public string Status { get; set; } = "Scheduled";
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public string? Location { get; set; }
    public string? Mode { get; set; } = "InPerson"; // InPerson | Online | Hybrid
    public string? TrainerName { get; set; }
    public Guid? TrainerEmployeeId { get; set; }
    public int? Capacity { get; set; }
    public string? Notes { get; set; }
    public DateTime DateAdd { get; set; } = DateTime.UtcNow;
    public DateTime? DateMod { get; set; }
    public bool IsDeleted { get; set; }

    public LocalTrainingCourse? Course { get; set; }
    public ICollection<LocalTrainingEnrollment> Enrollments { get; set; } = new List<LocalTrainingEnrollment>();
}
