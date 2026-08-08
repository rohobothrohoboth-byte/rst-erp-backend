namespace Svc.HRM.Attendance.Models.Entities;

public class LocalShift
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string? Description { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public TimeSpan BreakStartTime { get; set; }
    public TimeSpan BreakEndTime { get; set; }
    public double BreakDurationHours { get; set; }
    public double TotalHours => (EndTime - StartTime).TotalHours - BreakDurationHours;
    public bool IsActive { get; set; } = true;
    public string? ColorCode { get; set; }
    public int Order { get; set; }
    public DateTime DateAdd { get; set; } = DateTime.UtcNow;
    public DateTime? DateMod { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime SyncedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<LocalShiftAssignment> ShiftAssignments { get; set; } = new List<LocalShiftAssignment>();
}