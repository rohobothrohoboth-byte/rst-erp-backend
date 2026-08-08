namespace Recruit.Domain.Entities;

public class JobReqReview : BaseEntity
{
    public string Comment { get; set; } = default!;
    public int ReqQuantity { get; set; }
    public int AppQuantity { get; set; }
    public string Status { get; set; } = default!; // enum.ReqStatus(0/1)
    public Guid ReviewById { get; set; } // Cor.HRMM.Employee
    public Guid JobReqId { get; set; } // JobRequisition

    //******************************************//

    public virtual JobRequisition JobReq { get; set; } = null!;
}