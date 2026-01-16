namespace Leave.Domain.DTOs;

public class PolicyRuleCondListDto : BaseDto
{
    public string Field { get; set; } = default!; // enum.ConditionField
    public string Operator { get; set; } = default!; // enum.ConditionOperator
    public string Value { get; set; } = default!;

    public string FieldStr { get; set; } = default!;
    public string OperatorStr { get; set; } = default!;
    public string RuleName { get; set; } = default!; // PolicyAssignmentRule
}

public class PolicyRuleCondAddDto
{
    public string Field { get; set; } = default!; // enum.ConditionField
    public string Operator { get; set; } = default!; // enum.ConditionOperator
    public string Value { get; set; } = default!;
    public Guid PolicyAssRuleId { get; set; } // PolicyAssignmentRule
}

public class PolicyRuleCondModDto
{
    public Guid Id { get; set; }
    public string Field { get; set; } = default!; // enum.ConditionField
    public string Operator { get; set; } = default!; // enum.ConditionOperator
    public string Value { get; set; } = default!;
    public string RowVersion { get; set; } = default!;
}