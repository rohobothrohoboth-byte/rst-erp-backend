namespace Profile.Domain.Entities;

public class EmpSalary : BaseEntity
{
    public double BaseSalary { get; set; } = default!;
    //public bool IsCurrent { get; set; } = true;
    public string Currency { get; set; } = default!;
    public string SalaryPayFreq { get; set; } = default!;
    public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow;
    public DateTime? EffectiveTo { get; set; }
    public Guid EmployeeId { get; set; } = default!;
    public Guid JgStepId { get; set; } = default!; //Cor.HRMM.JgStep

    //******************************************//

    public Employee Employee { get; set; } = null!;
}