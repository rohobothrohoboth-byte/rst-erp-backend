namespace Cor.Finance.Models.DTOs;

public class GeneralLedgerDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid? PeriodId { get; set; }
    public string? PeriodName { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? AccountId { get; set; }
    public List<GeneralLedgerEntryDto> Entries { get; set; } = new();
    public decimal OpeningBalance { get; set; }
    public decimal ClosingBalance { get; set; }
    public decimal TotalDebits { get; set; }
    public decimal TotalCredits { get; set; }
}

public class GeneralLedgerEntryDto
{
    public DateTime Date { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public decimal Balance { get; set; }
}
