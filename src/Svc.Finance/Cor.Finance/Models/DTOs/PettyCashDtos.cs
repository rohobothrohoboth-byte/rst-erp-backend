// Cor.Finance/Models/DTOs/PettyCashDtos.cs
namespace Cor.Finance.Models.DTOs;

public class PettyCashDto
{
    public Guid Id { get; set; }
    public decimal Balance { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal TotalReplenishments { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
     public object? PeriodInfo { get; set; }
      public Guid? PeriodId { get; set; }
         public string? PeriodName { get; set; }
}

public class AddPettyCashTransactionDto
{
    public DateTime TransactionDate { get; set; }
    public string Description { get; set; } = default!;
    public decimal Amount { get; set; }
    public string TransactionType { get; set; } = default!; // Expense, Replenishment, Withdrawal, Deposit
    public string Category { get; set; } = default!;
    public string? ReceiptUrl { get; set; }
    public Guid? EmployeeId { get; set; }
      public Guid? PeriodId { get; set; }
}

public class ReplenishPettyCashDto
{
    public decimal Amount { get; set; }
    public string Description { get; set; } = default!;
    public DateTime? TransactionDate { get; set; }
    public string? ApprovedBy { get; set; }
}