namespace Recruit.Domain.DTOs;

public class JobOfferListDto : BaseDto
{
    public string OfferNumber { get; set; } = default!;
    public string Status { get; set; } = default!; // enum.OfferStatus(0/1)
    public DateTime OfferDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string OfferDocument { get; set; } = default!;
    public Guid? UpdatedById { get; set; } // ApprovalStep
    public Guid CanApplicationId { get; set; } // CandidateApplication
    public Guid JobRequisitionId { get; set; } // JobRequisition

    public bool IsExpired => DateTime.UtcNow > ExpirationDate;
}

public class JobOfferAddDto
{
    public string OfferNumber { get; set; } = default!;
    public string Status { get; set; } = default!; // enum.OfferStatus(0/1)
    public DateTime OfferDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string OfferDocument { get; set; } = default!;
    public Guid? UpdatedById { get; set; } // ApprovalStep
    public Guid CanApplicationId { get; set; } // CandidateApplication
    public Guid JobRequisitionId { get; set; } // JobRequisition
}

public class JobOfferModDto
{
    public Guid Id { get; set; }
    public string OfferNumber { get; set; } = default!;
    public string Status { get; set; } = default!; // enum.OfferStatus(0/1)
    public DateTime OfferDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string OfferDocument { get; set; } = default!;
    public Guid? UpdatedById { get; set; } // ApprovalStep
    public Guid CanApplicationId { get; set; } // CandidateApplication
    public Guid JobRequisitionId { get; set; } // JobRequisition
    public string RowVersion { get; set; } = default!;
}