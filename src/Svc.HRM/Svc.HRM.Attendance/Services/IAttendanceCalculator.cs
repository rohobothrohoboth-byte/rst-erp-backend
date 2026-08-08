using Svc.HRM.Attendance.Models.Entities;
using Svc.HRM.Attendance.Models.Enums;
namespace Svc.HRM.Attendance.Services;

public interface IAttendanceCalculator
{
    double CalculateHoursWorked(DateTime checkIn, DateTime checkOut, LocalShift? shift = null);
    double CalculateOvertimeHours(DateTime checkIn, DateTime checkOut, LocalShift? shift = null);
    bool IsLate(DateTime checkIn, LocalShift shift, int graceMinutes = 15);
    int CalculateLateMinutes(DateTime checkIn, LocalShift shift, int graceMinutes = 15);
    bool IsEarlyDeparture(DateTime checkOut, LocalShift shift, int graceMinutes = 15);
    int CalculateEarlyDepartureMinutes(DateTime checkOut, LocalShift shift, int graceMinutes = 15);
    double CalculateWorkingDays(DateTime start, DateTime end, List<DateTime> holidays);
    AttendanceStatus DetermineAttendanceStatus(DateTime checkIn, DateTime checkOut, LocalShift shift, bool isHoliday, bool isWeekend);
}