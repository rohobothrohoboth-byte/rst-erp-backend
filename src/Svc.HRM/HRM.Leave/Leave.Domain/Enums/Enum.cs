using System.ComponentModel.DataAnnotations;

namespace Leave.Domain.Enums;

public enum LeaveRequestStatus
{
    [Display(Name = "Draft")]
    Draft = 0,
    [Display(Name = "Pending")]
    Pending = 1,
    [Display(Name = "Approved")]
    Approved = 2,
    [Display(Name = "ApprovedPartial")]
    ApprovedPartial = 3,
    [Display(Name = "Rejected")]
    Rejected = 4,
    [Display(Name = "Cancelled")]
    Cancelled = 5,
    [Display(Name = "Withdrawn")]
    Withdrawn = 6,
    [Display(Name = "AutoApproved")]
    AutoApproved = 7
}

public enum LedgerSourceType
{
    [Display(Name = "Accrual")]
    Accrual = 0,
    [Display(Name = "Request")]
    Request = 1,
    [Display(Name = "Adjustment")]
    Adjustment = 2,
    [Display(Name = "Encashment")]
    Encashment = 3,
    [Display(Name = "Reconciliation")]
    Reconciliation = 4
}

public enum AccrualFrequency
{
    [Display(Name = "Monthly")]
    Monthly = 0,
    [Display(Name = "Yearly")]
    Yearly = 1,
    [Display(Name = "Daily")]
    Daily = 2,
    [Display(Name = "None")]
    None = 3
}

public enum LedgerEntryType
{
    [Display(Name = "Accrual")]
    Accrual = 1,
    [Display(Name = "Request")]
    Request = 2,
    [Display(Name = "Approval")]
    Approval = 3,
    [Display(Name = "Deduction")]
    Deduction = 4
}