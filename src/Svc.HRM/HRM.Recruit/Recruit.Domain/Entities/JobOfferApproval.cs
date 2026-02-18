namespace Recruit.Domain.Entities;

public class JobOfferApproval : BaseEntity
{
    public int StepOrder { get; set; }
    public string Role { get; set; } = default!; // Dept/HR/Finance
    public string Status { get; set; } = default!;
    public DateTime? ApprovedDate { get; set; }
    public Guid? ApprovedById { get; set; } // HRM.Profile.Employee
    public Guid JobOfferId { get; set; } // JobOffer

    //******************************************//

    public JobOffer JobOffer { get; set; } = null!;
}