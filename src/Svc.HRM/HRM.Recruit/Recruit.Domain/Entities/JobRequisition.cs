namespace Recruit.Domain.Entities;

public class JobRequisition : BaseEntity
{
    public string RequisitionNumber { get; set; } = default!;
    public string JobTitle { get; set; } = default!;
    public string JobDescription { get; set; } = default!;
    public int NumberOfPositions { get; set; }
    public int? BudgetCode { get; set; }
    public string Status { get; set; } = default!; // enum.RequisitionStatus(0/1)
    public DateTime TargetStartDate { get; set; }
    public string? ApprovalComments { get; set; }
    public Guid PositionId { get; set; } // Cor.HRMM.Employee
    public Guid JgStepId { get; set; } // Cor.HRMM.Employee
    public Guid WorkforcePlanId { get; set; } // WorkforcePlan

    //******************************************//

    public WorkforcePlan WorkforcePlan { get; set; } = null!;
}