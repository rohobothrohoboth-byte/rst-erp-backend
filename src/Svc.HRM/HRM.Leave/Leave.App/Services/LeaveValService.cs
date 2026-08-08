using Common;
using Helpers;
using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using Leave.Domain.Entities.Local;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Leave.App.Services;

public interface ILeaveValService
{
    Task<LeaveReqValResult> ValLeaveRequest(Guid empId, Guid leaveTypeId, DateTime startDate, DateTime endDate, bool isHalfDay, CancellationToken ct);
}

public class LeaveValService : ILeaveValService
{
    private readonly IUnitOfWork _uow;
    private readonly ICorModClient _corModClient;
    private readonly IHolidayService _hdService;
    private readonly ILogger<LeaveValService> _logger;

    public LeaveValService(
        IUnitOfWork uow,
        ICorModClient corModClient,
        IHolidayService hdService,
        ILogger<LeaveValService> logger)
    {
        _uow = uow;
        _corModClient = corModClient;
        _hdService = hdService;
        _logger = logger;
    }

    public async Task<LeaveReqValResult> ValLeaveRequest(Guid empId, Guid leaveTypeId, DateTime startDate, DateTime endDate, bool isHalfDay, CancellationToken ct)
    {
        var result = new LeaveReqValResult();

        var utcStartDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
        var utcEndDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);

        _logger.LogInformation("Validating leave request for EmployeeId: {EmpId}, LeaveTypeId: {LeaveTypeId}, Dates: {StartDate} - {EndDate}",
            empId, leaveTypeId, utcStartDate, utcEndDate);

        var workingDays = await _hdService.CalEmpLeaveWorkingDays(empId, utcStartDate, utcEndDate, isHalfDay);

        var empLeavePolicy = await GetActiveEmpLeavePolicy(empId, leaveTypeId, utcStartDate, ct);

        if (empLeavePolicy == null)
        {
            result.AddError("No active leave policy found for this employee and leave type.");
            return result;
        }

        await ValEntitlementBalance(empId, leaveTypeId, workingDays, empLeavePolicy, result, ct);
        await ValidateOverlap(empId, utcStartDate, utcEndDate, result, ct);
        await ValProbationEligibility(empId, empLeavePolicy, result, ct);
        await ValPolicyConstraints(empLeavePolicy, workingDays, utcStartDate, utcEndDate, isHalfDay, result, ct);

        result.CalculatedWorkingDays = workingDays;

        return result;
    }

    private async Task<EmpLeavePolicy?> GetActiveEmpLeavePolicy(Guid empId, Guid leaveTypeId, DateTime requestDate, CancellationToken ct)
    {
        var utcRequestDate = DateTime.SpecifyKind(requestDate.Date, DateTimeKind.Utc); // Normalize to date only

        try
        {
            var policy = await _uow.Set<EmpLeavePolicy>()
                .AsNoTracking()
                .Where(elp => elp.EmployeeId == empId
                    && elp.LeaveTypeId == leaveTypeId
                    && !elp.IsDeleted
                    && utcRequestDate >= elp.EffectiveFrom.Date  // Normalize dates
                    && (elp.EffectiveTo == null || utcRequestDate <= elp.EffectiveTo!.Value.Date))
                .FirstOrDefaultAsync(ct);

            if (policy != null)
            {
                _logger.LogInformation("✅ Active policy found for Employee {EmpId}, LeaveType {LeaveTypeId}. PolicyId: {PolicyId}",
                    empId, leaveTypeId, policy.Id);
            }
            else
            {
                _logger.LogWarning("❌ No active policy found. Checking raw data...");

                // Debug query - show all policies for this employee + type
                var allPolicies = await _uow.Set<EmpLeavePolicy>()
                    .AsNoTracking()
                    .Where(elp => elp.EmployeeId == empId && elp.LeaveTypeId == leaveTypeId)
                    .Select(elp => new { elp.Id, elp.EffectiveFrom, elp.EffectiveTo, elp.IsDeleted })
                    .ToListAsync(ct);

                _logger.LogWarning("Found {Count} total policies for this emp+type: {@Policies}", allPolicies.Count, allPolicies);
            }

            return policy;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetActiveEmpLeavePolicy");
            return null;
        }
    }
    private async Task ValEntitlementBalance(Guid empId, Guid leaveTypeId, double daysRequested, EmpLeavePolicy empLeavePolicy, LeaveReqValResult result, CancellationToken ct)
    {
        var approvedDays = await GetApprovedLeaveDays(empId, leaveTypeId, ct);

        var assignedEntitlement = empLeavePolicy.AssignedEntitlement;
        var assignedEntitlementDouble = (double)assignedEntitlement;
        var remainingBalance = assignedEntitlementDouble - approvedDays;

        if (daysRequested > remainingBalance)
        {
            result.AddError($"Insufficient leave balance. Requested: {daysRequested} working days, " +
                           $"Available: {remainingBalance:F2} days (Assigned: {assignedEntitlement}, " +
                           $"Used: {approvedDays})");
        }

        result.LeaveBalance = new LeaveBalanceInfo
        {
            AssignedEntitlement = assignedEntitlement,
            UsedDays = approvedDays,
            RemainingBalance = remainingBalance,
            RequestedDays = daysRequested
        };
    }

    private async Task<double> GetApprovedLeaveDays(Guid empId, Guid leaveTypeId, CancellationToken ct)
    {
        var stat = BoolToStr.EnumToString(Status.Approved);
        var fy = await _corModClient.GetActiveFiscal();
        if (fy.Id == null) { return 0; }

        var cStart = DateTime.SpecifyKind(DateTime.Parse(fy.StartDate), DateTimeKind.Utc);
        var cEnd = DateTime.SpecifyKind(DateTime.Parse(fy.EndDate), DateTimeKind.Utc);

        var totalDays = await _uow.Set<LeaveRequest>()
            .AsNoTracking()  // ✅ ADD THIS
            .Where(lr => lr.EmployeeId == empId
                && lr.LeaveTypeId == leaveTypeId
                && lr.Status == stat
                && lr.StartDate >= cStart
                && lr.EndDate <= cEnd)
            .SumAsync(lr => lr.DaysRequested, ct);

        return totalDays;
    }

    private async Task ValidateOverlap(Guid empId, DateTime startDate, DateTime endDate, LeaveReqValResult result, CancellationToken ct)
    {
        var stat = BoolToStr.EnumToString(Status.Approved);
        var stat2 = BoolToStr.EnumToString(Status.Pending);

        var overlaps = await _uow.Set<LeaveRequest>()
            .AsNoTracking()  // ✅ ADD THIS
            .Where(lr => lr.EmployeeId == empId
                && (lr.Status == stat || lr.Status == stat2)
                && ((lr.StartDate >= startDate && lr.StartDate <= endDate)
                    || (lr.EndDate >= startDate && lr.EndDate <= endDate)
                    || (lr.StartDate <= startDate && lr.EndDate >= endDate)))
            .Select(lr => new
            {
                lr.Id,
                lr.StartDate,
                lr.EndDate,
                lr.Status,
                lr.LeaveType.Name
            })
            .ToListAsync(ct);

        if (overlaps.Any())
        {
            var overlapsText = string.Join("; ", overlaps.Select(ol =>
                $"{ol.Name} from {ol.StartDate:yyyy-MM-dd} to {ol.EndDate:yyyy-MM-dd} ({ol.Status})"));
            result.AddError($"Leave request overlaps with existing leave(s): {overlapsText}");
        }
    }

    private async Task ValProbationEligibility(Guid empId, EmpLeavePolicy empLeavePolicy, LeaveReqValResult result, CancellationToken ct)
    {
        var policyConfig = await _uow.Set<LeavePolicyConfig>()
            .AsNoTracking()  // ✅ ADD THIS
            .FirstOrDefaultAsync(lpc => lpc.LeavePolicyId == empLeavePolicy.LeavePolicyId && lpc.IsActive, ct);

        if (policyConfig == null)
        {
            result.AddWarning("No active policy configuration found.");
            return;
        }

        var employee = await _uow.Set<LocalEmployee>()
            .AsNoTracking()  // ✅ ADD THIS
            .FirstOrDefaultAsync(e => e.Id == empId && !e.IsDeleted, ct);

        if (employee == null)
        {
            result.AddError("Employee not found.");
            return;
        }

        var serviceMonths = CalculateServiceMonths(employee.EmploymentDate);

        if (serviceMonths < policyConfig.MinServiceMonths)
        {
            result.AddError($"Employee does not meet minimum service requirement. " +
                           $"Required: {policyConfig.MinServiceMonths} months, " +
                           $"Current: {serviceMonths} months");
        }

        result.EligibilityInfo = new EligibilityInfo
        {
            ServiceMonths = serviceMonths,
            MinRequiredMonths = policyConfig.MinServiceMonths,
            IsEligible = serviceMonths >= policyConfig.MinServiceMonths,
            EmployeeName = $"{employee.FirstName} {employee.LastName}",
            Department = employee.Department?.Name ?? "",
            Position = employee.Position?.Name ?? ""
        };

        _logger.LogDebug("Employee {EmpId} service months: {ServiceMonths}, Required: {MinRequired}",
            empId, serviceMonths, policyConfig.MinServiceMonths);
    }

    private int CalculateServiceMonths(DateTime employmentDate)
    {
        var today = DateTime.UtcNow.Date;
        var months = ((today.Year - employmentDate.Year) * 12) + (today.Month - employmentDate.Month);

        if (today.Day < employmentDate.Day)
            months--;

        return Math.Max(0, months);
    }

    private async Task ValPolicyConstraints(EmpLeavePolicy empLvPolicy, double daysReq, DateTime startDate, DateTime endDate, bool isHalfDay, LeaveReqValResult result, CancellationToken ct)
    {
        var utcStartDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
        var utcEndDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);
        var utcToday = DateTime.UtcNow.Date;

        var lPolicyId = empLvPolicy.LeavePolicyId;

        var lPolicy = await _uow.Set<LeavePolicy>()
            .AsNoTracking()  // ✅ ADD THIS
            .FirstOrDefaultAsync(x => x.Id == lPolicyId, ct);

        var pConfig = await _uow.Set<LeavePolicyConfig>()
            .AsNoTracking()  // ✅ ADD THIS
            .FirstOrDefaultAsync(lpc => lpc.LeavePolicyId == lPolicyId && lpc.IsActive, ct);

        if (lPolicy == null)
        {
            result.AddWarning("No active Leave Policy found.");
            return;
        }

        if (lPolicy.Status != "Active")
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
            result.AddError($"Leave request exceeds maximum allowed days per request. " +
                           $"Requested: {daysReq} working days, Maximum: {pConfig.MaxDaysPerReq} days");
        }

        if (lPolicy.RequiresAttachment)
        {
            result.AddWarning("This leave type requires supporting documentation/attachment.");
        }

        if (utcStartDate > utcEndDate)
        {
            result.AddError("Start date cannot be after end date.");
        }

        if (utcStartDate < utcToday)
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
}