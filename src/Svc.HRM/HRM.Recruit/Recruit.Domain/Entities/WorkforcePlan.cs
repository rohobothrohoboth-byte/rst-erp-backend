// Recruit.Domain/Entities/WorkforcePlan.cs

using System.Text.Json.Serialization;

namespace Recruit.Domain.Entities;

public static class CurrencyConstants
{
    public const string ETB = "ETB";
    public const string USD = "USD";
    public const string EUR = "EUR";
    public const string GBP = "GBP";

    public static readonly string[] AllCurrencies = { ETB, USD, EUR, GBP };

    public static readonly Dictionary<string, string> CurrencySymbols = new()
    {
        { ETB, "Br" },
        { USD, "$" },
        { EUR, "€" },
        { GBP, "£" }
    };

    public static bool IsValid(string currency) => AllCurrencies.Contains(currency);

    public static string GetSymbol(string currency) =>
        CurrencySymbols.TryGetValue(currency, out var symbol) ? symbol : currency;
}

public class WorkforcePlan : BaseEntity
{
   public string PlanCode { get; set; } = default!; // ? Remove any default value attributes
    public string Title { get; set; } = default!;
    public string Desc { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalPositions { get; set; }
    public int AppPositions { get; set; } = 0;
    public string Status { get; set; } = default!; // enum.ReqStatus(0/1)
    public Guid DepartmentId { get; set; } // Cor.Module.Department
    public Guid? PeriodId { get; set; } // Cor.Module.Period (or FiscalYear)
    public Guid RequistionById { get; set; } // HRM.Profile.Employee

    // ? ADD BUDGET FIELDS
    public decimal? Budget { get; set; }
    public string? BudgetCurrency { get; set; } = CurrencyConstants.ETB;

    //******************************************//

    public List<JobRequisition> JobRequisitions { get; set; } = [];
    public List<WorkforcePlanReview> Reviews { get; set; } = [];
}