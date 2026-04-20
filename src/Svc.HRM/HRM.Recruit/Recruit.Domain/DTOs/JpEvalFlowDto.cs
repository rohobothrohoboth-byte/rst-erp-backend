using System.Text.Json.Serialization;

namespace Recruit.Domain.DTOs;

public class JpEvalFlowListDto : BaseDto
{
    [JsonIgnore]
    public string PostType { get; set; } = default!; // enum.JobPostingType(0/1) 
    [JsonIgnore]
    public DateTime PublishedDate { get; set; }
    [JsonIgnore]
    public DateTime EffectiveFrom { get; set; }
    [JsonIgnore]
    public DateTime? EffectiveTo { get; set; }
    [JsonIgnore]
    public Guid EvaluationFlowId { get; set; } // EvaluationFlow
    [JsonIgnore]
    public Guid JobPostingId { get; set; } // JobPosting

    public string PostNumber { get; set; } = default!;
    public string DatePublished => $"{PublishedDate:MMMM dd, yyyy}";
    public string EffeDateFrom => $"{EffectiveFrom:MMMM dd, yyyy}";
    public string EffeDateTo => EffectiveTo.HasValue ? $"{EffectiveTo:MMMM dd, yyyy}" : "";
    public string EvalFlowName { get; set; } = default!;
    public string PostTypeStr { get; set; } = default!;    
    public List<EvalSteps> Steps { get; set; } = [];
}

public class EvalSteps
{
    [JsonIgnore]
    public bool IsFinal { get; set; } = false;
    [JsonIgnore]
    public int StepOrder { get; set; }    // 1, 2, 3 ...

    public string EvalType { get; set; } = default!;
    public string StepName { get; set; } = default!;
    public double MaxScore { get; set; }
    public double MinScore { get; set; }
    public string IsFinalStr { get; set; } = default!;
}

public class JpEvalFlowAddDto
{
    public Guid EvaluationFlowId { get; set; } // EvaluationFlow
    public Guid JobPostingId { get; set; } // JobPosting
    public DateTime EffectiveFrom { get; set; }
}

public class JpEvalFlowModDto
{
    public Guid Id { get; set; }
    public Guid EvaluationFlowId { get; set; } // EvaluationFlow
    public DateTime EffectiveFrom { get; set; }
    public string RowVersion { get; set; } = default!;
}