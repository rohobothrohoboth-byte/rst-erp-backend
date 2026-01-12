namespace Leave.Domain.Entities;

public class PolicyRuleCondition : BaseEntity
{
    public string Field { get; set; } = default!;
    public string Operator { get; set; } = default!; // enum.ConditionOperator
    public string Value { get; set; } = default!;
    public Guid PolicyAssignmentRuleId { get; set; } // PolicyAssignmentRule

    //******************************************//

    public PolicyAssignmentRule PolicyAssignmentRule { get; set; } = null!;
}