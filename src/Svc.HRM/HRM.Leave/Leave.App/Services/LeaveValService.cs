using Common;
using Helpers;
using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;

namespace Leave.App.Services;

public interface ILeaveValService
{
    Task<LeaveReqValResult> ValLeaveRequest(Guid empId, Guid leaveTypeId, DateTime startDate, DateTime endDate, bool isHalfDay);
}

public class LeaveValService : ILeaveValService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICorModClient _corModClient;
    private readonly IHrmProfileClient _hrmProfileClient;
    private readonly IHolidayService _hdService;

    public LeaveValService(IUnitOfWork unitOfWork, ICorModClient corModClient, IHrmProfileClient hrmProfileClient, IHolidayService hdService)
    {
        _unitOfWork = unitOfWork;
        _corModClient = corModClient;
        _hrmProfileClient = hrmProfileClient;
        _hdService = hdService;
    }

    public async Task<LeaveReqValResult> ValLeaveRequest(Guid empId, Guid leaveTypeId, DateTime startDate, DateTime endDate, bool isHalfDay)
    {
        var result = new LeaveReqValResult();
        var workingDays = await _hdService.CalEmpLeaveWorkingDays(empId, startDate, endDate, isHalfDay);
        var empLeavePolicy = await GetActiveEmpLeavePolicy(empId, leaveTypeId, startDate);

        if (empLeavePolicy == null)
        {
            result.AddError("NO ACTIVE LEAVE POLICY FOUND for this employee and leave type.");
            return result;
        }

        await ValEntitlementBalance(empId, leaveTypeId, workingDays, empLeavePolicy, result);
        await ValidateOverlap(empId, startDate, endDate, result);
        await ValProbationEligibility(empId, empLeavePolicy, result);
        await ValPolicyConstraints(empLeavePolicy, workingDays, startDate, endDate, isHalfDay, result);
        //await ProvideHolidayInfo(startDate, endDate, result);
        result.CalculatedWorkingDays = workingDays;

        return result;
    }

    private async Task<EmpLeavePolicy?> GetActiveEmpLeavePolicy(Guid empId, Guid leaveTypeId, DateTime requestDate)
    {
        //var ePolicy = await _unitOfWork.Repository<EmpLeavePolicy>().GetFoD(elp => elp.EmployeeId == empId && elp.LeaveTypeId == leaveTypeId && elp.IsActive(requestDate));
        var ePolicyL = (await _unitOfWork.Repository<EmpLeavePolicy>().Find(elp => elp.EmployeeId == empId && elp.LeaveTypeId == leaveTypeId && elp.EffectiveFrom <= requestDate)).ToList();
        if (ePolicyL.Count <= 0) { return null; }
        var ePolicy = ePolicyL.FirstOrDefault(e => e.IsActive(requestDate));
        return ePolicy;
    }

    private async Task ValEntitlementBalance(Guid empId, Guid leaveTypeId, double daysRequested, EmpLeavePolicy empLeavePolicy, LeaveReqValResult result)
    {
        // Get total days already approved for this leave type in current period
        var approvedDays = await GetAppLeaveDays(empId, leaveTypeId);

        // Calculate remaining balance
        var assignedEntitlement = empLeavePolicy.AssignedEntitlement;
        var remainingBalance = assignedEntitlement - approvedDays;

        if (daysRequested > remainingBalance)
        {
            result.AddError($"Insufficient leave balance. Requested: {daysRequested} working days, " + $"Available: {remainingBalance} days (Assigned: {assignedEntitlement}, " + $"Used: {approvedDays})");
        }

        // Store balance information for reference
        result.LeaveBalance = new LeaveBalanceInfo
        {
            AssignedEntitlement = assignedEntitlement,
            UsedDays = approvedDays,
            RemainingBalance = remainingBalance,
            RequestedDays = daysRequested
        };
    }

    private async Task<double> GetAppLeaveDays(Guid empId, Guid leaveTypeId)
    {
        var stat = BoolToStr.EnumToString(Status.Approved);
        var fy = await _corModClient.GetActiveFiscal();
        if (fy.Id == null) { return 0; }
        var cStart = DateTime.Parse(fy.StartDate);
        var cEnd = DateTime.Parse(fy.EndDate);

        var d = (await _unitOfWork.Repository<LeaveRequest>().Find(lr => lr.EmployeeId == empId && lr.LeaveTypeId == leaveTypeId && lr.Status == stat && lr.StartDate >= cStart && lr.EndDate <= cEnd)).ToList().Sum(lr => lr.DaysRequested);

        return d;
    }

    private async Task ValidateOverlap(Guid empId, DateTime startDate, DateTime endDate, LeaveReqValResult result)
    {
        var stat = BoolToStr.EnumToString(Status.Approved);
        var stat2 = BoolToStr.EnumToString(Status.Pending);
        var lvLedger = (await _unitOfWork.Repository<LeaveRequest>().Find(lr => lr.EmployeeId == empId && (lr.Status == stat || lr.Status == stat2)
                && ((lr.StartDate >= startDate && lr.StartDate <= endDate) || (lr.EndDate >= startDate && lr.EndDate <= endDate) || (lr.StartDate <= startDate && lr.EndDate >= endDate)))).Select(lr => new
                {
                    lr.Id,
                    lr.StartDate,
                    lr.EndDate,
                    lr.Status,
                    lr.LeaveType.Name
                }).ToList();

        if (lvLedger.Count != 0)
        {
            var overlaps = string.Join("; ", lvLedger.Select(ol => $"{ol.Name} from {ol.StartDate:yyyy-MM-dd} to {ol.EndDate:yyyy-MM-dd} ({ol.Status})"));
            result.AddError($"Leave request overlaps with existing leave(s): {overlaps}");
        }
    }

    private async Task ValProbationEligibility(Guid empId, EmpLeavePolicy empLeavePolicy, LeaveReqValResult result)
    {
        var policyConfig = await _unitOfWork.Repository<LeavePolicyConfig>().GetFoD(lpc => lpc.LeavePolicyId == empLeavePolicy.LeavePolicyId && lpc.IsActive);
        if (policyConfig == null)
        {
            result.AddWarning("No active policy configuration found.");
            return;
        }

        var employee = await _hrmProfileClient.GetEmpPolicy(empId.ToString());

        if (employee.SerYear == null)
        {
            result.AddError("Employee not found.");
            return;
        }

        var serviceDuration = double.Parse(employee.SerYear);
        if (serviceDuration < policyConfig.MinServiceMonths)
        {
            result.AddError($"Employee does not meet minimum service requirement. " + $"Required: {policyConfig.MinServiceMonths} months, " + $"Current: {Math.Floor(serviceDuration)} months");
        }

        result.EligibilityInfo = new EligibilityInfo
        {
            //JoiningDate = joiningDate,
            ServiceMonths = (int)Math.Floor(serviceDuration),
            MinRequiredMonths = policyConfig.MinServiceMonths,
            IsEligible = serviceDuration >= policyConfig.MinServiceMonths
        };
    }

    private async Task ValPolicyConstraints(EmpLeavePolicy empLvPolicy, double daysReq, DateTime startDate, DateTime endDate, bool isHalfDay, LeaveReqValResult result)
    {
        var lPolicyId = empLvPolicy.LeavePolicyId;
        var lPolicy = await _unitOfWork.Repository<LeavePolicy>().GetById(lPolicyId);
        var pConfig = await _unitOfWork.Repository<LeavePolicyConfig>().GetFoD(lpc => lpc.LeavePolicyId == lPolicyId && lpc.IsActive);

        if (lPolicy == null)
        {
            result.AddWarning("No active Leave Policy found.");
            return;
        }

        var stat = BoolToStr.EnumToString(PolicyStatus.Active);
        if (lPolicy.Status != stat)
        {
            result.AddError($"Leave policy is not active. Current status: {lPolicy.Status}");
            return;
        }

        if (pConfig == null)
        {
            result.AddWarning("No active policy configuration found.");
            return;
        }

        if (daysReq > pConfig.MaxDaysPerReq)
        {
            result.AddError($"Leave request exceeds maximum allowed days per request. " + $"Requested: {daysReq} working days, Maximum: {pConfig.MaxDaysPerReq} days");
        }

        if (lPolicy.RequiresAttachment)
        {
            result.AddWarning("This leave type requires supporting documentation/attachment.");
        }

        if (startDate > endDate)
        {
            result.AddError("Start date cannot be after end date.");
        }

        if (startDate < DateTime.Today)
        {
            result.AddError("Cannot request leave for past dates.");
        }

        if (isHalfDay && daysReq != 0.5)
        {
            result.AddError("Half-day leave must be 0.5 days.");
        }

        result.PolicyConstraints = new PolicyConstraintsInfo
        {
            MaxDaysPerRequest = pConfig.MaxDaysPerReq,
            RequiresAttachment = lPolicy.RequiresAttachment
        };
    }

    //private async Task ProvideHolidayInfo(DateTime startDate, DateTime endDate, LeaveReqValResult result)
    //{
    //    var nonWorkingDays = await _hdService.GetNonWorkingDays(startDate, endDate);

    //    if (nonWorkingDays.Any())
    //    {
    //        var holidays = nonWorkingDays.Where(nwd => nwd.Type == "Holiday").ToList();
    //        var weekends = nonWorkingDays.Where(nwd => nwd.Type == "Weekend").ToList();

    //        result.HolidayInfo = new HolidayInfo
    //        {
    //            TotalNonWorkingDays = nonWorkingDays.Count,
    //            HolidaysCount = holidays.Count,
    //            WeekendsCount = weekends.Count,
    //            HolidayDates = holidays.Select(h => new HolidayDate
    //            {
    //                Date = h.Date,
    //                Name = h.Description
    //            }).ToList()
    //        };

    //        if (holidays.Any())
    //        {
    //            var holidayNames = string.Join(", ", holidays.Select(h => $"{h.Description} ({h.Date:MMM dd})"));
    //            result.AddWarning($"Note: Your leave period includes {holidays.Count} holiday(s): {holidayNames}. " + $"These are not counted against your leave balance.");
    //        }
    //    }
    //}
}
