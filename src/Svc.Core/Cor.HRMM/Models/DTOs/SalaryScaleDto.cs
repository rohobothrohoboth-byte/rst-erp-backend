namespace Cor.HRMM.Models.DTOs;

public class SalaryScaleListDto : BaseDto
{
    public Guid JobGradeId { get; set; } = default!;
    public decimal Salary { get; set; } = default!;
    public string JobGrade { get; set; } = default!;
    public string SalaryStr => $"{Salary:#,##0.##} ETB";
}

public class SalaryScaleAddDto
{
    public Guid JobGradeId { get; set; } = default!;
    public decimal Salary { get; set; } = default!;
}

public class SalaryScaleModDto
{
    public Guid Id { get; set; }
    public Guid JobGradeId { get; set; } = default!;
    public decimal Salary { get; set; } = default!;
    public string RowVersion { get; set; } = default!;
}