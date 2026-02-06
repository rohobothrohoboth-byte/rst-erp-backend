namespace Recruit.Domain.Entities;

public class InterviewFeedback : BaseEntity
{
    public double Rating { get;  set; }
    public string Strengths { get;  set; } = default!;
    public string WeakAreas { get;  set; } = default!;
    public string OverallComments { get;  set; } = default!;
    public string Recommendation { get;  set; } = default!;
}