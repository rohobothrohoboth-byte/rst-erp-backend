using System.ComponentModel.DataAnnotations;

namespace Recruit.Domain.Enums;

public enum Gender
{
    [Display(Name = "Male")]
    Male,
    [Display(Name = "Female")]
    Female
}

public enum AddressType
{
    [Display(Name = "Residence")]
    Res,
    [Display(Name = "Work Place")]
    Work
}

public enum JobPostingType
{
    Internal,
    External,
    Both
}

public enum RequisitionStatus
{
    Draft = 0,
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Cancelled = 4,
    Closed = 5
}

public enum PostingStatus
{
    Draft = 0,
    Published = 1,
    Closed = 2,
    OnHold = 3,
    Cancelled = 4
}

public enum ApplicationStatus
{
    Applied = 0,
    UnderReview = 1,
    Shortlisted = 2,
    Rejected = 3,
    Withdrawn = 4,
    InterviewScheduled = 5,
    InterviewCompleted = 6,
    OfferExtended = 7,
    OfferAccepted = 8,
    OfferRejected = 9,
    OnHold = 10
}

public enum InterviewType
{
    PhoneScreen = 0,
    TechnicalRound = 1,
    BehavioralRound = 2,
    ManagementRound = 3,
    GroupDiscussion = 4,
    PresentationRound = 5,
    FinalRound = 6
}

public enum InterviewStatus
{
    Scheduled = 0,
    InProgress = 1,
    Completed = 2,
    Cancelled = 3,
    Rescheduled = 4,
    NoShow = 5
}

public enum OfferStatus
{
    Draft = 0,
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Extended = 4,
    Accepted = 5,
    Declined = 6,
    Withdrawn = 7,
    Expired = 8
}

public enum EmployeeStatus
{
    Inactive = 0,
    Active = 1,
    OnLeave = 2,
    Suspended = 3,
    Terminated = 4,
    Retired = 5
}

public enum ApprovalStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,
    Recalled = 3,
    Escalated = 4
}

public enum OnboardingStatus
{
    Pending = 0,
    InProgress = 1,
    Completed = 2,
    OnHold = 3,
    Cancelled = 4
}
