namespace Svc.HRM.Payroll.Models.DTOs;

public class SalaryStructureDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public decimal BaseSalary { get; set; }
    public decimal HousingAllowance { get; set; }
    public decimal TransportAllowance { get; set; }
    public decimal MealAllowance { get; set; }
    public decimal MedicalAllowance { get; set; }
    public decimal OtherAllowances { get; set; }
    public decimal Deductions { get; set; }
    public decimal PensionContribution { get; set; }
    public decimal TotalSalary => BaseSalary + HousingAllowance + TransportAllowance +
                                   MealAllowance + MedicalAllowance + OtherAllowances -
                                   Deductions - PensionContribution;
    public bool IsActive { get; set; }
}

public class SalaryStructureCreateDto
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public decimal BaseSalary { get; set; }
    public decimal HousingAllowance { get; set; }
    public decimal TransportAllowance { get; set; }
    public decimal MealAllowance { get; set; }
    public decimal MedicalAllowance { get; set; }
    public decimal OtherAllowances { get; set; }
    public decimal Deductions { get; set; }
    public decimal PensionContribution { get; set; }
}