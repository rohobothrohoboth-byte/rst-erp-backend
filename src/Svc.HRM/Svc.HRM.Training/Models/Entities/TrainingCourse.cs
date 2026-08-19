namespace Svc.HRM.Training.Models.Entities;

public class TrainingCourse : BaseEntity
{
    public Guid ProgramId { get; set; }
    public TrainingProgram? Program { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Instructor { get; set; }
    public int DurationHours { get; set; }
    public string? Location { get; set; }
    public int Capacity { get; set; }
    public DateTime? ScheduledDate { get; set; }
}
