namespace Cor.HRMM.Models.Entities;

public class SalaryScale: BaseEntity
{
    public Guid JobGradeId { get; set; } = default!;
    public decimal Salary { get; set; } = default!;

    //******************************************//

    public JobGrade JobGrade { get; set; } = null!;
}
