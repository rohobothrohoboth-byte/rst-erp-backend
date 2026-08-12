namespace Recruit.Domain.Entities;

public class JobOffer : BaseEntity
{
    public string OfferNumber { get; set;} = default!;
    public string Status { get; set; } = default!; // Draft / Sent / Accepted / Rejected / Expired
    public DateTime OfferDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string OfferDocument { get; set; } = default!;
    public Guid JobApplicationId { get; set; } // JobApplication
    public Guid JobPostingId { get; set; } // JobPosting

    // Compensation / offer details
    public Guid ApplicantId { get; set; } // Applicant (stored for easy querying/enrichment)
    public decimal Salary { get; set; }
    public string Currency { get; set; } = "ETB";
    public string? Benefits { get; set; }
    public DateTime StartDate { get; set; }
    public string? Notes { get; set; }

    //public bool IsExpired => DateTime.UtcNow > ExpirationDate;

    //******************************************//

    public virtual JobPosting JobPosting { get; set; } = null!;
    public virtual JobApplication JobApplication { get; set; } = null!;
}