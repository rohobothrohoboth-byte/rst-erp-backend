using System.Text.Json.Serialization;

namespace Cor.HRMM.Models.DTOs;

public class JgStepListDto : BaseDto
{
    [JsonIgnore]
    public string Currency { get; set; } = default!; // enum.Currency
    [JsonIgnore]
    public string SalaryPayFreq { get; set; } = default!; // enum.SalaryPayFreq
    [JsonIgnore]
    public double Salary { get; set; } = default!;

    public Guid JobGradeId { get; set; } = default!; // JobGrade
    public string Name { get; set; } = default!;
    public string SalaryStr { get; set; } = default!;
    public string CurrencyStr { get; set; } = default!;
    public string SalaryPayFreqStr { get; set; } = default!;
    public string JobGrade { get; set; } = default!;
}

public class JgStepAddDto
{
    public string Name { get; set; } = default!;
    public double Salary { get; set; } = default!;
    public string Currency { get; set; } = default!; // enum.Currency
    public string SalaryPayFreq { get; set; } = default!; // enum.SalaryPayFreq
    public Guid JobGradeId { get; set; } = default!; // JobGrade
}

public class JgStepModDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public double Salary { get; set; } = default!;
    public string Currency { get; set; } = default!; // enum.Currency
    public string SalaryPayFreq { get; set; } = default!; // enum.SalaryPayFreq
    public Guid JobGradeId { get; set; } = default!; // JobGrade
    public string RowVersion { get; set; } = default!;
}