namespace Svc.HRM.Training.Models.Entities;

public class LocalTrainingCourse
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid ProgramId { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string? Objectives { get; set; }
    public int Sequence { get; set; }
    public int? DurationHours { get; set; }
    public DateTime DateAdd { get; set; } = DateTime.UtcNow;
    public DateTime? DateMod { get; set; }
    public bool IsDeleted { get; set; }

    public LocalTrainingProgram? Program { get; set; }
    public ICollection<LocalTrainingSession> Sessions { get; set; } = new List<LocalTrainingSession>();
}
