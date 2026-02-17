using System.ComponentModel.DataAnnotations;

namespace Helpers;

#region Cor.HRMM Enums

public enum Gender
{
    [Display(Name = "Male")]
    Male,
    [Display(Name = "Female")]
    Female
}

public enum PositionGender
{
    [Display(Name = "Male")]
    Male,
    [Display(Name = "Female")]
    Female,
    [Display(Name = "Male/Female")]
    Both
}

public enum YesNo
{
    [Display(Name = "Yes")]
    Yes,
    [Display(Name = "No")]
    No
}

public enum WorkOption
{
    [Display(Name = "Morning")]
    Morning,
    [Display(Name = "Afternoon")]
    Afternoon,
    [Display(Name = "Both")]
    Both,
    [Display(Name = "None")]
    None
}

public enum Per
{
    [Display(Name = "Day")]
    Day,
    [Display(Name = "Month")]
    Month,
    [Display(Name = "Year")]
    Year
}

public enum ProfessionType
{
    [Display(Name = "Professional")]
    Pro,
    [Display(Name = "Semi-Professional")]
    SemiPro,
    [Display(Name = "Non-Professional")]
    NonPro
}

#endregion

#region Cor.Module Enums

public enum BranchType
{
    [Display(Name = "Head Office")]
    HeadOff,
    [Display(Name = "Regional")]
    RegOff,
    [Display(Name = "Local")]
    LocOff,
    [Display(Name = "Virtual")]
    VirOff
}

public enum BranchStat
{
    [Display(Name = "Active")]
    Active,
    [Display(Name = "Inactive")]
    InAct,
    [Display(Name = "Under construction")]
    UndCon
}

public enum DeptStat
{
    [Display(Name = "Active")]
    Active,
    [Display(Name = "Inactive")]
    InAct
}

public enum Quarter
{
    [Display(Name = "1st Quarter")]
    Q1,
    [Display(Name = "2nd Quarter")]
    Q2,
    [Display(Name = "3rd Quarter")]
    Q3,
    [Display(Name = "4th Quarter")]
    Q4
}

#endregion

#region HRM.Leave Enums

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
    [Display(Name = "Employement Nature")]
    EmpNat,
    [Display(Name = "Gender")]
    Gender,
    [Display(Name = "Service Months")]
    SerYear,
    [Display(Name = "Work Arrangement")]
    WorkAr,
    [Display(Name = "Job Grade")]
    Jg,
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

public enum LedgerSource
{
    [Display(Name = "Accrual")]
    Accrual,
    [Display(Name = "Leave Request")]
    LeaveReq,
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
    [Display(Name = "Credit")]
    Credit,
    [Display(Name = "Debit")]
    Debit
}

public enum AccuralSource
{
    [Display(Name = "SYSTEM")]
    Sys,
    [Display(Name = "MANUAL")]
    Manual
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
#endregion

#region HRM.Profile Enums

public enum EmpType
{
    [Display(Name = "Replacement")]
    Rep,
    [Display(Name = "New Opening")]
    NewOp,
    [Display(Name = "Additional Required")]
    AddReq,
    [Display(Name = "Old Employee")]
    Old
}

public enum EmpNature
{
    [Display(Name = "Permanent / Full-time")]
    Per,
    [Display(Name = "Contract / Fixed-term")]
    Con,
    [Display(Name = "Probation")]
    Pro,
    [Display(Name = "Intern / Trainee")]
    Inte,
    [Display(Name = "Part-time / Casual")]
    Par
}

public enum WorkArrangement
{
    [Display(Name = "On-site")]
    OnSite,
    [Display(Name = "Remote")]
    Remote,
    [Display(Name = "Hybrid")]
    Hybrid,
    [Display(Name = "Shift-based")]
    ShiftB,
    [Display(Name = "Rotational / Roster-based")]
    Rota
}

public enum MaritalStat
{
    [Display(Name = "Single / Not Married")]
    NotMar,
    [Display(Name = "Married")]
    Mar,
    [Display(Name = "Widow/er")]
    Wid,
    [Display(Name = "Divorced")]
    Div,
    [Display(Name = "Not Mentioned")]
    NotMen
}

public enum AddressType
{
    [Display(Name = "Residence")]
    Res,
    [Display(Name = "Work Place")]
    Work
}

#endregion

#region HRM.Recruit Enums

public enum EducationLevel
{
    [Display(Name = "Preparatory")]
    Prep,
    [Display(Name = "Collage")]
    Coll,
    [Display(Name = "TVT")]
    Tvt,
    [Display(Name = "University")]
    Univ,
    [Display(Name = "Elementary")]
    Elem,
    [Display(Name = "High School")]
    HiSch,
    [Display(Name = "None")]
    None
}

public enum ReviewStat
{
    [Display(Name = "Approve")]
    App,
    [Display(Name = "Modify")]
    ReWork,
    [Display(Name = "Reject")]
    Rej
}

public enum JobPostingType
{
    [Display(Name = "Internal")]
    Internal,
    [Display(Name = "External")]
    External,
    [Display(Name = "Internal & External")]
    Both
}

public enum ReqStatus
{
    [Display(Name = "Pending Approval")]
    Pending,
    [Display(Name = "Approved")]
    Approved,
    [Display(Name = "Rejected")]
    Rejected,
    [Display(Name = "Cancelled")]
    Cancelled,
    [Display(Name = "Closed")]
    Closed
}

public enum PostingStatus
{
    [Display(Name = "Pending Approval")]
    Pending,
    [Display(Name = "Published")]
    Published,
    [Display(Name = "Closed")]
    Closed,
    [Display(Name = "On Hold")]
    OnHold,
    [Display(Name = "Cancelled")]
    Cancelled
}

public enum PostStatus
{
    [Display(Name = "Pending Approval")]
    Pending,
    [Display(Name = "On Hold")]
    OnHold,
    [Display(Name = "Cancelled")]
    Cancelled
}

public enum ApplicationStatus
{
    [Display(Name = "Applied")]
    Applied,
    [Display(Name = "Under Review")]
    UnderReview,
    [Display(Name = "Shortlisted")]
    Shortlisted,
    [Display(Name = "Rejected")]
    Rejected,
    [Display(Name = "Withdrawn")]
    Withdrawn,
    [Display(Name = "Interview Scheduled")]
    InterviewScheduled,
    [Display(Name = "Interview Completed")]
    InterviewCompleted,
    [Display(Name = "Offer Extended")]
    OfferExtended,
    [Display(Name = "Offer Accepted")]
    OfferAccepted,
    [Display(Name = "Offer Rejected")]
    OfferRejected,
    [Display(Name = "On Hold")]
    OnHold
}

public enum InterviewType
{
    [Display(Name = "Phone Screening")]
    PhoneScreen,
    [Display(Name = "Technical Interview")]
    TechnicalRound,
    [Display(Name = "Behavioral Interview")]
    BehavioralRound,
    [Display(Name = "Management Interview")]
    ManagementRound,
    [Display(Name = "Group Discussion")]
    GroupDiscussion,
    [Display(Name = "Presentation")]
    PresentationRound,
    [Display(Name = "Final Interview")]
    FinalRound
}

public enum InterviewStatus
{
    [Display(Name = "Scheduled")]
    Scheduled,
    [Display(Name = "In Progress")]
    InProgress,
    [Display(Name = "Completed")]
    Completed,
    [Display(Name = "Cancelled")]
    Cancelled,
    [Display(Name = "Rescheduled")]
    Rescheduled,
    [Display(Name = "No Show")]
    NoShow
}

public enum OfferStatus
{
    [Display(Name = "Draft")]
    Draft,
    [Display(Name = "Pending Approval")]
    Pending,
    [Display(Name = "Approved")]
    Approved,
    [Display(Name = "Rejected")]
    Rejected,
    [Display(Name = "Extended")]
    Extended,
    [Display(Name = "Accepted")]
    Accepted,
    [Display(Name = "Declined")]
    Declined,
    [Display(Name = "Withdrawn")]
    Withdrawn,
    [Display(Name = "Expired")]
    Expired
}

public enum EmployeeStatus
{
    [Display(Name = "Inactive")]
    Inactive,
    [Display(Name = "Active")]
    Active,
    [Display(Name = "On Leave")]
    OnLeave,
    [Display(Name = "Suspended")]
    Suspended,
    [Display(Name = "Terminated")]
    Terminated,
    [Display(Name = "Retired")]
    Retired
}

public enum ApprovalStatus
{
    [Display(Name = "Pending")]
    Pending,
    [Display(Name = "Approved")]
    Approved,
    [Display(Name = "Rejected")]
    Rejected,
    [Display(Name = "Recalled")]
    Recalled,
    [Display(Name = "Escalated")]
    Escalated
}

public enum OnboardingStatus
{
    [Display(Name = "Pending")]
    Pending,
    [Display(Name = "In Progress")]
    InProgress,
    [Display(Name = "Completed")]
    Completed,
    [Display(Name = "On Hold")]
    OnHold,
    [Display(Name = "Cancelled")]
    Cancelled
}

#endregion