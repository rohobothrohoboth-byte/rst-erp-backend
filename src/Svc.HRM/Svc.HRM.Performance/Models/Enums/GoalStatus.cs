namespace Svc.HRM.Performance.Models.Enums;

public enum GoalStatus
{
    NotStarted,
    InProgress,
    Completed,
    Cancelled,
    OnHold
}

public enum ReviewStatus
{
    Draft,
    Submitted,
    InReview,
    Approved,
    Rejected,
    Cancelled
}

public enum FeedbackType
{
    Peer,
    Manager,
    Self,
    SkipLevel,
    External
}
