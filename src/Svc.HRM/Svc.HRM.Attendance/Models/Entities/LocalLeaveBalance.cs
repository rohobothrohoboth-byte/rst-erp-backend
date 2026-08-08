using Svc.HRM.Attendance.Models.Enums;

namespace Svc.HRM.Attendance.Models.Entities;

public class LocalLeaveBalance
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid EmployeeId { get; set; }
    public string LeaveType { get; set; } = default!;
    public double TotalDays { get; set; }
    public double UsedDays { get; set; }
    public double BalanceDays => TotalDays - UsedDays;
    public int Year { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime DateAdd { get; set; } = DateTime.UtcNow;
    public DateTime? DateMod { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime SyncedAt { get; set; } = DateTime.UtcNow;
}