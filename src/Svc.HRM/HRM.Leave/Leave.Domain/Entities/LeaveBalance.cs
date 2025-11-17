namespace Leave.Domain.Entities;

public class LeaveBalance : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public Guid FiscalYearId { get; set; }
    public Guid LeavePolicyId { get; set; }
    public double Balance { get; set; }
    public double Carried { get; set; }
    public DateTime? CarriedExpireDate { get; set; }

    //******************************************//

    public LeavePolicy LeavePolicy { get; set; } = null!;
}