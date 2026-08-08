namespace Svc.HRM.Payroll.Models.DTOs;

public class EmployeeSalaryDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = default!;
    public string EmployeeCode { get; set; } = default!;
    public Guid SalaryStructureId { get; set; }
    public string SalaryStructureName { get; set; } = default!;
    public decimal BaseSalary { get; set; }
    public decimal HousingAllowance { get; set; }
    public decimal TransportAllowance { get; set; }
    public decimal MealAllowance { get; set; }
    public decimal MedicalAllowance { get; set; }
    public decimal OtherAllowances { get; set; }
    public decimal Deductions { get; set; }
    public decimal PensionContribution { get; set; }
    public decimal TotalSalary { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
}

public class EmployeeSalaryCreateDto
{
    public Guid EmployeeId { get; set; }
    public Guid SalaryStructureId { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
}