using System.Text.Json.Serialization;

namespace Recruit.Domain.DTOs;

public class ReviewDto
{
    public Guid Id { get; set; }
    [JsonIgnore]
    public Guid ReviewById { get; set; }
    public int AppCount { get; set; }
    public string Status { get; set; } = default!; // enum.ReviewStat(0/1) // Accept/Modify/Reject
    public string Comment { get; set; } = default!;
}

public class ReviewAllDto
{
    public Guid Id { get; set; }
    [JsonIgnore]
    public Guid ReviewById { get; set; }
    public string Status { get; set; } = default!; // enum.ReviewStat(0/1) // Accept/Modify/Reject
    public string Comment { get; set; } = default!;
}

public class PostPublish
{
    public Guid Id { get; set; }
    [JsonIgnore]
    public Guid ReviewById { get; set; }
    public string Comment { get; set; } = default!;
}

public class JpAppEvalDto
{
    [JsonIgnore]
    public Guid EvaluatorId { get; set; }
    public Guid Id { get; set; }
    public double Score { get; set; }
    public string Feedback { get; set; } = default!;
}

public class EvalScoreHistDto
{
    public Guid StepId { get; set; }
    public string StepName { get; set; } = default!;
    public int StepOrder { get; set; }
    public double Score { get; set; }
    public string? Feedback { get; set; }
}

public class JobAppEvalProgressDto
{
    public Guid JobAppId { get; set; }
    public bool IsStarted { get; set; }
    public bool IsCompleted { get; set; }
    public string AppStatus { get; set; } = default!; // JobApplication.Status
    public Guid CurrentStepId { get; set; }
    public string CurrentStepName { get; set; } = default!;
    public int CurrentStepOrder { get; set; }
    public double MinScore { get; set; }
    public double MaxScore { get; set; }
    public bool IsFinal { get; set; }
    public int TotalSteps { get; set; }
    public int CompletedSteps { get; set; }
    public List<EvalScoreHistDto> Scores { get; set; } = new();
}