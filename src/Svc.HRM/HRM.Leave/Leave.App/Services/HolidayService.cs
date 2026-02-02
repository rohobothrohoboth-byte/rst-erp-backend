using Common;
using Leave.App.Interfaces;
using Leave.Domain.DTOs;

namespace Leave.App.Services;

public class HolidayService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICorModClient _corModClient;
    public HolidayService(IUnitOfWork unitOfWork, ICorModClient corModClient)
    {
        _unitOfWork = unitOfWork;
        _corModClient = corModClient;
    }

    public async Task<double> CalculateWorkingDays(DateTime startDate, DateTime endDate, bool exWeekends = true, bool exHolidays = true)
    {
        if (startDate > endDate)
        {
            throw new ArgumentException("Start date cannot be after end date");
        }

        // Get holidays in the range if needed
        List<DateTime> holidayDates = new();
        if (exHolidays)
        {
            var holidays = await GetHolidaysInRange(startDate, endDate, publicOnly: true);
            holidayDates = holidays.Select(h => h.Date.Date).ToList();
        }

        double workingDays = 0;
        var currentDate = startDate.Date;

        while (currentDate <= endDate.Date)
        {
            // Check if it's a weekend
            bool isWeekend = exWeekends && (currentDate.DayOfWeek == DayOfWeek.Saturday || currentDate.DayOfWeek == DayOfWeek.Sunday);

            // Check if it's a holiday
            bool isHoliday = exHolidays && holidayDates.Contains(currentDate);

            // Count as working day if not weekend or holiday
            if (!isWeekend && !isHoliday)
            {
                workingDays++;
            }

            currentDate = currentDate.AddDays(1);
        }

        return workingDays;
    }

    public async Task<double> CalculateLeaveWorkingDays(DateTime startDate, DateTime endDate, bool isHalfDay = false)
    {
        var workingDays = await CalculateWorkingDays(startDate, endDate, exWeekends: true, exHolidays: true);
        if (isHalfDay) { return 0.5; }
        return workingDays;
    }

    public async Task<DateTime> GetNextWorkingDay(DateTime date)
    {
        var nextDate = date.AddDays(1);

        while (true)
        {
            bool isWeekend = nextDate.DayOfWeek == DayOfWeek.Saturday || nextDate.DayOfWeek == DayOfWeek.Sunday;
            bool isHoliday = await IsHoliday(nextDate, publicOnly: true);
            if (!isWeekend && !isHoliday) { return nextDate; }
            nextDate = nextDate.AddDays(1);
        }
    }

    public async Task<List<NonWorkingDay>> GetNonWorkingDays(DateTime startDate, DateTime endDate)
    {
        var nonWorkingDays = new List<NonWorkingDay>();
        var holidays = await GetHolidaysInRange(startDate, endDate, publicOnly: false);

        var currentDate = startDate.Date;
        while (currentDate <= endDate.Date)
        {
            bool isWeekend = currentDate.DayOfWeek == DayOfWeek.Saturday || currentDate.DayOfWeek == DayOfWeek.Sunday;
            var holiday = holidays.FirstOrDefault(h => h.Date.Date == currentDate);

            if (isWeekend || holiday != null)
            {
                nonWorkingDays.Add(new NonWorkingDay
                {
                    Date = currentDate,
                    Type = holiday != null ? "Holiday" : "Weekend",
                    Description = holiday?.Name ?? currentDate.DayOfWeek.ToString()
                });
            }

            currentDate = currentDate.AddDays(1);
        }

        return nonWorkingDays;
    }

    public async Task<double> ConvertCalendarDaysToWorkingDays(DateTime startDate, int calendarDays)
    {
        var endDate = startDate.AddDays(calendarDays - 1);
        return await CalculateWorkingDays(startDate, endDate);
    }

    public async Task<DateTime> GetEndDateForWorkingDays(DateTime startDate, double workingDaysRequired)
    {
        var currentDate = startDate.Date;
        double workingDaysCount = 0;

        var holidays = await GetHolidaysInRange(startDate, startDate.AddDays(365), publicOnly: true);
        var holidayDates = holidays.Select(h => h.Date.Date).ToList();

        while (workingDaysCount < workingDaysRequired)
        {
            bool isWeekend = currentDate.DayOfWeek == DayOfWeek.Saturday || currentDate.DayOfWeek == DayOfWeek.Sunday;
            bool isHoliday = holidayDates.Contains(currentDate);

            if (!isWeekend && !isHoliday)
            {
                workingDaysCount++;
            }

            if (workingDaysCount < workingDaysRequired)
            {
                currentDate = currentDate.AddDays(1);
            }
        }

        return currentDate;
    }

    public async Task<HolidayStatistics> GetHolidayStatisticsAsync(Guid fYearId)
    {
        var fYear = await _corModClient.GetFiscYearDesc(fYearId.ToString());
        if (fYear.Id == null)
        {
            throw new InvalidOperationException("Fiscal year not found");
        }

        var holidays = await HolidaysByFiscalYear(fYearId);

        var stats = new HolidayStatistics
        {
            FiscalYearId = fYearId,
            FiscalYearName = fYear.Name,
            TotalHolidays = holidays.Count,
            PublicHolidays = holidays.Count(h => h.IsPublic),
            HolidaysByMonth = holidays.GroupBy(h => h.Date.Month).ToDictionary(g => g.Key, g => g.Count()),
            HolidaysByDayOfWeek = holidays.GroupBy(h => h.Date.DayOfWeek).ToDictionary(g => g.Key.ToString(), g => g.Count())
        };

        stats.TotalWorkingDays = await CalculateWorkingDays(DateTime.Parse(fYear.StartDate), DateTime.Parse(fYear.EndDate));

        return stats;
    }





    private async Task<List<HolidaySerListDto>> GetHolidaysInRange(DateTime startDate, DateTime endDate, bool publicOnly)
    {
        var hDayL = new List<HolidaySerListDto>();
        var allHd = await _corModClient.GetListHoDay();
        if (allHd.Res.Count <= 0)
        {
            return hDayL;
        }

        var data = allHd.Res.ToList();
        if (publicOnly)
        {
            data = [.. data.Where(h => h.IsPublic)];
        }
        if (data.Count <= 0) { return hDayL; }

        foreach (var hd in data)
        {
            var c = new HolidaySerListDto
            {
                Id = Guid.Parse(hd.Id),
                Name = hd.Name,
                Date = DateTime.Parse(hd.Date),
                IsPublic = hd.IsPublic,
                FiscalYearId = Guid.Parse(hd.FiscalYearId)
            };
            hDayL.Add(c);
        }

        return [.. hDayL.Where(h => h.Date >= startDate && h.Date <= endDate).OrderBy(h => h.Date)];
    }

    private async Task<List<HolidaySerListDto>> HolidaysByFiscalYear(Guid id)
    {
        var hDayL = new List<HolidaySerListDto>();
        var allHd = await _corModClient.GetListHoDay();
        if (allHd.Res.Count <= 0) { return hDayL; }

        var data = allHd.Res.ToList();
        foreach (var hd in data)
        {
            var c = new HolidaySerListDto
            {
                Id = Guid.Parse(hd.Id),
                Name = hd.Name,
                Date = DateTime.Parse(hd.Date),
                IsPublic = hd.IsPublic,
                FiscalYearId = Guid.Parse(hd.FiscalYearId)
            };
            hDayL.Add(c);
        }

        return [.. hDayL.Where(h => h.FiscalYearId == id).OrderBy(h => h.Date)];
    }

    public async Task<bool> IsHoliday(DateTime date, bool publicOnly)
    {
        var allHd = await _corModClient.GetListHoDay();
        if (allHd.Res.Count <= 0) { return false; }

        var hDayL = new List<HolidaySerListDto>();
        var data = allHd.Res.ToList();
        if (publicOnly) { data = [.. data.Where(h => h.IsPublic)]; }
        if (data.Count <= 0) { return false; }

        var hd = data.Where(h => h.Date == date.ToString()).ToList();
        return hd.Count != 0;
    }

    public async Task<HolidaySerListDto?> GetHolidayByDate(DateTime date)
    {
        var allHd = await _corModClient.GetListHoDay();
        if (allHd.Res.Count <= 0) { return null; }
        var data = allHd.Res.ToList();
        var hd = data.FirstOrDefault(h => h.Date == date.ToString());
        if (hd == null) { return null; }

        var c = new HolidaySerListDto
        {
            Id = Guid.Parse(hd.Id),
            Name = hd.Name,
            Date = DateTime.Parse(hd.Date),
            IsPublic = hd.IsPublic,
            FiscalYearId = Guid.Parse(hd.FiscalYearId)
        };

        return c;
    }
}