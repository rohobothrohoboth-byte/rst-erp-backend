namespace Cor.HRMM.Models.DTOs;

public class JgStepListDto : BaseDto
{
    public Guid JobGradeId { get; set; } = default!; // JobGrade
    public string Name { get; set; } = default!;
    public decimal Salary { get; set; } = default!;
    public string JobGrade { get; set; } = default!;
}

public class JgStepAddDto
{
    public string Name { get; set; } = default!;
    public decimal Salary { get; set; } = default!;
    public Guid JobGradeId { get; set; } = default!; // JobGrade
}

public class JgStepModDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Salary { get; set; } = default!;
    public Guid JobGradeId { get; set; } = default!; // JobGrade
    public string RowVersion { get; set; } = default!;
}