using Svc.HRM.Attendance.Models.Enums;

namespace Svc.HRM.Attendance.Models.Entities;

public class LocalOvertimeRequest
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid EmployeeId { get; set; }
    public DateTime Date { get; set; }
    public double HoursRequested { get; set; }
    public double HoursApproved { get; set; }
    public string Reason { get; set; } = default!;
    public string Status { get; set; } = OvertimeStatus.Pending.ToString();
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovedBy { get; set; }
    public string? RejectedReason { get; set; }
    public string? RequestedBy { get; set; }
    public DateTime DateAdd { get; set; } = DateTime.UtcNow;
    public DateTime? DateMod { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime SyncedAt { get; set; } = DateTime.UtcNow;
}