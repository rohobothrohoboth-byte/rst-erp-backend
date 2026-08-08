namespace Svc.HRM.Payroll.Models.Entities;

public class LocalTaxRate
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Name { get; set; } = default!;
    public decimal MinIncome { get; set; }
    public decimal? MaxIncome { get; set; }
    public decimal TaxRate { get; set; }
    public decimal DeductibleAmount { get; set; }
    public string TaxYear { get; set; } = default!;
    public bool IsActive { get; set; } = true;
    public DateTime DateAdd { get; set; } = DateTime.UtcNow;
    public DateTime? DateMod { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime SyncedAt { get; set; } = DateTime.UtcNow;
}