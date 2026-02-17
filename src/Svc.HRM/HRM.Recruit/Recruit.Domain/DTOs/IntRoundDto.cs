namespace Recruit.Domain.DTOs;

public class IntRoundListDto : BaseDto
{
    public int RoundNumber { get; set; }
    public string RoundName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DateTime ScheduledDate { get; set; }
    public string Status { get; set; } = default!; // enum.InterviewStatus(0/1)
    public Guid InterviewId { get; set; } // Interview
    public Guid? FeedbackId { get; set; } // InterviewFeedback
}

public class IntRoundAddDto
{
    public int RoundNumber { get; set; }
    public string RoundName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DateTime ScheduledDate { get; set; }
    public string Status { get; set; } = default!; // enum.InterviewStatus(0/1)
    public Guid InterviewId { get; set; } // Interview
    public Guid? FeedbackId { get; set; } // InterviewFeedback
}

public class IntRoundModDto
{
    public Guid Id { get; set; }
    public int RoundNumber { get; set; }
    public string RoundName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DateTime ScheduledDate { get; set; }
    public string Status { get; set; } = default!; // enum.InterviewStatus(0/1)
    public Guid InterviewId { get; set; } // Interview
    public Guid? FeedbackId { get; set; } // InterviewFeedback
    public string RowVersion { get; set; } = default!;
}