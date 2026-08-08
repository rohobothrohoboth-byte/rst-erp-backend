namespace Profile.Domain.Entities;

public class EmpExperience : BaseEntity
{
    public string Company { get; set; } = default!;
    public string PosTitle { get; set; } = default!;
    public string Location { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Respo { get; set; } = "";
    public string Status { get; set; } = default!; // enum.ApprovalStatus
    public Guid EmployeeId { get; set; } = default!; //Person

    //******************************************//

    public Employee Employee { get; set; } = null!;
}