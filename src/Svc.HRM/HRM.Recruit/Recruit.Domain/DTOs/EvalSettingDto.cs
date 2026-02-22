namespace Recruit.Domain.DTOs;

public class EvalFlowListDto : BaseDto
{
    public string Name { get; set; } = default!;
    public bool IsGlobal { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public string IsGlobalStr { get; set; } = default!;
    public string IsActiveStr { get; set; } = default!;
}

public class EvalFlowAddDto
{
    public string Name { get; set; } = default!;
    public bool IsGlobal { get; set; } = false;
}

public class EvalFlowModDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public bool IsGlobal { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public string RowVersion { get; set; } = default!;
}

public class EvalStepListDto : BaseDto
{
    public string StepName { get; set; } = default!;
    public int StepOrder { get; set; }    // 1, 2, 3 ...
    public bool IsFinal { get; set; } = false;
    public string IsFinalStr { get; set; } = default!;
    public string EvaluationFlow { get; set; } = default!; // EvaluationFlow
    public string EvalType { get; set; } = default!; // EvaluationType
}

public class EvalStepAddDto
{
    public string StepName { get; set; } = default!;
    public int StepOrder { get; set; }    // 1, 2, 3 ...
    public bool IsFinal { get; set; } = false;
    public Guid EvalTypeId { get; set; } // EvaluationType
    public Guid EvaluationFlowId { get; set; } // EvaluationFlow
}

public class EvalStepModDto
{
    public Guid Id { get; set; }
    public string StepName { get; set; } = default!;
    public int StepOrder { get; set; }    // 1, 2, 3 ...
    public bool IsFinal { get; set; } = false;
    public Guid EvalTypeId { get; set; } // EvaluationType
    public Guid EvaluationFlowId { get; set; } // EvaluationFlow
    public string RowVersion { get; set; } = default!;
}

public class EvalTypeListDto : BaseDto
{
    public string Name { get; set; } = default!;
    public double MaxScore { get; set; }
    public bool IsActive { get; set; } = true;
    public string IsActiveStr { get; set; } = default!;
}

public class EvalTypeAddDto
{
    public string Name { get; set; } = default!;
    public double MaxScore { get; set; }
}

public class EvalTypeModDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public double MaxScore { get; set; }
    public bool IsActive { get; set; } = true;
    public string RowVersion { get; set; } = default!;
}

public class JobEvalFlowListDto : BaseDto
{
    public Guid EvalFlowId { get; set; }
    public Guid JobPostingId { get; set; } // JobPosting
    public string FlowName { get; set; } = default!;
    public string JobPostNum { get; set; } = default!;
    public int StepsNum { get; set; }
}

public class JobEvalFlowAddDto
{
    public Guid EvaluationFlowId { get; set; } // EvaluationFlow
    public Guid JobPostingId { get; set; } // JobPosting
}