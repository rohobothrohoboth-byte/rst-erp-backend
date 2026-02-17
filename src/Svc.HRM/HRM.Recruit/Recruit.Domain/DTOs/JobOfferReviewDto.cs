namespace Recruit.Domain.DTOs;

public class JobOfferReviewListDto : BaseDto
{
    public DateTime? AcceptanceDate { get; set; }
    public DateTime? RejectionDate { get; set; }
    public string? RejectionReason { get; set; }
    public string? ApprovalComments { get; set; }
    public Guid JobOfferId { get; set; } // JobOffer
}

public class JobOfferReviewAddDto
{
    public DateTime? AcceptanceDate { get; set; }
    public DateTime? RejectionDate { get; set; }
    public string? RejectionReason { get; set; }
    public string? ApprovalComments { get; set; }
    public Guid JobOfferId { get; set; } // JobOffer
}

public class JobOfferReviewModDto
{
    public Guid Id { get; set; }
    public DateTime? AcceptanceDate { get; set; }
    public DateTime? RejectionDate { get; set; }
    public string? RejectionReason { get; set; }
    public string? ApprovalComments { get; set; }
    public Guid JobOfferId { get; set; } // JobOffer
    public string RowVersion { get; set; } = default!;
}