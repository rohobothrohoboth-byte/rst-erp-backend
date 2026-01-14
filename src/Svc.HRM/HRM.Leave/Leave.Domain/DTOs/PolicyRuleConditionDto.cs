namespace Leave.Domain.DTOs;

public class PolicyRuleConditionListDto : BaseDto
{
    public string Field { get; set; } = default!;
    public string Operator { get; set; } = default!; // enum.ConditionOperator
    public string Value { get; set; } = default!;

    public string OperatorStr { get; set; } = default!;
    public string PolicyAssignmentRule { get; set; } = default!; // PolicyAssignmentRule
}

public class PolicyRuleConditionAddDto
{
    public string Field { get; set; } = default!;
    public string Operator { get; set; } = default!; // enum.ConditionOperator
    public string Value { get; set; } = default!;
    public Guid PolicyAssignmentRuleId { get; set; } // PolicyAssignmentRule
}

public class PolicyRuleConditionModDto
{
    public Guid Id { get; set; }
    public string Field { get; set; } = default!;
    public string Operator { get; set; } = default!; // enum.ConditionOperator
    public string Value { get; set; } = default!;
    public Guid PolicyAssignmentRuleId { get; set; } // PolicyAssignmentRule
    public string RowVersion { get; set; } = default!;
}