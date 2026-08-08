namespace Svc.HRM.Attendance.Services;

public interface IAttendanceEventPublisher
{
    Task PublishAttendanceClockedInAsync(Guid employeeId, Guid attendanceId, DateTime checkIn, CancellationToken ct = default);
    Task PublishAttendanceClockedOutAsync(Guid employeeId, Guid attendanceId, DateTime checkOut, double hoursWorked, CancellationToken ct = default);
    Task PublishLeaveApprovedAsync(Guid employeeId, Guid leaveId, DateTime startDate, DateTime endDate, double days, CancellationToken ct = default);
    Task PublishOvertimeApprovedAsync(Guid employeeId, DateTime date, double hours, CancellationToken ct = default);
}