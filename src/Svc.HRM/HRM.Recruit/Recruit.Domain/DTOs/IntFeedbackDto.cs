namespace Recruit.Domain.DTOs;

public class IntFeedbackListDto : BaseDto
{
    public double Rating { get; set; }
    public string Strengths { get; set; } = default!;
    public string WeakAreas { get; set; } = default!;
    public string OverallComments { get; set; } = default!;
    public string Recommendation { get; set; } = default!;
}

public class IntFeedbackAddDto
{
    public double Rating { get; set; }
    public string Strengths { get; set; } = default!;
    public string WeakAreas { get; set; } = default!;
    public string OverallComments { get; set; } = default!;
    public string Recommendation { get; set; } = default!;
}

public class IntFeedbackModDto
{
    public Guid Id { get; set; }
    public double Rating { get; set; }
    public string Strengths { get; set; } = default!;
    public string WeakAreas { get; set; } = default!;
    public string OverallComments { get; set; } = default!;
    public string Recommendation { get; set; } = default!;
    public string RowVersion { get; set; } = default!;
}