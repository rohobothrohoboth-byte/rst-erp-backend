namespace Leave.Domain.Entities;

public class AccrualHistory : BaseEntity
{
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public DateTime ExecutedAt { get; set; }
    public int EmployeeCountProcessed { get; set; }
    public double TotalDaysAccrued { get; set; }
}