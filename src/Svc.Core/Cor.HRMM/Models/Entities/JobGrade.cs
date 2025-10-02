namespace Cor.HRMM.Models.Entities;

public class JobGrade: BaseEntity
{
    public string Name { get; set; } = default!;
    public decimal StartSalary { get; set; } = default!;
    public decimal MaxSalary { get; set; } = default!;
}
