using Common;
using Helpers;
using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using Leave.Domain.Entities.Local;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Leave.App.Services;

public interface IHolidayService
{
    Task<double> CalEmpWorkingDays(Guid empId, DateTime startDate, DateTime endDate, bool exHolidays = true);
    Task<double> CalEmpLeaveWorkingDays(Guid empId, DateTime startDate, DateTime endDate, bool isHalfDay = false);
}

public class HolidayService : IHolidayService
{
    private readonly IUnitOfWork _uow;
    private readonly ICorModClient _corModClient;
    private readonly ILogger<HolidayService> _logger;

    public HolidayService(IUnitOfWork uow, ICorModClient corModClient, ILogger<HolidayService> logger)
    {
        _uow = uow;
        _corModClient = corModClient;
        _logger = logger;
    }

    public async Task<double> CalEmpWorkingDays(Guid empId, DateTime startDate, DateTime endDate, bool exHolidays = true)
    {
        if (startDate > endDate)
            throw new DomainException("Start date cannot be after end date");

        double saturdayWork = 0;
        double sundayWork = 0;

        try
        {
            // ✅ FIX: Use projection instead of Include to avoid split query
            var employeeData = await _uow.Set<LocalEmployee>()
                .AsNoTracking()
                .Where(e => e.Id == empId && !e.IsDeleted)
                .Select(e => new
                {
                    e.Id,
                    PositionId = e.Position != null ? e.Position.Id : (Guid?)null
                })
                .FirstOrDefaultAsync();

            if (employeeData?.PositionId != null)
            {
                var positionReq = await _uow.Set<LocalPositionReq>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(pr => pr.PositionId == employeeData.PositionId && !pr.IsDeleted);

                if (positionReq != null)
                {
                    saturdayWork = ResolveWorkOption(positionReq.SaturdayWorkOption);
                    sundayWork = ResolveWorkOption(positionReq.SundayWorkOption);
                    _logger.LogDebug("Employee {EmpId} Saturday work: {Saturday}, Sunday work: {Sunday}",
                        empId, saturdayWork, sundayWork);
                }
                else
                {
                    _logger.LogWarning("Position requirements not found for PositionId: {PositionId}, using defaults",
                        employeeData.PositionId);
                }
            }
            else
            {
                _logger.LogWarning("Position not found for employee {EmpId}, using defaults (no weekend work)", empId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not get position requirements for employee {EmpId}, using defaults", empId);
        }

        HashSet<DateTime> holidayDates = [];



// In CalEmpWorkingDays - you can make holiday fetching more robust
if (exHolidays)
{
    try
    {
        var holidays = await GetHolidaysInRange(startDate, endDate, publicOnly: true);
        holidayDates = holidays.Select(h => h.Date.Date).ToHashSet();
    }
    catch (Exception ex)
    {
        _logger.LogWarning(ex, "Could not fetch holidays, proceeding without excluding holidays");
        // Do NOT rethrow unless you want to fail the whole request
    }
}

        double workingDays = 0;
        var currentDate = startDate.Date;

        while (currentDate <= endDate.Date)
        {
            if (exHolidays && holidayDates.Contains(currentDate))
            {
                currentDate = currentDate.AddDays(1);
                continue;
            }

            switch (currentDate.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                    workingDays += saturdayWork;
                    break;
                case DayOfWeek.Sunday:
                    workingDays += sundayWork;
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
        if (isHalfDay)
            return 0.5;

        if (workingDays > 0 && workingDays < 0.5)
            return 0.5;

        return workingDays;
    }

    private static double ResolveWorkOption(string workOption)
    {
        if (string.IsNullOrEmpty(workOption))
            return 0;

        try
        {
            var wOption = (WorkOption)Enum.Parse(typeof(WorkOption), workOption, true);
            return wOption switch
            {
                WorkOption.Morning => 0.5,
                WorkOption.Afternoon => 0.5,
                WorkOption.Both => 1.0,
                WorkOption.None => 0.0,
                _ => 0.0
            };
        }
        catch
        {
            return 0;
        }
    }

    private async Task<List<HolidaySerListDto>> GetHolidaysInRange(DateTime startDate, DateTime endDate, bool publicOnly)
    {
        var hDayL = new List<HolidaySerListDto>();

        try
        {
            var allHd = await _corModClient.GetListHoDay();
            if (allHd.Res == null || allHd.Res.Count <= 0)
            {
                return hDayL;
            }

            var data = allHd.Res.ToList();
            if (publicOnly)
            {
                data = data.Where(h => h.IsPublic).ToList();
            }
            if (data.Count <= 0)
                return hDayL;

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

            return hDayL.Where(h => h.Date >= startDate && h.Date <= endDate).OrderBy(h => h.Date).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not fetch holidays");
            return [];
        }
    }
}