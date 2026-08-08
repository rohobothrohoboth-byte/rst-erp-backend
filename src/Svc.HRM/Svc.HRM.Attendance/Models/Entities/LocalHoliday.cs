namespace Svc.HRM.Attendance.Models.Entities;

public class LocalHoliday
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public DateTime Date { get; set; }
    public bool IsRecurring { get; set; }
    public int? DayOfMonth { get; set; }
    public int? Month { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime DateAdd { get; set; } = DateTime.UtcNow;
    public DateTime? DateMod { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime SyncedAt { get; set; } = DateTime.UtcNow;
}