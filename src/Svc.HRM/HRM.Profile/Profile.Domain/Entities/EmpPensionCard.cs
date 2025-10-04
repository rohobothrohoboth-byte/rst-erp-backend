namespace Profile.Domain.Entities;

public class EmpPensionCard : BaseEntity
{
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    public DateTime? SentDate { get; set; }
    public DateTime? ReceivedDate { get; set; }
    public string IsReceived { get; set; } = default!; //enum.YesNo
    public string IsSent { get; set; } = default!; //enum.YesNo
    public Guid EmployeeId { get; set; } = default!; //Person

    //******************************************//

    public Employee Employee { get; set; } = null!;
}