namespace Cor.Finance.Models.DTOs;

public class BudgetAvailabilityDto
{
    public Guid BudgetId { get; set; }
    public string BudgetName { get; set; } = "";
    public decimal Allocated { get; set; }   // Budget.TotalAmount
    public decimal Spent { get; set; }        // Budget.SpentAmount
    public decimal Committed { get; set; }    // sum of active reservations
    public decimal Available { get; set; }    // Allocated - Spent - Committed
    public decimal Requested { get; set; }
    public bool Ok { get; set; }              // Available >= Requested
    public string Message { get; set; } = "";
}

public class BudgetReserveRequest
{
    public Guid BudgetId { get; set; }
    public Guid ReferenceId { get; set; }
    public string ReferenceType { get; set; } = "WorkforcePlan";
    public decimal Amount { get; set; }
    public string? Note { get; set; }
}

public class BudgetReleaseRequest
{
    public Guid ReferenceId { get; set; }
    public string ReferenceType { get; set; } = "WorkforcePlan";
}

public class BudgetConsumeRequest
{
    public Guid ReferenceId { get; set; }
    public string ReferenceType { get; set; } = "WorkforcePlan";
    public decimal? Amount { get; set; } // if null, consume the full reserved amount
}
