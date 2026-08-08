using System.Text.Json.Serialization;

namespace Cor.HRMM.Models.DTOs;

public class JgStepListDto : BaseDto
{
    // ? REMOVE [JsonIgnore] - These need to be sent to the frontend
    public string Currency { get; set; } = "ETB"; // enum.Currency
    public string SalaryPayFreq { get; set; } = "Mthly"; // enum.SalaryPayFreq
    public double Salary { get; set; } // REMOVE default! for value types

    public Guid JobGradeId { get; set; }
    public string Name { get; set; } = string.Empty;

    // ? These are computed/display fields - keep them
    public string SalaryStr { get; set; } = string.Empty;
    public string CurrencyStr { get; set; } = string.Empty;
    public string SalaryPayFreqStr { get; set; } = string.Empty;
    public string JobGrade { get; set; } = string.Empty;
}

public class JgStepAddDto
{
    public string Name { get; set; } = string.Empty;
    public double Salary { get; set; }
    public string Currency { get; set; } = "ETB"; // enum.Currency
    public string SalaryPayFreq { get; set; } = "Mthly"; // enum.SalaryPayFreq
    public Guid JobGradeId { get; set; }
}

public class JgStepModDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Salary { get; set; }
    public string Currency { get; set; } = "ETB"; // enum.Currency
    public string SalaryPayFreq { get; set; } = "Mthly"; // enum.SalaryPayFreq
    public Guid JobGradeId { get; set; }
    public string RowVersion { get; set; } = string.Empty;
}
