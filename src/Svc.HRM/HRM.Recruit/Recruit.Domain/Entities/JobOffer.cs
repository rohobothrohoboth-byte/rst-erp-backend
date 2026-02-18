namespace Recruit.Domain.Entities;

public class JobOffer : BaseEntity
{
    public string OfferNumber { get; set; } = default!;
    public string Status { get; set; } = default!; // enum.OfferStatus(0/1)
    public DateTime OfferDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string OfferDocument { get; set; } = default!;
    public Guid JobApplicationId { get; set; } // JobApplication
    public Guid JobPostingId { get; set; } // JobPosting

    //public bool IsExpired => DateTime.UtcNow > ExpirationDate;

    //******************************************//

    public virtual JobPosting JobPosting { get; set; } = null!;
    public virtual JobApplication JobApplication { get; set; } = null!;
}