namespace Recruit.Domain.DTOs;

public class InterviewListDto : BaseDto
{
    public string InterviewType { get; set; } = default!; // enum.InterviewType(0/1)
    public string Status { get; set; } = default!; // enum.InterviewStatus(0/1)
    public DateTime ScheduledDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public string? InterviewLocation { get; set; }
    public string? MeetingLink { get; set; }
    public Guid CreatedById { get; set; } // HRM.Profile.Employee
    public Guid? UpdatedById { get; set; } // HRM.Profile.Employee
    public Guid InterviewerId { get; set; } // HRM.Profile.Employee
    public Guid CanApplicationId { get; set; } // CandidateApplication
    public Guid FeedbackId { get; set; } // InterviewFeedback
}

public class InterviewAddDto
{
    public string InterviewType { get; set; } = default!; // enum.InterviewType(0/1)
    public DateTime ScheduledDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public string? InterviewLocation { get; set; }
    public string? MeetingLink { get; set; }
    public Guid CreatedById { get; set; } // HRM.Profile.Employee
    public Guid? UpdatedById { get; set; } // HRM.Profile.Employee
    public Guid InterviewerId { get; set; } // HRM.Profile.Employee
    public Guid JobApplicationId { get; set; } // JobApplication
    public Guid FeedbackId { get; set; } // InterviewFeedback
}

public class InterviewModDto
{
    public Guid Id { get; set; }
    public string InterviewType { get; set; } = default!; // enum.InterviewType(0/1)
    public string Status { get; set; } = default!; // enum.InterviewStatus(0/1)
    public DateTime ScheduledDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public string? InterviewLocation { get; set; }
    public string? MeetingLink { get; set; }
    public Guid CreatedById { get; set; } // HRM.Profile.Employee
    public Guid? UpdatedById { get; set; } // HRM.Profile.Employee
    public Guid InterviewerId { get; set; } // HRM.Profile.Employee
    public Guid JobApplicationId { get; set; } // JobApplication
    public Guid FeedbackId { get; set; } // InterviewFeedback
    public string RowVersion { get; set; } = default!;
}