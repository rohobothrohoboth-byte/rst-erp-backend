namespace Recruit.Domain.Entities;

public class JobOfferReview : BaseEntity
{
    public DateTime? AcceptanceDate { get;  set; }
    public DateTime? RejectionDate { get;  set; }
    public string? RejectionReason { get;  set; }
    public string? ApprovalComments { get; set; }
    public Guid JobOfferId { get; set; } // JobOffer

    //******************************************//

    public JobOffer JobOffer { get; set; } = null!;
}