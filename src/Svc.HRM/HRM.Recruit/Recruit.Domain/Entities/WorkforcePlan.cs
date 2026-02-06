namespace Recruit.Domain.Entities;

public class WorkforcePlan : BaseEntity
{
    public string PlanCode { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalPositions { get; set; }
    public int ApprovedPositions { get; set; } = 0;
    public string Status { get; set; } = default!; // enum.RequisitionStatus(0/1)
    public Guid DepartmentId { get; set; } // Cor.Module.Department
    public Guid? PeriodId { get; set; } // Cor.Module.Period (or FiscalYear)
    public Guid RequistionById { get; set; } // HRM.Profile.Employee

    //******************************************//

    public List<JobRequisition> JobRequisitions { get; set; } = new();
}