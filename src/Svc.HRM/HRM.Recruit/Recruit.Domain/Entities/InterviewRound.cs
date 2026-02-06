namespace Recruit.Domain.Entities;

public class InterviewRound : BaseEntity
{
    public int RoundNumber { get; set; }
    public string RoundName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DateTime ScheduledDate { get; set; }
    public string Status { get; set; } = default!; // enum.InterviewStatus(0/1)
    public Guid InterviewId { get; set; } // Interview
    public Guid? FeedbackId { get; set; } // InterviewFeedback

    //******************************************//

    public Interview Interview { get; set; } = null!;
    public InterviewFeedback Feedback { get; set; } = null!;
}