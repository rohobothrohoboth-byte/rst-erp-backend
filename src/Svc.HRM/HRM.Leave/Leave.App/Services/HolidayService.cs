using Common;
using Helpers;
using Leave.Domain.DTOs;
using Leave.Domain.Enums;

namespace Leave.App.Services;

public interface IHolidayService
{
    Task<double> CalEmpWorkingDays(Guid empId, DateTime startDate, DateTime endDate, bool exHolidays = true);
    Task<double> CalEmpLeaveWorkingDays(Guid empId, DateTime startDate, DateTime endDate, bool isHalfDay = false);
    //Task<List<NonWorkingDay>> GetNonWorkingDays(DateTime startDate, DateTime endDate);
}

public class HolidayService : IHolidayService
{
    private readonly IHrmProfileClient _hrmProfileClient;
    private readonly ICorModClient _corModClient;
    public HolidayService(IHrmProfileClient hrmProfileClient, ICorModClient corModClient)
    {
        _hrmProfileClient = hrmProfileClient;
        _corModClient = corModClient;
    }

    public async Task<double> CalEmpWorkingDays(Guid empId, DateTime startDate, DateTime endDate, bool exHolidays = true)
    {
        if (startDate > endDate) { throw new DomainException("Start date cannot be after end date"); }

        var positionReq = await _hrmProfileClient.GetPosEmp(empId.ToString()) ?? throw new DomainException("Position requirements not configured");

        HashSet<DateTime> holidayDates = [];
        if (exHolidays)
        {
            var holidays = await GetHolidaysInRange(startDate, endDate, publicOnly: true);
            holidayDates = holidays.Select(h => h.Date.Date).ToHashSet();
        }

        double workingDays = 0;
        var currentDate = startDate.Date;

        while (currentDate <= endDate.Date)
        {
            // Holiday always wins
            if (exHolidays && holidayDates.Contains(currentDate))
            {
                currentDate = currentDate.AddDays(1);
                continue;
            }

            switch (currentDate.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                    workingDays += ResolveWorkOption(positionReq.SaturdayWorkOption);
                    break;

                case DayOfWeek.Sunday:
                    workingDays += ResolveWorkOption(positionReq.SundayWorkOption);
                    break;

                default:
                    workingDays += 1;
                    break;
            }

            currentDate = currentDate.AddDays(1);
        }

        return workingDays;
    }

    public async Task<double> CalEmpLeaveWorkingDays(Guid empId, DateTime startDate, DateTime endDate, bool isHalfDay = false)
    {
        var workingDays = await CalEmpWorkingDays(empId, startDate, endDate, exHolidays: true);
        if (isHalfDay) { return 0.5; }
        return workingDays;
    }

    //public async Task<List<NonWorkingDay>> GetNonWorkingDays(DateTime startDate, DateTime endDate)
    //{
    //    var nonWorkingDays = new List<NonWorkingDay>();
    //    var holidays = await GetHolidaysInRange(startDate, endDate, publicOnly: false);

    //    var currentDate = startDate.Date;
    //    while (currentDate <= endDate.Date)
    //    {
    //        bool isWeekend = currentDate.DayOfWeek == DayOfWeek.Saturday || currentDate.DayOfWeek == DayOfWeek.Sunday;
    //        var holiday = holidays.FirstOrDefault(h => h.Date.Date == currentDate);

    //        if (isWeekend || holiday != null)
    //        {
    //            nonWorkingDays.Add(new NonWorkingDay
    //            {
    //                Date = currentDate,
    //                Type = holiday != null ? "Holiday" : "Weekend",
    //                Description = holiday?.Name ?? currentDate.DayOfWeek.ToString()
    //            });
    //        }

    //        currentDate = currentDate.AddDays(1);
    //    }

    //    return nonWorkingDays;
    //}



    private static double ResolveWorkOption(string workOption)
    {
        var wOption = ((WorkOption)Enum.Parse(typeof(WorkOption), workOption));

        return wOption switch
        {
            WorkOption.Morning => 0.5,
            WorkOption.Afternoon => 0.5,
            WorkOption.Both => 1.0,
            _ => 0.0
        };
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
}