using Svc.HRM.Attendance.Models.Enums;

namespace Svc.HRM.Attendance.Models.Entities;

public class LocalLeaveRequest
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid EmployeeId { get; set; }
    public string LeaveType { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double DaysRequested { get; set; }
    public double DaysApproved { get; set; }
    public string Reason { get; set; } = default!;
    public string Status { get; set; } = LeaveStatus.Pending.ToString();
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovedBy { get; set; }
    public string? RejectedReason { get; set; }
    public string? AttachmentUrl { get; set; }
    public string? RequestedBy { get; set; }
    public bool IsPaid { get; set; } = true;
    public DateTime DateAdd { get; set; } = DateTime.UtcNow;
    public DateTime? DateMod { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime SyncedAt { get; set; } = DateTime.UtcNow;
}