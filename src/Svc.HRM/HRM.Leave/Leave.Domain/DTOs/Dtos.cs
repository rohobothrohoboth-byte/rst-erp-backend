using System.Text.Json.Serialization;

namespace Leave.Domain.DTOs;

public class StatChangeDto
{
    public Guid Id { get; set; }
    public bool Stat { get; set; } = true;
    public string RowVersion { get; set; } = default!;
}

public class NameList
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
}

public class LedgerEntryDto
{
    public double Amount { get; set; } = default!; // +/-
    public string EntryType { get; set; } = default!; // enum.LedgerEntryType (0/1)
    public string SourceType { get; set; } = default!; // enum.LedgerSource (0/1)
    public Guid EmployeeId { get; set; } // HRM.Profile.Employee
    public Guid LeaveTypeId { get; set; } // LeaveType
    public Guid? LeavePolicyId { get; set; } // LeavePolicy
    public Guid? ReferenceId { get; set; } // LeaveRequestId, AccrualBatchId, etc.
}

public class EmpPolicyCtx
{
    public Guid EmployeeId { get; set; }
    public string Name { get; set; } = default!;
    public double SerYear { get; set; } = default!;
    public string EmpType { get; set; } = default!;
    public string WorkAr { get; set; } = default!;
    public string Gender { get; set; } = default!;
    public string Jg { get; set; } = default!;
}

public class PolicyCondCtx
{
    public Guid PolicyId { get; set; }
    public Guid PolAssignRuleId { get; set; }
    public Guid PolRuleCondId { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public string Priority { get; set; } = default!;
    public string Field { get; set; } = default!; // enum.ConditionField
    public string Operator { get; set; } = default!; // enum.ConditionOperator
    public string Value { get; set; } = default!;
}

public class ResolvePolicy
{
    public DateTime EffectiveFrom { get; set; }
    public double AssignedEntitlement { get; set; }
    public Guid EmployeeId { get; set; } // HRM.Profile.Employee
    public Guid LeaveTypeId { get; set; } // LeaveType
    public Guid LeavePolicyId { get; set; } // LeavePolicy
}

public class LeaveReqValResult
{
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public bool IsValid => !Errors.Any();
    public LeaveBalanceInfo? LeaveBalance { get; set; }
    public EligibilityInfo? EligibilityInfo { get; set; }
    public PolicyConstraintsInfo? PolicyConstraints { get; set; }
    public HolidayInfo? HolidayInfo { get; set; }
    public double CalculatedWorkingDays { get; set; }

    public void AddError(string error) => Errors.Add(error);
    public void AddWarning(string warning) => Warnings.Add(warning);
}

public class LeaveBalanceInfo
{
    public double AssignedEntitlement { get; set; }
    public double UsedDays { get; set; }
    public double RemainingBalance { get; set; }
    public double RequestedDays { get; set; }
}

public class EligibilityInfo
{
    public DateTime JoiningDate { get; set; }
    public int ServiceMonths { get; set; }
    public int MinRequiredMonths { get; set; }
    public bool IsEligible { get; set; }
}

public class PolicyConstraintsInfo
{
    public double MaxDaysPerRequest { get; set; }
    public bool RequiresAttachment { get; set; }
}

public class HolidayInfo
{
    public int TotalNonWorkingDays { get; set; }
    public int HolidaysCount { get; set; }
    public int WeekendsCount { get; set; }
    public List<HolidayDate> HolidayDates { get; set; } = new();
}

public class HolidayDate
{
    public DateTime Date { get; set; }
    public string Name { get; set; } = default!;
}

public class HolidaySerListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTime Date { get; set; } = default!;
    public bool IsPublic { get; set; } = true;
    public Guid FiscalYearId { get; set; }
}

public class LeaveAppRes
{
    public Guid LeaveRequestId { get; set; }
    public string Status { get; set; } = default!;
    public string Message { get; set; } = default!;
    public int CurrentStep { get; set; }
    public int TotalSteps { get; set; }
    public bool RequiresApproval { get; set; }
    public bool IsFinalApproval { get; set; }
    public ApproverInfo? NextApprover { get; set; }
    public List<string> Errors { get; set; } = new();

    public bool IsSuccessful => !Errors.Any();
    public void AddError(string error) => Errors.Add(error);
}

public class ApproverInfo
{
    public int StepOrder { get; set; }
    public string StepName { get; set; } = default!;
    public string Role { get; set; } = default!;
    public Guid? SpecificEmployeeId { get; set; }
    public bool IsFinalStep { get; set; }
}

public class EmpLeaveBal
{
    [JsonIgnore]
    public double Balance { get; set; }
    [JsonIgnore]
    public double AssignedEntitlement { get; set; }
    [JsonIgnore]
    public Guid EmployeeId { get; set; } // HRM.Profile.Employee
    [JsonIgnore]
    public Guid LeaveTypeId { get; set; } // LeaveType
    [JsonIgnore]
    public Guid LeavePolicyId { get; set; } // LeavePolicy

    public string LeaveType { get; set; } = default!;
    public double Percent { get; set; } = 100;
    public string TotalDays { get; set; } = default!;
    public string RemainDays { get; set; } = default!;
    public string UsedDays { get; set; } = default!;
}

public class DoubleDto
{
    public double Value { get; set; }
}

public class PendingApprovalInfo
{
    public Guid LeaveRequestId { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = default!;
    public string LeaveType { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double DaysRequested { get; set; }
    public int CurrentStep { get; set; }
    public int TotalSteps { get; set; }
    public string StepName { get; set; } = default!;
    public DateTime SubmittedDate { get; set; }
}

public class NonWorkingDay
{
    public DateTime Date { get; set; }
    public string Type { get; set; } = default!; // "Weekend" or "Holiday"
    public string Description { get; set; } = default!;
}

public class HolidayStatistics
{
    public Guid FiscalYearId { get; set; }
    public string FiscalYearName { get; set; } = default!;
    public int TotalHolidays { get; set; }
    public int PublicHolidays { get; set; }
    public double TotalWorkingDays { get; set; }
    public Dictionary<int, int> HolidaysByMonth { get; set; } = new();
    public Dictionary<string, int> HolidaysByDayOfWeek { get; set; } = new();
}
