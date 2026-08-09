using Microsoft.EntityFrameworkCore;
using Svc.HRM.Attendance.Models.DTOs;
using Svc.HRM.Attendance.Models.Entities;
using Svc.HRM.Attendance.Models.Enums;
using Svc.HRM.Attendance.Persistence;
using Shared.Helpers.Services;
using System.Text.Json;

namespace Svc.HRM.Attendance.Services;

public class AttendanceService : IAttendanceService
{
    private readonly AttendanceDbContext _context;
    private readonly IShiftService _shiftService;
    private readonly IAttendanceCalculator _calculator;
    private readonly IAttendanceEventPublisher _eventPublisher;
    private readonly ICacheService _cache;
    private readonly ILogger<AttendanceService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHrmLeaveClient _leaveClient;

    // ✅ Ethiopia Time Zone Offset (UTC+3)
    private static readonly TimeSpan EthiopiaOffset = TimeSpan.FromHours(3);

    public AttendanceService(
        AttendanceDbContext context,
        IShiftService shiftService,
        IAttendanceCalculator calculator,
        IAttendanceEventPublisher eventPublisher,
        ICacheService cache,
        ILogger<AttendanceService> logger,
        IServiceProvider serviceProvider,
        IHrmLeaveClient leaveClient)
    {
        _context = context;
        _shiftService = shiftService;
        _calculator = calculator;
        _eventPublisher = eventPublisher;
        _cache = cache;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _leaveClient = leaveClient;
    }

    // ✅ Get current time in Ethiopia (UTC+3)
   // ✅ Get current time in Ethiopia (UTC+3) with fallback
   private DateTime GetEthiopiaNow()
   {
       try
       {
           return DateTime.UtcNow.Add(EthiopiaOffset);
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, "Error calculating Ethiopia time, using UTC+3 fallback");
           return DateTime.UtcNow.AddHours(3);
       }
   }

   // ✅ Get today's date in Ethiopia with fallback
   private DateTime GetTodayInEthiopia()
   {
       try
       {
           return GetEthiopiaNow().Date;
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, "Error getting today's date in Ethiopia, using UTC fallback");
           return DateTime.UtcNow.Date;
       }
   }

   // ✅ Convert UTC to Ethiopia time with fallback
   private DateTime ToEthiopiaTime(DateTime utcDateTime)
   {
       try
       {
           return utcDateTime.Add(EthiopiaOffset);
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, "Error converting to Ethiopia time, using UTC+3 fallback");
           return utcDateTime.AddHours(3);
       }
   }

    // ✅ Convert Ethiopia time to UTC
    private DateTime ToUtc(DateTime ethiopiaDateTime)
    {
        return ethiopiaDateTime.Subtract(EthiopiaOffset);
    }

    // ✅ Helper method to ensure UTC
    private DateTime EnsureUtc(DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Unspecified)
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        if (dateTime.Kind == DateTimeKind.Local)
            return dateTime.ToUniversalTime();
        return dateTime;
    }

    // ✅ Helper method to ensure UTC for nullable DateTime
    private DateTime? EnsureUtc(DateTime? dateTime)
    {
        if (!dateTime.HasValue)
            return null;
        return EnsureUtc(dateTime.Value);
    }

    #region Clock In/Out

   public async Task<AttendanceRecordDto> ClockInAsync(Guid employeeId, ClockInDto dto, CancellationToken ct = default)
   {
       try
       {
           // ✅ FIRST: Find the employee in LocalEmployees table
           var localEmployee = await _context.LocalEmployees
               .FirstOrDefaultAsync(e => e.Id == employeeId, ct);

           if (localEmployee == null)
           {
               _logger.LogWarning("Employee {EmployeeId} not found in LocalEmployees. Checking by EmployeeId...", employeeId);

               // ✅ Try to find by AppUserId or other identifier if needed
               // For now, throw a clear error
               throw new InvalidOperationException($"Employee with ID '{employeeId}' not found in LocalEmployees. Please run the sync first.");
           }

           _logger.LogInformation("Found LocalEmployee: {Id} - {FirstName} {LastName} (Code: {Code})",
               localEmployee.Id, localEmployee.FirstName, localEmployee.LastName, localEmployee.Code);

           // ✅ Use Ethiopia time for date
           var today = GetTodayInEthiopia();
           var checkIn = EnsureUtc(dto.CheckIn ?? DateTime.UtcNow);

           _logger.LogInformation($"ClockInAsync - Employee: {employeeId}");
           _logger.LogInformation($"  LocalEmployeeId: {localEmployee.Id}");
           _logger.LogInformation($"  Ethiopia Date: {today:yyyy-MM-dd}");
           _logger.LogInformation($"  UTC CheckIn: {checkIn:yyyy-MM-dd HH:mm:ss} UTC");

           // ✅ Check if already clocked in TODAY
           var existing = await _context.AttendanceRecords
               .FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.Date == today && !x.IsDeleted, ct);

           if (existing != null)
           {
               var ethiopiaCheckIn = existing.CheckIn.HasValue ? ToEthiopiaTime(existing.CheckIn.Value) : (DateTime?)null;
               var checkInTime = ethiopiaCheckIn?.ToString("yyyy-MM-dd HH:mm:ss") ?? "unknown";
               _logger.LogWarning($"Employee {employeeId} already clocked in today at {checkInTime} (Ethiopia time)");
               throw new InvalidOperationException($"Employee already clocked in today at {checkInTime}");
           }

           // Get employee's shift
           var shift = await _shiftService.GetEmployeeShiftAsync(employeeId, today, ct);

           // ✅ Create the attendance record with the correct LocalEmployeeId
           var record = new LocalAttendanceRecord
           {
               EmployeeId = employeeId,
               LocalEmployeeId = localEmployee.Id,  // ✅ CRITICAL: Use the actual ID from LocalEmployees
               Date = today,
               CheckIn = checkIn,
               ShiftId = shift?.Id,
               Status = AttendanceStatus.Present.ToString(),
               CheckInLocation = dto.Location,
               Notes = dto.Notes,
               CreatedBy = "System",
               DateAdd = DateTime.UtcNow,
               IsDeleted = false,
               SyncedAt = DateTime.UtcNow
           };

           _logger.LogInformation("Creating attendance record with LocalEmployeeId: {LocalEmployeeId}", record.LocalEmployeeId);

           // Calculate late status using Ethiopia time
           if (shift != null)
           {
               var ethiopiaCheckIn = ToEthiopiaTime(checkIn);
               record.IsLate = _calculator.IsLate(ethiopiaCheckIn, shift);
               record.LateMinutes = _calculator.CalculateLateMinutes(ethiopiaCheckIn, shift);
               record.Status = record.IsLate ? AttendanceStatus.Late.ToString() : AttendanceStatus.Present.ToString();
           }

           await _context.AttendanceRecords.AddAsync(record, ct);
           await _context.SaveChangesAsync(ct);

           // Publish event
           await _eventPublisher.PublishAttendanceClockedInAsync(employeeId, record.Id, checkIn, ct);

           // Clear cache
           await _cache.RemoveAsync($"attendance_today_{employeeId}", ct);
           await _cache.RemoveAsync($"attendance_employee_{employeeId}_summary", ct);

           return MapToDto(record);
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, $"Error in ClockInAsync for employee {employeeId}");
           throw;
       }
   }

    public async Task<AttendanceRecordDto> ClockOutAsync(Guid employeeId, ClockOutDto dto, CancellationToken ct = default)
    {
        try
        {
            // ✅ Use Ethiopia time for date
            var today = GetTodayInEthiopia();
            var checkOut = EnsureUtc(dto.CheckOut ?? DateTime.UtcNow);

            _logger.LogInformation($"ClockOutAsync - Employee: {employeeId}");
            _logger.LogInformation($"  Ethiopia Date: {today:yyyy-MM-dd}");
            _logger.LogInformation($"  UTC CheckOut: {checkOut:yyyy-MM-dd HH:mm:ss} UTC");

            var record = await _context.AttendanceRecords
                .Include(x => x.Shift)
                .FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.Date == today && !x.IsDeleted, ct);

            if (record == null)
            {
                _logger.LogWarning($"No clock-in record found for employee {employeeId} on {today:yyyy-MM-dd}");
                throw new KeyNotFoundException($"No clock-in record found for today ({today:yyyy-MM-dd})");
            }

            if (record.CheckOut.HasValue)
            {
                var ethiopiaCheckOut = ToEthiopiaTime(record.CheckOut.Value);
                _logger.LogWarning($"Employee {employeeId} already clocked out at {ethiopiaCheckOut:HH:mm:ss}");
                throw new InvalidOperationException($"Already clocked out for today at {ethiopiaCheckOut:HH:mm:ss}");
            }

            record.CheckOut = checkOut;
            record.CheckOutLocation = dto.Location;
            record.Notes = string.IsNullOrEmpty(record.Notes) ? dto.Notes : $"{record.Notes} | {dto.Notes}";
            record.DateMod = DateTime.UtcNow;

            // Calculate hours
            var shift = record.Shift;

            record.HoursWorked = _calculator.CalculateHoursWorked(
                record.CheckIn ?? DateTime.MinValue,
                checkOut,
                shift
            );
           record.OvertimeHours = _calculator.CalculateOvertimeHours(
               record.CheckIn ?? DateTime.MinValue,
               checkOut,
               shift
           );

            // Check early departure using Ethiopia time
            if (shift != null)
            {
                var ethiopiaCheckOut = ToEthiopiaTime(checkOut);
                record.IsEarlyDeparture = _calculator.IsEarlyDeparture(ethiopiaCheckOut, shift);
                record.EarlyDepartureMinutes = _calculator.CalculateEarlyDepartureMinutes(ethiopiaCheckOut, shift);
            }

            await _context.SaveChangesAsync(ct);

            // Publish event
            await _eventPublisher.PublishAttendanceClockedOutAsync(employeeId, record.Id, checkOut, record.HoursWorked, ct);

            // Clear cache
            await _cache.RemoveAsync($"attendance_today_{employeeId}", ct);
            await _cache.RemoveAsync($"attendance_employee_{employeeId}_summary", ct);

            return MapToDto(record);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in ClockOutAsync for employee {employeeId}");
            throw;
        }
    }

    #endregion

    #region Get Attendance

    public async Task<AttendanceRecordDto> GetAttendanceByIdAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var cacheKey = $"attendance_{id}";
            var cached = await _cache.GetAsync<AttendanceRecordDto>(cacheKey, ct);
            if (cached != null)
                return cached;

            var record = await _context.AttendanceRecords
                .Include(x => x.Shift)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);

            if (record == null)
                throw new KeyNotFoundException($"Attendance record {id} not found");

            var dto = MapToDto(record);

            // Convert times to Ethiopia time for display
            if (dto.CheckIn.HasValue)
                dto.CheckIn = ToEthiopiaTime(dto.CheckIn.Value);
            if (dto.CheckOut.HasValue)
                dto.CheckOut = ToEthiopiaTime(dto.CheckOut.Value);

            await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(15), ct);
            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in GetAttendanceByIdAsync for id {id}");
            throw;
        }
    }

    public async Task<AttendanceRecordDto> GetTodayAttendanceAsync(Guid employeeId, CancellationToken ct = default)
    {
        try
        {
            var cacheKey = $"attendance_today_{employeeId}";
            var cached = await _cache.GetAsync<AttendanceRecordDto>(cacheKey, ct);
            if (cached != null)
                return cached;

            // ✅ Use Ethiopia date with try-catch
            DateTime today;
            try
            {
                today = GetTodayInEthiopia();
                _logger.LogInformation($"GetTodayAttendanceAsync - Employee: {employeeId}, Ethiopia Date: {today:yyyy-MM-dd}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting Ethiopia date for employee {employeeId}");
                // Fallback to UTC date if Ethiopia time fails
                today = DateTime.UtcNow.Date;
                _logger.LogWarning($"Using UTC date as fallback: {today:yyyy-MM-dd}");
            }

            var record = await _context.AttendanceRecords
                .Include(x => x.Shift)
                .FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.Date == today && !x.IsDeleted, ct);

            if (record == null)
            {
                _logger.LogInformation($"No attendance record found for employee {employeeId} on {today:yyyy-MM-dd}");
                return null!;
            }

            var dto = MapToDto(record);

            // Convert times to Ethiopia time for display
            if (dto.CheckIn.HasValue)
                dto.CheckIn = ToEthiopiaTime(dto.CheckIn.Value);
            if (dto.CheckOut.HasValue)
                dto.CheckOut = ToEthiopiaTime(dto.CheckOut.Value);

            await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(5), ct);
            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in GetTodayAttendanceAsync for employee {employeeId}");
            // Return null instead of throwing - this allows the frontend to handle it gracefully
            return null!;
        }
    }

    public async Task<List<AttendanceRecordDto>> GetAttendanceByPeriodAsync(Guid employeeId, DateTime start, DateTime end, CancellationToken ct = default)
    {
        try
        {
            // Convert to Ethiopia time for date filtering
            var ethiopiaStart = start.Add(EthiopiaOffset).Date;
            var ethiopiaEnd = end.Add(EthiopiaOffset).Date;

            var cacheKey = $"attendance_period_{employeeId}_{ethiopiaStart:yyyyMMdd}_{ethiopiaEnd:yyyyMMdd}";
            var cached = await _cache.GetAsync<List<AttendanceRecordDto>>(cacheKey, ct);
            if (cached != null)
                return cached;

            var records = await _context.AttendanceRecords
                .Include(x => x.Shift)
                .Where(x => x.EmployeeId == employeeId &&
                            x.Date >= ethiopiaStart &&
                            x.Date <= ethiopiaEnd &&
                            !x.IsDeleted)
                .OrderBy(x => x.Date)
                .ToListAsync(ct);

            var dtos = records.Select(MapToDto).ToList();

            // Convert times to Ethiopia time for display
            foreach (var dto in dtos)
            {
                if (dto.CheckIn.HasValue)
                    dto.CheckIn = ToEthiopiaTime(dto.CheckIn.Value);
                if (dto.CheckOut.HasValue)
                    dto.CheckOut = ToEthiopiaTime(dto.CheckOut.Value);
            }

            await _cache.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(15), ct);
            return dtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in GetAttendanceByPeriodAsync for employee {employeeId}");
            throw;
        }
    }

   public async Task<List<AttendanceRecordDto>> GetAttendanceByDateAsync(DateTime date, CancellationToken ct = default)
   {
       try
       {
           // ✅ Ensure UTC for database query
           DateTime queryDate;
           if (date.Kind == DateTimeKind.Unspecified)
           {
               queryDate = DateTime.SpecifyKind(date, DateTimeKind.Utc).Date;
           }
           else if (date.Kind == DateTimeKind.Local)
           {
               queryDate = date.ToUniversalTime().Date;
           }
           else
           {
               queryDate = date.Date;
           }

           var cacheKey = $"attendance_date_{queryDate:yyyyMMdd}";
           var cached = await _cache.GetAsync<List<AttendanceRecordDto>>(cacheKey, ct);
           if (cached != null)
               return cached;

           var records = await _context.AttendanceRecords
               .Include(x => x.Shift)
               .Where(x => x.Date == queryDate && !x.IsDeleted)
               .ToListAsync(ct);

           var dtos = records.Select(MapToDto).ToList();

           // Convert times to Ethiopia time for display
           foreach (var dto in dtos)
           {
               if (dto.CheckIn.HasValue)
                   dto.CheckIn = ToEthiopiaTime(dto.CheckIn.Value);
               if (dto.CheckOut.HasValue)
                   dto.CheckOut = ToEthiopiaTime(dto.CheckOut.Value);
           }

           await _cache.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(5), ct);
           return dtos;
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, $"Error in GetAttendanceByDateAsync for date {date:yyyy-MM-dd}");
           return new List<AttendanceRecordDto>();
       }
   }

    public async Task<AttendanceSummaryDto> GetAttendanceSummaryAsync(Guid employeeId, DateTime? from = null, DateTime? to = null, CancellationToken ct = default)
    {
        try
        {
            var cacheKey = $"attendance_employee_{employeeId}_summary";
            var cached = await _cache.GetAsync<AttendanceSummaryDto>(cacheKey, ct);
            if (cached != null)
                return cached;

            // Use Ethiopia time for date range
            DateTime ethiopiaNow;
            try
            {
                ethiopiaNow = GetEthiopiaNow();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting Ethiopia time for employee {employeeId}");
                ethiopiaNow = DateTime.UtcNow.AddHours(3); // Fallback to UTC+3
            }

            var end = to.HasValue ? to.Value.Add(EthiopiaOffset).Date : ethiopiaNow.Date;
            var start = from.HasValue ? from.Value.Add(EthiopiaOffset).Date : end.AddMonths(-1);

            if (start > end)
            {
                var temp = start;
                start = end;
                end = temp;
            }

            var records = await _context.AttendanceRecords
                .Where(x => x.EmployeeId == employeeId &&
                            x.Date >= start &&
                            x.Date <= end &&
                            !x.IsDeleted)
                .ToListAsync(ct);

            var summary = new AttendanceSummaryDto
            {
                EmployeeId = employeeId,
                EmployeeName = "",
                PeriodStart = start,
                PeriodEnd = end,
                TotalDays = (end - start).Days + 1,
                PresentDays = records.Count(x => x.Status == AttendanceStatus.Present.ToString()),
                AbsentDays = records.Count(x => x.Status == AttendanceStatus.Absent.ToString()),
                LateDays = records.Count(x => x.Status == AttendanceStatus.Late.ToString()),
                LeaveDays = records.Count(x => x.Status == AttendanceStatus.Leave.ToString()),
                HolidayDays = records.Count(x => x.Status == AttendanceStatus.Holiday.ToString()),
                WeekendDays = records.Count(x => x.Status == AttendanceStatus.Weekend.ToString()),
                TotalHoursWorked = records.Sum(x => x.HoursWorked),
                TotalOvertimeHours = records.Sum(x => x.OvertimeHours)
            };

            summary.AverageHoursPerDay = summary.PresentDays > 0
                ? summary.TotalHoursWorked / summary.PresentDays
                : 0;

            summary.AttendanceRate = summary.TotalDays > 0
                ? (double)summary.PresentDays / summary.TotalDays * 100
                : 0;

            await _cache.SetAsync(cacheKey, summary, TimeSpan.FromMinutes(15), ct);
            return summary;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in GetAttendanceSummaryAsync for employee {employeeId}");
            // Return a default summary instead of throwing
            return new AttendanceSummaryDto
            {
                EmployeeId = employeeId,
                EmployeeName = "",
                PeriodStart = DateTime.UtcNow.AddHours(3).Date.AddMonths(-1),
                PeriodEnd = DateTime.UtcNow.AddHours(3).Date,
                TotalDays = 30,
                PresentDays = 0,
                AbsentDays = 0,
                LateDays = 0,
                LeaveDays = 0,
                HolidayDays = 0,
                WeekendDays = 0,
                TotalHoursWorked = 0,
                TotalOvertimeHours = 0,
                AverageHoursPerDay = 0,
                AttendanceRate = 0
            };
        }
    }

    #endregion

    #region Update

    public async Task<AttendanceRecordDto> UpdateAttendanceAsync(Guid id, UpdateAttendanceDto dto, CancellationToken ct = default)
    {
        try
        {
            var record = await _context.AttendanceRecords
                .Include(x => x.Shift)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);

            if (record == null)
                throw new KeyNotFoundException($"Attendance record {id} not found");

            if (dto.CheckIn.HasValue)
                record.CheckIn = EnsureUtc(dto.CheckIn.Value);

            if (dto.CheckOut.HasValue)
                record.CheckOut = EnsureUtc(dto.CheckOut.Value);

            if (!string.IsNullOrEmpty(dto.Status))
                record.Status = dto.Status;

            if (!string.IsNullOrEmpty(dto.Notes))
                record.Notes = dto.Notes;

            record.DateMod = DateTime.UtcNow;

            // Recalculate hours
            if (record.CheckIn.HasValue && record.CheckOut.HasValue)
            {
                record.HoursWorked = _calculator.CalculateHoursWorked(record.CheckIn.Value, record.CheckOut.Value, record.Shift);
                record.OvertimeHours = _calculator.CalculateOvertimeHours(record.CheckIn.Value, record.CheckOut.Value, record.Shift);
            }

            await _context.SaveChangesAsync(ct);

            // Clear cache
            await _cache.RemoveAsync($"attendance_today_{record.EmployeeId}", ct);
            await _cache.RemoveAsync($"attendance_{id}", ct);
            await _cache.RemoveAsync($"attendance_employee_{record.EmployeeId}_summary", ct);

            var dtoResult = MapToDto(record);

            // Convert times to Ethiopia time for display
            if (dtoResult.CheckIn.HasValue)
                dtoResult.CheckIn = ToEthiopiaTime(dtoResult.CheckIn.Value);
            if (dtoResult.CheckOut.HasValue)
                dtoResult.CheckOut = ToEthiopiaTime(dtoResult.CheckOut.Value);

            return dtoResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in UpdateAttendanceAsync for id {id}");
            throw;
        }
    }

    #endregion

    #region Reports

#region Reports
public async Task<AttendanceReportDto> GetDailyReportAsync(DateTime date, CancellationToken ct = default)
{
    try
    {
        _logger.LogInformation($"GetDailyReportAsync called with date: {date:yyyy-MM-dd} (Kind: {date.Kind})");

        // ✅ Get the date in UTC for database query
        // The date coming from the client is already in UTC format (YYYY-MM-DD)
        // We need to ensure it's treated as UTC for the database
        DateTime queryDate;

        if (date.Kind == DateTimeKind.Unspecified)
        {
            // If unspecified, treat as UTC
            queryDate = DateTime.SpecifyKind(date, DateTimeKind.Utc).Date;
        }
        else if (date.Kind == DateTimeKind.Local)
        {
            queryDate = date.ToUniversalTime().Date;
        }
        else
        {
            queryDate = date.Date;
        }

        _logger.LogInformation($"Query date (UTC): {queryDate:yyyy-MM-dd}");

        // ✅ Use the UTC date for database query
        var cacheKey = $"attendance_report_daily_{queryDate:yyyyMMdd}";
        var cached = await _cache.GetAsync<AttendanceReportDto>(cacheKey, ct);
        if (cached != null)
            return cached;

        var records = await _context.AttendanceRecords
            .Where(x => x.Date == queryDate && !x.IsDeleted)
            .ToListAsync(ct);

        // ✅ Convert to Ethiopia time for display
        var ethiopiaDate = queryDate.Add(EthiopiaOffset);
        _logger.LogInformation($"Found {records.Count} records for Ethiopia date: {ethiopiaDate:yyyy-MM-dd}");

        var report = new AttendanceReportDto
        {
            ReportDate = ethiopiaDate,  // Display in Ethiopia time
            ReportType = "Daily",
            TotalEmployees = records.Count,
            PresentCount = records.Count(x => x.Status == AttendanceStatus.Present.ToString()),
            AbsentCount = records.Count(x => x.Status == AttendanceStatus.Absent.ToString()),
            LateCount = records.Count(x => x.Status == AttendanceStatus.Late.ToString()),
            LeaveCount = records.Count(x => x.Status == AttendanceStatus.Leave.ToString()),
            HolidayCount = records.Count(x => x.Status == AttendanceStatus.Holiday.ToString()),
            AttendanceRate = records.Count > 0
                ? (double)records.Count(x => x.Status == AttendanceStatus.Present.ToString()) / records.Count * 100
                : 0,
            Records = records.Select(MapToDto).ToList()
        };

        // Convert times to Ethiopia time for display
        foreach (var dto in report.Records)
        {
            if (dto.CheckIn.HasValue)
                dto.CheckIn = ToEthiopiaTime(dto.CheckIn.Value);
            if (dto.CheckOut.HasValue)
                dto.CheckOut = ToEthiopiaTime(dto.CheckOut.Value);
        }

        await _cache.SetAsync(cacheKey, report, TimeSpan.FromMinutes(15), ct);
        return report;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, $"Error in GetDailyReportAsync for date {date:yyyy-MM-dd}");
        return new AttendanceReportDto
        {
            ReportDate = date,
            ReportType = "Daily",
            TotalEmployees = 0,
            PresentCount = 0,
            AbsentCount = 0,
            LateCount = 0,
            LeaveCount = 0,
            HolidayCount = 0,
            AttendanceRate = 0,
            Records = new List<AttendanceRecordDto>()
        };
    }
}
public async Task<AttendanceReportDto> GetMonthlyReportAsync(int year, int month, CancellationToken ct = default)
{
    try
    {
        _logger.LogInformation($"GetMonthlyReportAsync called for {year}-{month}");

        // ✅ Create UTC dates for database query
        var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        _logger.LogInformation($"Query range (UTC): {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");

        var cacheKey = $"attendance_report_monthly_{year}_{month}";
        var cached = await _cache.GetAsync<AttendanceReportDto>(cacheKey, ct);
        if (cached != null)
            return cached;

        var records = await _context.AttendanceRecords
            .Where(x => x.Date >= startDate.Date && x.Date <= endDate.Date && !x.IsDeleted)
            .ToListAsync(ct);

        // ✅ Convert to Ethiopia time for display
        var ethiopiaStart = startDate.Add(EthiopiaOffset);
        var ethiopiaEnd = endDate.Add(EthiopiaOffset);
        _logger.LogInformation($"Found {records.Count} records for Ethiopia range: {ethiopiaStart:yyyy-MM-dd} to {ethiopiaEnd:yyyy-MM-dd}");

        var report = new AttendanceReportDto
        {
            ReportDate = ethiopiaStart,
            ReportType = "Monthly",
            TotalEmployees = records.Select(x => x.EmployeeId).Distinct().Count(),
            PresentCount = records.Count(x => x.Status == AttendanceStatus.Present.ToString()),
            AbsentCount = records.Count(x => x.Status == AttendanceStatus.Absent.ToString()),
            LateCount = records.Count(x => x.Status == AttendanceStatus.Late.ToString()),
            LeaveCount = records.Count(x => x.Status == AttendanceStatus.Leave.ToString()),
            HolidayCount = records.Count(x => x.Status == AttendanceStatus.Holiday.ToString()),
            AttendanceRate = records.Count > 0
                ? (double)records.Count(x => x.Status == AttendanceStatus.Present.ToString()) / records.Count * 100
                : 0,
            Records = records.Select(MapToDto).ToList()
        };

        // Convert times to Ethiopia time for display
        foreach (var dto in report.Records)
        {
            if (dto.CheckIn.HasValue)
                dto.CheckIn = ToEthiopiaTime(dto.CheckIn.Value);
            if (dto.CheckOut.HasValue)
                dto.CheckOut = ToEthiopiaTime(dto.CheckOut.Value);
        }

        await _cache.SetAsync(cacheKey, report, TimeSpan.FromMinutes(15), ct);
        return report;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, $"Error in GetMonthlyReportAsync for {year}-{month}");
        return new AttendanceReportDto
        {
            ReportDate = new DateTime(year, month, 1),
            ReportType = "Monthly",
            TotalEmployees = 0,
            PresentCount = 0,
            AbsentCount = 0,
            LateCount = 0,
            LeaveCount = 0,
            HolidayCount = 0,
            AttendanceRate = 0,
            Records = new List<AttendanceRecordDto>()
        };
    }
}

// Also update these methods to return empty lists instead of throwing
public async Task<List<AttendanceRecordDto>> GetLateEmployeesAsync(DateTime date, int? thresholdMinutes = 15, CancellationToken ct = default)
{
    try
    {
        var ethiopiaDate = date.Add(EthiopiaOffset).Date;

        var records = await _context.AttendanceRecords
            .Include(x => x.Shift)
            .Where(x => x.Date == ethiopiaDate &&
                        x.IsLate &&
                        x.LateMinutes >= (thresholdMinutes ?? 15) &&
                        !x.IsDeleted)
            .ToListAsync(ct);

        var dtos = records.Select(MapToDto).ToList();

        foreach (var dto in dtos)
        {
            if (dto.CheckIn.HasValue)
                dto.CheckIn = ToEthiopiaTime(dto.CheckIn.Value);
            if (dto.CheckOut.HasValue)
                dto.CheckOut = ToEthiopiaTime(dto.CheckOut.Value);
        }

        return dtos;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, $"Error in GetLateEmployeesAsync for date {date}");
        // ✅ Return empty list instead of throwing
        return new List<AttendanceRecordDto>();
    }
}

public async Task<List<AttendanceRecordDto>> GetAbsentEmployeesAsync(DateTime date, CancellationToken ct = default)
{
    try
    {
        var ethiopiaDate = date.Add(EthiopiaOffset).Date;

        var records = await _context.AttendanceRecords
            .Where(x => x.Date == ethiopiaDate &&
                        x.Status == AttendanceStatus.Absent.ToString() &&
                        !x.IsDeleted)
            .ToListAsync(ct);

        var dtos = records.Select(MapToDto).ToList();

        foreach (var dto in dtos)
        {
            if (dto.CheckIn.HasValue)
                dto.CheckIn = ToEthiopiaTime(dto.CheckIn.Value);
            if (dto.CheckOut.HasValue)
                dto.CheckOut = ToEthiopiaTime(dto.CheckOut.Value);
        }

        return dtos;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, $"Error in GetAbsentEmployeesAsync for date {date}");
        // ✅ Return empty list instead of throwing
        return new List<AttendanceRecordDto>();
    }
}

#endregion

    #endregion

    #region Batch Processing

    public async Task ProcessDailyAttendanceAsync(DateTime date, CancellationToken ct = default)
    {
        try
        {
            // Convert to Ethiopia time
            var ethiopiaDate = date.Add(EthiopiaOffset).Date;

            _logger.LogInformation($"Processing daily attendance for {ethiopiaDate:yyyy-MM-dd} (Ethiopia time)");

            var records = await _context.AttendanceRecords
                .Where(x => x.Date == ethiopiaDate && !x.IsDeleted)
                .ToListAsync(ct);

            var ethiopiaNow = GetEthiopiaNow();

            // Canonical leave source: HRM.Leave (not local Attendance leave tables)
            var approvedLeaves = await _leaveClient.GetApprovedLeavesAsync(ethiopiaDate, ethiopiaDate, null, ct);
            var onLeaveIds = approvedLeaves
                .Where(x => x.StartDate.Date <= ethiopiaDate && x.EndDate.Date >= ethiopiaDate)
                .Select(x => x.EmployeeId)
                .ToHashSet();

            foreach (var record in records)
            {
                if (record.CheckIn.HasValue && record.CheckOut.HasValue)
                {
                    continue;
                }

                if (onLeaveIds.Contains(record.EmployeeId))
                {
                    record.Status = AttendanceStatus.Leave.ToString();
                    record.Notes = string.IsNullOrWhiteSpace(record.Notes)
                        ? "Marked Leave from HRM.Leave approved request"
                        : record.Notes;
                    continue;
                }

                if (!record.CheckIn.HasValue && ethiopiaDate <= ethiopiaNow.Date)
                {
                    record.Status = AttendanceStatus.Absent.ToString();
                }
            }

            await _context.SaveChangesAsync(ct);
            _logger.LogInformation($"Processed {records.Count} attendance records");

            await _cache.RemoveAsync($"attendance_date_{ethiopiaDate:yyyyMMdd}", ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in ProcessDailyAttendanceAsync for date {date}");
            throw;
        }
    }

    #endregion

    #region Admin

    public async Task MarkAttendanceAsync(Guid employeeId, DateTime date, string status, string? notes = null, CancellationToken ct = default)
    {
        try
        {
            // Convert to Ethiopia time
            var ethiopiaDate = date.Add(EthiopiaOffset).Date;

            var record = await _context.AttendanceRecords
                .FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.Date == ethiopiaDate && !x.IsDeleted, ct);

            if (record == null)
            {
                record = new LocalAttendanceRecord
                {
                    EmployeeId = employeeId,
                    Date = ethiopiaDate,
                    Status = status,
                    Notes = notes,
                    CreatedBy = "Admin",
                    DateAdd = DateTime.UtcNow
                };
                await _context.AttendanceRecords.AddAsync(record, ct);
            }
            else
            {
                record.Status = status;
                record.Notes = notes ?? record.Notes;
                record.DateMod = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(ct);

            // Clear cache
            await _cache.RemoveAsync($"attendance_today_{employeeId}", ct);
            await _cache.RemoveAsync($"attendance_{record.Id}", ct);
            await _cache.RemoveAsync($"attendance_employee_{employeeId}_summary", ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in MarkAttendanceAsync for employee {employeeId}");
            throw;
        }
    }

    #endregion

    #region Mapping

    private static AttendanceRecordDto MapToDto(LocalAttendanceRecord record)
    {
        return new AttendanceRecordDto
        {
            Id = record.Id,
            EmployeeId = record.EmployeeId,
            EmployeeName = record.LocalEmployee?.FirstName ?? string.Empty,
            EmployeeCode = record.LocalEmployee?.Code ?? string.Empty,
            Date = record.Date,
            CheckIn = record.CheckIn,
            CheckOut = record.CheckOut,
            Status = record.Status,
            HoursWorked = record.HoursWorked,
            OvertimeHours = record.OvertimeHours,
            IsLate = record.IsLate,
            LateMinutes = record.LateMinutes,
            IsEarlyDeparture = record.IsEarlyDeparture,
            EarlyDepartureMinutes = record.EarlyDepartureMinutes,
            ShiftName = record.Shift?.Name,
            Notes = record.Notes
        };
    }

    #endregion
}