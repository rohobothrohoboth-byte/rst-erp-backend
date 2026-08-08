namespace Cor.Finance.Models.Events;

public class ChartOfAccountsEventData
{
    public Guid Id { get; set; }
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string AccountType { get; set; } = default!;
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public Guid? ParentId { get; set; }
    public int Level { get; set; }
}

public class JournalEntryEventData
{
    public Guid Id { get; set; }
    public string Reference { get; set; } = default!;
    public DateTime EntryDate { get; set; }
    public string Description { get; set; } = default!;
    public string EntryType { get; set; } = default!;
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public bool IsPosted { get; set; }
}

public class InvoiceEventData
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = default!;
    public DateTime InvoiceDate { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public string Status { get; set; } = default!;
    public Guid? CustomerId { get; set; }
    public Guid? BranchId { get; set; }
}

public class PaymentEventData
{
    public Guid Id { get; set; }
    public string PaymentNumber { get; set; } = default!;
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = default!;
    public Guid? InvoiceId { get; set; }
}

public class BudgetEventData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = default!;
}
