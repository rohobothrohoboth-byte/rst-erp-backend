using System.ComponentModel.DataAnnotations;

namespace Leave.Domain.Enums;

public enum Status
{
    [Display(Name = "Pending")]
    Pending,
    [Display(Name = "Approved")]
    Approved,
    [Display(Name = "Rejected")]
    Rejected,
    [Display(Name = "Cancelled")]
    Cancelled
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

public enum ApprovalRole
{
    [Display(Name = "Manager")]
    Manager,
    [Display(Name = "HR")]
    Hr,
    [Display(Name = "Director")]
    Director
}

public enum LeaveCategory
{
    [Display(Name = "Paid")]
    Paid,
    [Display(Name = "Unpaid")]
    Unpaid,
    [Display(Name = "Special")]
    Special
}

public enum PolicyStatus
{
    [Display(Name = "Active")]
    Active,
    [Display(Name = "Inactive")]
    Inactive
}

public enum Priority
{
    [Display(Name = "High")]
    High,
    [Display(Name = "Medium")]
    Medium,
    [Display(Name = "Low")]
    Low
}

public enum ConditionOperator
{
    [Display(Name = "Equals")]
    Equal,
    [Display(Name = "Not Equals")]
    NotEquals,
    [Display(Name = "In")]
    In,
    [Display(Name = "Greater than")]
    GreaterThan,
    [Display(Name = "Less than")]
    LessThan,
    [Display(Name = "Greater than or Equals")]
    GreaterOrEqual,
    [Display(Name = "Less than or Equals")]
    LessOrEqual
}

public enum ConditionField
{
    [Display(Name = "Employement Type")]
    EmpType,
    [Display(Name = "Employement Nature")]
    EmpNat,
    [Display(Name = "Gender")]
    Gender,
    [Display(Name = "Service Months")]
    SerYear,
    [Display(Name = "Disablity Status")]
    Disable
}

public enum LedgerSource
{
    [Display(Name = "Accrual")]
    Accrual,
    [Display(Name = "Leave Approved")]
    LeaveApproved,
    [Display(Name = "Adjustment")]
    Adjustment,
    [Display(Name = "Encashment")]
    Encashment,
    [Display(Name = "Carry Over")]
    CarryOver,
    [Display(Name = "Expiry")]
    Expiry,
}

public enum LedgerEntryType
{
    [Display(Name = "Accrual")]
    Accrual,
    [Display(Name = "Request")]
    Request,
    [Display(Name = "Approval")]
    Approval,
    [Display(Name = "Deduction")]
    Deduction
}

public enum PolicyGender
{
    [Display(Name = "Male")]
    Male,
    [Display(Name = "Female")]
    Female,
    [Display(Name = "Male & Female")]
    Both
}

public enum AccuralSource
{
    [Display(Name = "SYSTEM")]
    Sys,
    [Display(Name = "MANUAL")]
    Manual
}

public enum EmpLeavePolReason
{
    [Display(Name = "Employee Onboarding")]
    OnBoard,
    [Display(Name = "Promotion")]
    Porm,
    [Display(Name = "Transfer")]
    Tra,
    [Display(Name = "Policy Migration")]
    PolChange
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