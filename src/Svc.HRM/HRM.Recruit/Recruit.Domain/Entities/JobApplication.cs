namespace Recruit.Domain.Entities;

public class JobApplication : BaseEntity
{
    public string Status { get; set; } = default!; // enum.ApplicationStatus(0/1)
    public string PostType { get; set; } = default!; // enum.JobPostingType(0/1) 
    public DateTime AppliedDate { get; set; }
    public Guid? ApplicantId { get; set; } // Applicant
    public Guid? EmployeeId { get; set; } // HRM.Profile.Employee
    public Guid JobPostingId { get; set; } // JobPosting

    //******************************************//

    public virtual JobPosting JobPosting { get; set; } = null!;
}