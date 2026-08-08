namespace Svc.HRM.Performance.Models.Entities;

public class LocalReviewTemplate
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? CriteriaJson { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime DateAdd { get; set; } = DateTime.UtcNow;
    public DateTime? DateMod { get; set; }
    public bool IsDeleted { get; set; }
}
