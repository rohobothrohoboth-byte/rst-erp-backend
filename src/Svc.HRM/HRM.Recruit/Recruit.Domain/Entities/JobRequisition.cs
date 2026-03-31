namespace Recruit.Domain.Entities;

public class JobRequisition : BaseEntity
{
    public string ReqNumber { get; private set;} = default!;
    public string ReqReason { get; set; } = default!;
    public int ReqQuantity { get; set; }
    public string BudgetCode { get; set; } = default!;
    public string Status { get; set; } = default!; // enum.ReqStatus(0/1)
    public DateTime StartDate { get; set; }
    public Guid PositionId { get; set; } // Cor.HRMM.Position
    public Guid JgStepId { get; set; } // Cor.HRMM.JgStep
    public Guid WorkforcePlanId { get; set; } // WorkforcePlan
    public Guid JobDecId { get; set; } // JobDec

    //******************************************//

    public virtual WorkforcePlan WorkforcePlan { get; set; } = null!;
    public virtual JobDec JobDec { get; set; } = null!;
    public List<JobReqReview> Reviews { get; set; } = [];
}