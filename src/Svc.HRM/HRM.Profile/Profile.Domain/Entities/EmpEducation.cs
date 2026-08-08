namespace Profile.Domain.Entities;

public class EmpEducation : BaseEntity
{
    public string Institution { get; set; } = default!;
    public string FieldOfStudy { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double? GPA { get; set; }
    public string EduLevel { get; set; } = default!; //enum.EducationLevel
    public string Status { get; set; } = default!; // enum.ApprovalStatus
    public Guid EmployeeId { get; set; } = default!; //Person

    //******************************************//

    public Employee Employee { get; set; } = null!;
}