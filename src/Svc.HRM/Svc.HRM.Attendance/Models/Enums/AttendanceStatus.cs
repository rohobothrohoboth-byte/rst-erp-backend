namespace Svc.HRM.Attendance.Models.Enums;

public enum AttendanceStatus
{
    Present = 1,
    Absent = 2,
    Late = 3,
    Leave = 4,
    Holiday = 5,
    Weekend = 6,
    HalfDay = 7
}

public enum OvertimeStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Cancelled = 4
}

public enum LeaveType
{
    Annual = 1,
    Sick = 2,
    Casual = 3,
    Maternity = 4,
    Paternity = 5,
    Study = 6,
    Unpaid = 7,
    Compensatory = 8
}

public enum LeaveStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Cancelled = 4
}