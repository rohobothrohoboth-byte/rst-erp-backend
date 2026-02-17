namespace Recruit.Domain.Entities;

public class WorkforcePlanReview : BaseEntity
{
    public string Comment { get; set; } = default!;
    public int ReqPositions { get; set; }
    public int AppPositions { get; set; } = 0;
    public string Status { get; set; } = default!; // enum.ReqStatus(0/1)
    public Guid ReviewById { get; set; } // HRM.Profile.Employee
    public Guid WorkforcePlanId { get; set; } // WorkforcePlan

    //******************************************//

    public WorkforcePlan WorkforcePlan { get; set; } = null!;
}