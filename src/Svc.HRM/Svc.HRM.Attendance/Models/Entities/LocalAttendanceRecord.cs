using Svc.HRM.Attendance.Models.Enums;
using Svc.HRM.Attendance.Models.Entities.Local;

namespace Svc.HRM.Attendance.Models.Entities;

public class LocalAttendanceRecord
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    // ✅ Employee identifier (your app's employee ID)
    public Guid EmployeeId { get; set; }

    // ✅ ADD THIS: Foreign key to LocalEmployees table
    public Guid LocalEmployeeId { get; set; }  // ← CRITICAL: This was missing!

    public DateTime Date { get; set; }
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public string Status { get; set; } = AttendanceStatus.Present.ToString();
    public double HoursWorked { get; set; }
    public double OvertimeHours { get; set; }
    public bool IsLate { get; set; }
    public int LateMinutes { get; set; }
    public bool IsEarlyDeparture { get; set; }
    public int EarlyDepartureMinutes { get; set; }
    public Guid? ShiftId { get; set; }
    public string? CheckInLocation { get; set; }
    public string? CheckOutLocation { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime DateAdd { get; set; } = DateTime.UtcNow;
    public DateTime? DateMod { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime SyncedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual LocalEmployee LocalEmployee { get; set; } = null!;
    public LocalShift? Shift { get; set; }
}