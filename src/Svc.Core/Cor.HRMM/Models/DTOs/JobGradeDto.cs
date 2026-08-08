
namespace Cor.HRMM.Models.DTOs;

public class JobGradeListDto : BaseDto
{
    public string Name { get; set; } = default!;
    public double StartSalary { get; set; } = default!;
    public double MaxSalary { get; set; } = default!;
}

public class JobGradeAddDto
{
    public string Name { get; set; } = default!;
    public double StartSalary { get; set; } = default!;
    public double MaxSalary { get; set; } = default!;
}

public class JobGradeModDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public double StartSalary { get; set; } = default!;
    public double MaxSalary { get; set; } = default!;
    public string RowVersion { get; set; } = default!;
}
