namespace Cor.Finance.Models.DTOs;

public class RecurringTransactionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Frequency { get; set; } = default!; // Daily, Weekly, Monthly, Quarterly, Yearly
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? NextRunDate { get; set; }
    public DateTime? LastRunDate { get; set; }
    public string Status { get; set; } = default!; // Active, Paused, Completed
    public decimal Amount { get; set; }
    public string Description { get; set; } = default!;
    public Guid? AccountId { get; set; }
    public string? AccountName { get; set; }
    public Guid? ExpenseCategoryId { get; set; }
    public string? CategoryName { get; set; }
    public Guid? BranchId { get; set; }
    public int RunCount { get; set; }
    public int MaxRuns { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}

public class AddRecurringTransactionDto
{
    public string Name { get; set; } = default!;
    public string Frequency { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = default!;
    public Guid? AccountId { get; set; }
    public Guid? ExpenseCategoryId { get; set; }
    public Guid? BranchId { get; set; }
    public int MaxRuns { get; set; } = 0;
}

public class EditRecurringTransactionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Frequency { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = default!;
    public Guid? AccountId { get; set; }
    public Guid? ExpenseCategoryId { get; set; }
    public Guid? BranchId { get; set; }
    public string Status { get; set; } = default!;
}