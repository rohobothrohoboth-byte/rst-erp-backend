// Leave.Domain/Entities/LeaveType.cs
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Leave.Domain.Entities;

public class LeaveType : BaseEntity
{
    // Basic Info
    public string Name { get; set; } = default!;
    public string? NameAm { get; set; }
    public string Code { get; set; } = default!;
    public string LeaveCategory { get; set; } = default!; // Paid, Unpaid, Special
    public string? Description { get; set; }

    // Accrual Settings
    public string AccrualFrequency { get; set; } = "Annual"; // Annual, Monthly, Daily, None
    public decimal AccrualRate { get; set; } = 12;
    public decimal MaxAccrual { get; set; } = 30;
    public bool AllowCarryover { get; set; } = true;
    public decimal MaxCarryoverDays { get; set; } = 5;
    public int CarryoverExpiryMonths { get; set; } = 3;

    // Usage Limits
    public int MaxDaysPerRequest { get; set; } = 30;
    public int MaxDaysPerYear { get; set; } = 30;
    public decimal MinDaysPerRequest { get; set; } = 0.5M;
    public bool RequiresAttachment { get; set; } = false;
    public bool RequiresDoctorNote { get; set; } = false;
    public bool RequiresApproval { get; set; } = true;
    public bool AllowHalfDay { get; set; } = true;
    public bool AllowNegativeBalance { get; set; } = false;

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

    // Status
    public bool IsActive { get; set; } = true;
    public bool HolidaysAsLeave { get; set; } = false;
}