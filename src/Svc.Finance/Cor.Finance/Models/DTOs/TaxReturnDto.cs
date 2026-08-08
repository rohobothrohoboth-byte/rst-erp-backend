// Models/DTOs/TaxReturnDto.cs
namespace Cor.Finance.Models.DTOs;

public class TaxReturnDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? TaxType { get; set; }
    public string? Period { get; set; }
    public string? FiscalYear { get; set; }
    public DateTime? FilingDate { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal TaxableAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal BalanceDue { get; set; }
    public string? Status { get; set; }
    public string? FiledBy { get; set; }
    public string? Notes { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public string? RowVersion { get; set; }
}

public class AddTaxReturnDto
{
    public string Code { get; set; } = string.Empty;
    public string? TaxType { get; set; }
    public string? Period { get; set; }
    public string? FiscalYear { get; set; }
    public DateTime? FilingDate { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal TaxableAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal BalanceDue { get; set; }
    public string? Status { get; set; }
    public string? FiledBy { get; set; }
    public string? Notes { get; set; }
}

public class EditTaxReturnDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? TaxType { get; set; }
    public string? Period { get; set; }
    public string? FiscalYear { get; set; }
    public DateTime? FilingDate { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal TaxableAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal BalanceDue { get; set; }
    public string? Status { get; set; }
    public string? FiledBy { get; set; }
    public string? Notes { get; set; }
    public string? RowVersion { get; set; }
}