namespace Svc.HRM.Training.Models.Entities;

public class LocalTrainingProgram
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Code { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string Category { get; set; } = "General";
    public string Status { get; set; } = "Draft";
    public int? DurationHours { get; set; }
    public string? Provider { get; set; }
    public bool IsMandatory { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime DateAdd { get; set; } = DateTime.UtcNow;
    public DateTime? DateMod { get; set; }
    public bool IsDeleted { get; set; }

    public ICollection<LocalTrainingCourse> Courses { get; set; } = new List<LocalTrainingCourse>();
}
