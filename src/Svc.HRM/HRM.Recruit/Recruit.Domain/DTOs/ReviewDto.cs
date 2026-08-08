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