// Leave.Domain/DTOs/LeaveTypeDto.cs
namespace Leave.Domain.DTOs;

public class LeaveTypeListDto : BaseDto
{
    public string Name { get; set; } = default!;
    public string? NameAm { get; set; }
    public string Code { get; set; } = default!;
    public string LeaveCategory { get; set; } = default!;
    public string? Description { get; set; }

    // Accrual
    public string AccrualFrequency { get; set; } = "Annual";
    public decimal AccrualRate { get; set; } = 12;
    public decimal MaxAccrual { get; set; } = 30;
    public bool AllowCarryover { get; set; } = true;
    public decimal MaxCarryoverDays { get; set; } = 5;
    public int CarryoverExpiryMonths { get; set; } = 3;

    // Usage
    public int MaxDaysPerRequest { get; set; } = 30;
    public int MaxDaysPerYear { get; set; } = 30;
    public decimal MinDaysPerRequest { get; set; } = 0.5M;
    public bool RequiresAttachment { get; set; } = false;
    public bool RequiresDoctorNote { get; set; } = false;
    public bool RequiresApproval { get; set; } = true;
    public bool AllowHalfDay { get; set; } = true;
    public bool AllowNegativeBalance { get; set; } = false;
    public bool HolidaysAsLeave { get; set; } = false;

    // Eligibility
    public int MinServiceMonths { get; set; } = 0;
    public bool ProbationPeriodOnly { get; set; } = false;
    public string[] EligibleEmploymentTypes { get; set; } = new[] { "Permanent" };

    // Notifications
    public int[] SendReminderDays { get; set; } = new[] { 30, 14, 7, 3 };
    public bool NotifyManagerOnRequest { get; set; } = true;

    // Display
    public string Icon { get; set; } = "Calendar";
    public string Color { get; set; } = "emerald";
    public int Priority { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    // String representations
    public string LeaveCategoryStr { get; set; } = default!;
    public string RequiresApprovalStr { get; set; } = default!;
    public string AllowHalfDayStr { get; set; } = default!;
    public string IsActiveStr { get; set; } = default!;
    public string HolidaysAsLeaveStr { get; set; } = default!;
    public string AccrualFrequencyStr { get; set; } = default!;
    public string AllowCarryoverStr { get; set; } = default!;
    public string RequiresAttachmentStr { get; set; } = default!;
    public string RequiresDoctorNoteStr { get; set; } = default!;
    public string AllowNegativeBalanceStr { get; set; } = default!;
    public string ProbationPeriodOnlyStr { get; set; } = default!;
}

public class LeaveTypeAddDto
{
    public string Name { get; set; } = default!;
    public string? NameAm { get; set; }
    public string Code { get; set; } = default!;
    public string LeaveCategory { get; set; } = "Paid";
    public string? Description { get; set; }

    public string AccrualFrequency { get; set; } = "Annual";
    public decimal AccrualRate { get; set; } = 12;
    public decimal MaxAccrual { get; set; } = 30;
    public bool AllowCarryover { get; set; } = true;
    public decimal MaxCarryoverDays { get; set; } = 5;
    public int CarryoverExpiryMonths { get; set; } = 3;

    public int MaxDaysPerRequest { get; set; } = 30;
    public int MaxDaysPerYear { get; set; } = 30;
    public decimal MinDaysPerRequest { get; set; } = 0.5M;
    public bool RequiresAttachment { get; set; } = false;
    public bool RequiresDoctorNote { get; set; } = false;
    public bool RequiresApproval { get; set; } = true;
    public bool AllowHalfDay { get; set; } = true;
    public bool AllowNegativeBalance { get; set; } = false;
    public bool HolidaysAsLeave { get; set; } = false;

    public int MinServiceMonths { get; set; } = 0;
    public bool ProbationPeriodOnly { get; set; } = false;
    public string[] EligibleEmploymentTypes { get; set; } = new[] { "Permanent" };

    public int[] SendReminderDays { get; set; } = new[] { 30, 14, 7, 3 };
    public bool NotifyManagerOnRequest { get; set; } = true;

    public string Icon { get; set; } = "Calendar";
    public string Color { get; set; } = "emerald";
    public int Priority { get; set; } = 0;
    public ApprovalChainInputDto? ApprovalChain { get; set; }
}

public class ApprovalChainInputDto
{
    public string EffectiveFrom { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd");
    public List<ApprovalStepInputDto> Steps { get; set; } = new();
}

public class ApprovalStepInputDto
{
    public int StepOrder { get; set; }
    public string StepName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? ApproverValue { get; set; }
    public bool IsFinal { get; set; }
    public int? TimeoutHours { get; set; }
}

public class LeaveTypeModDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? NameAm { get; set; }
    public string Code { get; set; } = default!;
    public string LeaveCategory { get; set; } = "Paid";
    public string? Description { get; set; }

    public string AccrualFrequency { get; set; } = "Annual";
    public decimal AccrualRate { get; set; } = 12;
    public decimal MaxAccrual { get; set; } = 30;
    public bool AllowCarryover { get; set; } = true;
    public decimal MaxCarryoverDays { get; set; } = 5;
    public int CarryoverExpiryMonths { get; set; } = 3;

    public int MaxDaysPerRequest { get; set; } = 30;
    public int MaxDaysPerYear { get; set; } = 30;
    public decimal MinDaysPerRequest { get; set; } = 0.5M;
    public bool RequiresAttachment { get; set; } = false;
    public bool RequiresDoctorNote { get; set; } = false;
    public bool RequiresApproval { get; set; } = true;
    public bool AllowHalfDay { get; set; } = true;
    public bool AllowNegativeBalance { get; set; } = false;
    public bool HolidaysAsLeave { get; set; } = false;

    public int MinServiceMonths { get; set; } = 0;
    public bool ProbationPeriodOnly { get; set; } = false;
    public string[] EligibleEmploymentTypes { get; set; } = new[] { "Permanent" };

    public int[] SendReminderDays { get; set; } = new[] { 30, 14, 7, 3 };
    public bool NotifyManagerOnRequest { get; set; } = true;

    public string Icon { get; set; } = "Calendar";
    public string Color { get; set; } = "emerald";
    public int Priority { get; set; } = 0;

    public bool IsActive { get; set; } = true;
    public string RowVersion { get; set; } = default!;
}

public class StatChangeDto
{
    public Guid Id { get; set; }
    public bool Stat { get; set; }
    public string RowVersion { get; set; } = string.Empty;
}

public class LeaveTypeNameDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameAm { get; set; }
}