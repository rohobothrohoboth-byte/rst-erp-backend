namespace Profile.Domain.Entities;

public class EmployeePromotion : BaseEntity
{
    public Guid EmployeeId { get; set; } = default!;
    public string? FromPosition { get; set; }
    public string ToPosition { get; set; } = default!;
    public DateTime EffectiveDate { get; set; }
    public string? Reason { get; set; }
    public string Status { get; set; } = "Pending";
    public string? ApprovedBy { get; set; }

    //******************************************//

    public Employee Employee { get; set; } = null!;
}
