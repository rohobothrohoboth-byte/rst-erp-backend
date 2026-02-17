namespace Recruit.Domain.Entities;

public class JobAppScreening : BaseEntity
{
    public double Score { get; set; }
    public string Comments { get; set; } = default!;
    public Guid ScreeningById { get; set; } // HRM.Profile.Employee
    public Guid JobAppId { get; set; } // JobApplication

    //******************************************//

    public JobApplication JobApp { get; set; } = null!;
}