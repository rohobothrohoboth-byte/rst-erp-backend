using Svc.HRM.Attendance.Models.Entities;
using Svc.HRM.Attendance.Models.Enums;

namespace Svc.HRM.Attendance.Services;

public class AttendanceCalculator : IAttendanceCalculator
{
    private readonly ILogger<AttendanceCalculator> _logger;

    public AttendanceCalculator(ILogger<AttendanceCalculator> logger)
    {
        _logger = logger;
    }

    public double CalculateHoursWorked(DateTime checkIn, DateTime checkOut, LocalShift? shift = null)
    {
        if (checkIn == DateTime.MinValue || checkOut == DateTime.MinValue)
            return 0;

        var totalHours = (checkOut - checkIn).TotalHours;

        // Subtract break time if shift is provided
        if (shift != null)
        {
            totalHours -= shift.BreakDurationHours;
        }

        return Math.Max(0, totalHours);
    }

    public double CalculateOvertimeHours(DateTime checkIn, DateTime checkOut, LocalShift? shift = null)
    {
        if (shift == null || checkIn == DateTime.MinValue || checkOut == DateTime.MinValue)
            return 0;

        var shiftEnd = checkIn.Date.Add(shift.EndTime);
        if (checkOut > shiftEnd)
        {
            return (checkOut - shiftEnd).TotalHours;
        }

        return 0;
    }

    public bool IsLate(DateTime checkIn, LocalShift shift, int graceMinutes = 15)
    {
        if (shift == null || checkIn == DateTime.MinValue)
            return false;

        var shiftStart = checkIn.Date.Add(shift.StartTime);
        var graceTime = shiftStart.AddMinutes(graceMinutes);

        return checkIn > graceTime;
    }

    public int CalculateLateMinutes(DateTime checkIn, LocalShift shift, int graceMinutes = 15)
    {
        if (!IsLate(checkIn, shift, graceMinutes))
            return 0;

        var shiftStart = checkIn.Date.Add(shift.StartTime);
        return (int)(checkIn - shiftStart).TotalMinutes;
    }

    public bool IsEarlyDeparture(DateTime checkOut, LocalShift shift, int graceMinutes = 15)
    {
        if (shift == null || checkOut == DateTime.MinValue)
            return false;

        var shiftEnd = checkOut.Date.Add(shift.EndTime);
        var graceTime = shiftEnd.AddMinutes(-graceMinutes);

        return checkOut < graceTime;
    }

    public int CalculateEarlyDepartureMinutes(DateTime checkOut, LocalShift shift, int graceMinutes = 15)
    {
        if (!IsEarlyDeparture(checkOut, shift, graceMinutes))
            return 0;

        var shiftEnd = checkOut.Date.Add(shift.EndTime);
        return (int)(shiftEnd - checkOut).TotalMinutes;
    }

    public double CalculateWorkingDays(DateTime start, DateTime end, List<DateTime> holidays)
    {
        int workingDays = 0;
        for (var date = start; date <= end; date = date.AddDays(1))
        {
            if (date.DayOfWeek != DayOfWeek.Saturday &&
                date.DayOfWeek != DayOfWeek.Sunday &&
                !holidays.Contains(date.Date))
            {
                workingDays++;
            }
        }
        return workingDays;
    }

    public AttendanceStatus DetermineAttendanceStatus(DateTime checkIn, DateTime checkOut, LocalShift shift, bool isHoliday, bool isWeekend)
    {
        if (isHoliday)
            return AttendanceStatus.Holiday;

        if (isWeekend)
            return AttendanceStatus.Weekend;

        if (checkIn == DateTime.MinValue && checkOut == DateTime.MinValue)
            return AttendanceStatus.Absent;

        if (IsLate(checkIn, shift, 15) && IsEarlyDeparture(checkOut, shift, 15))
            return AttendanceStatus.HalfDay;

        if (IsLate(checkIn, shift, 15))
            return AttendanceStatus.Late;

        return AttendanceStatus.Present;
    }
}