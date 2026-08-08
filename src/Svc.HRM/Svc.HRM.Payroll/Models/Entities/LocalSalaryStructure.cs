namespace Svc.HRM.Payroll.Models.Entities;

public class LocalSalaryStructure
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
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
    public bool IsActive { get; set; } = true;
    public DateTime DateAdd { get; set; } = DateTime.UtcNow;
    public DateTime? DateMod { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime SyncedAt { get; set; } = DateTime.UtcNow;
}