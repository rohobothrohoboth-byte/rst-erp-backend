namespace Cor.HRMM.Models.Entities;

public class JobGrade: BaseEntity
{
    public string Name { get; set; } = default!;
    public double StartSalary { get; set; } = default!;
    public double MaxSalary { get; set; } = default!;

     public ICollection<JgStep> JgSteps { get; set; } = new List<JgStep>();
        public ICollection<Position> Positions { get; set; } = new List<Position>();
}
