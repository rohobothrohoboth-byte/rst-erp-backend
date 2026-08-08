// Models/DTOs/TrialBalanceDto.cs
namespace Cor.Finance.Models.DTOs;

public class TrialBalanceDto
{
    public DateTime AsOfDate { get; set; }
    public List<TrialBalanceLineDto> Lines { get; set; } = new();
    public decimal TotalDebits { get; set; }
    public decimal TotalCredits { get; set; }
    public decimal Difference { get; set; }
    public bool IsBalanced { get; set; }
}

public class TrialBalanceLineDto
{
    public string AccountId { get; set; } = string.Empty;
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public decimal Balance { get; set; }
}