namespace Cor.HRMM.Models.Entities;

public class JgStep: BaseEntity
{
    public string Name { get; set; } = default!;
    public double Salary { get; set; } = default!;
    public Guid JobGradeId { get; set; } = default!;

    //******************************************//

    public JobGrade JobGrade { get; set; } = null!;
}