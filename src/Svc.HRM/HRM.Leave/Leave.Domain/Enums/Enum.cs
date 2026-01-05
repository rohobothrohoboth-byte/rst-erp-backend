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
    [Display(Name = "Annual")]
    Annual,
    [Display(Name = "BiAnnual")]
    BiAnnual,
    [Display(Name = "Quarterly")]
    Quarterly,
    [Display(Name = "Monthly")]
    Monthly,
    [Display(Name = "Daily")]
    Daily,
    [Display(Name = "None")]
    None
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

public enum LeaveCondition 
{
    [Display(Name = "With Half Salary")]
    HalfSalary,
    [Display(Name = "With Full Salary")]
    FullSlary,
    [Display(Name = "With No Salary")]
    NoSalary
}

public enum PolicyGender
{
    [Display(Name = "Male")]
    Male,
    [Display(Name = "Female")]
    Female,
    [Display(Name = "Male/Female")]
    Both
}