// Recruit.Domain/DTOs/WorkforcePlanDtos.cs

using System.Text.Json.Serialization;
using EthiopianCalendar;

namespace Recruit.Domain.DTOs;

public class WorkforcePlanListDto : BaseDto
{
    [JsonIgnore]
    public string Status { get; set; } = default!; // enum.ReqStatus(0/1)
    [JsonIgnore]
    public Guid DepartmentId { get; set; } // Cor.Module.Department
    [JsonIgnore]
    public Guid? PeriodId { get; set; } // Cor.Module.Period (or FiscalYear)
    [JsonIgnore]
    public Guid RequistionById { get; set; } // HRM.Profile.Employee

    public string PlanCode { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Desc { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalPositions { get; set; }
    public int AppPositions { get; set; } = 0;
    public string StatusStr { get; set; } = default!;
    public string Department { get; set; } = default!;
    public string Period { get; set; } = default!;
    public string RequistionBy { get; set; } = default!;

    // ? ADD BUDGET FIELDS
    public decimal? Budget { get; set; }
    public string? BudgetCurrency { get; set; }
    public Guid? BudgetId { get; set; }
    public Guid? PlanDevBudgetId { get; set; }

    // ? Computed properties for formatted display
    public string BudgetFormatted => Budget.HasValue
        ? $"{BudgetCurrency ?? "ETB"} {Budget.Value:N0}"
        : "N/A";
    public string BudgetFormattedWithCurrency => Budget.HasValue
        ? $"{BudgetCurrency ?? "ETB"} {Budget.Value:N2}"
        : "No budget set";
}

public class WorkforcePlanAddDto
{
    public Guid? DepartmentId { get; set; } // optional: if sent, skips the Profile gRPC lookup
    [JsonIgnore]
    public Guid RequistionById { get; set; } // HRM.Profile.Employee

    public string Title { get; set; } = default!;
    public string Desc { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalPositions { get; set; }
    public Guid? PeriodId { get; set; } // Cor.Module.Period (or FiscalYear)

    // ? ADD BUDGET FIELDS
    public decimal? Budget { get; set; }
    public string? BudgetCurrency { get; set; }
    public Guid? BudgetId { get; set; } // Cor.Finance.Budget to encumber against
    public Guid? PlanDevBudgetId { get; set; } // Cor.PlanDev.Budget planned source
}

public class WorkforcePlanModDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Desc { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalPositions { get; set; }
    public string RowVersion { get; set; } = default!;

    // ? ADD BUDGET FIELDS
    public decimal? Budget { get; set; }
    public string? BudgetCurrency { get; set; }
    public Guid? BudgetId { get; set; }
    public Guid? PlanDevBudgetId { get; set; }
}