namespace Cor.HRMM.Models.Entities;

public class JgStep: BaseEntity
{
    public string Name { get; set; } = default!;
    public double Salary { get; set; } = default!;
    public string Currency { get; set; } = default!; // enum.Currency
    public string SalaryPayFreq { get; set; } = default!; // enum.SalaryPayFreq
    public Guid JobGradeId { get; set; } = default!;

    //******************************************//

    public JobGrade JobGrade { get; set; } = null!;
}