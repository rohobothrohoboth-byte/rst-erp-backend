using Microsoft.Extensions.Options;
using Svc.HRM.Payroll.Models.Entities;

namespace Svc.HRM.Payroll.Services;

public class PayrollFinanceAccountsOptions
{
    public const string SectionName = "PayrollFinanceAccounts";

    /// <summary>Debit: salary/expense (gross pay).</summary>
    public string SalaryExpenseCode { get; set; } = "5100";

    /// <summary>Credit: tax payable.</summary>
    public string TaxPayableCode { get; set; } = "2200";

    /// <summary>Credit: pension payable.</summary>
    public string PensionPayableCode { get; set; } = "2210";

    /// <summary>Credit: other deductions payable.</summary>
    public string OtherDeductionsPayableCode { get; set; } = "2220";

    /// <summary>Credit: net wages payable.</summary>
    public string NetPayPayableCode { get; set; } = "2100";

    /// <summary>When true, auto-post journal after create.</summary>
    public bool AutoPost { get; set; } = true;

    /// <summary>When false, approval continues even if Finance posting fails.</summary>
    public bool FailApprovalOnPostingError { get; set; } = false;
}

public interface IPayrollFinancePoster
{
    Task PostPayrollRunAsync(LocalPayrollRun run, string approvedBy, CancellationToken ct = default);
}

public class PayrollFinancePoster : IPayrollFinancePoster
{
    private readonly IFinanceApiService _finance;
    private readonly PayrollFinanceAccountsOptions _options;
    private readonly ILogger<PayrollFinancePoster> _logger;

    public PayrollFinancePoster(
        IFinanceApiService finance,
        IOptions<PayrollFinanceAccountsOptions> options,
        ILogger<PayrollFinancePoster> logger)
    {
        _finance = finance;
        _options = options.Value;
        _logger = logger;
    }

    public async Task PostPayrollRunAsync(LocalPayrollRun run, string approvedBy, CancellationToken ct = default)
    {
        var reference = $"PR-{run.Id:N}";

        // Idempotency: skip if already posted or journal exists
        if (run.FinanceJournalEntryId.HasValue && run.FinanceJournalEntryId != Guid.Empty)
        {
            _logger.LogInformation("Payroll run {Id} already linked to journal {JournalId}", run.Id, run.FinanceJournalEntryId);
            return;
        }

        var existing = await _finance.GetJournalByReferenceAsync(reference, ct);
        if (existing != null)
        {
            run.FinanceJournalEntryId = existing.Id;
            run.FinancePostedAt = DateTime.UtcNow;
            run.FinancePostingStatus = existing.IsPosted ? "Posted" : "Created";
            return;
        }

        var period = await _finance.GetActivePeriodAsync(run.PaymentDate, ct)
            ?? throw new InvalidOperationException($"No active Finance period for payment date {run.PaymentDate:yyyy-MM-dd}.");

        var salaryAcc = await RequireAccount(_options.SalaryExpenseCode, ct);
        var taxAcc = await RequireAccount(_options.TaxPayableCode, ct);
        var pensionAcc = await RequireAccount(_options.PensionPayableCode, ct);
        var otherAcc = await RequireAccount(_options.OtherDeductionsPayableCode, ct);
        var netAcc = await RequireAccount(_options.NetPayPayableCode, ct);

        var pensionTotal = run.PayrollEmployees?.Sum(e => e.PensionContribution) ?? 0m;
        var otherDeductions = Math.Max(0, run.TotalDeductions - run.TotalTaxes - pensionTotal);

        // Balance check: Gross = Tax + Pension + Other + Net
        var creditTotal = run.TotalTaxes + pensionTotal + otherDeductions + run.TotalNetPay;
        if (creditTotal != run.TotalGrossPay)
        {
            // Adjust other deductions so journal balances (rounding / mapping differences)
            otherDeductions = run.TotalGrossPay - run.TotalTaxes - pensionTotal - run.TotalNetPay;
            if (otherDeductions < 0)
                throw new InvalidOperationException(
                    $"Payroll totals do not balance for GL posting. Gross={run.TotalGrossPay}, Credits components exceed gross.");
        }

        var lines = new List<FinanceJournalLineRequest>
        {
            new()
            {
                AccountId = salaryAcc.Id,
                Direction = "Debit",
                Amount = run.TotalGrossPay,
                Description = $"Salary expense - {run.Name}"
            }
        };

        void AddCredit(FinanceAccountInfo acc, decimal amount, string desc)
        {
            if (amount <= 0) return;
            lines.Add(new FinanceJournalLineRequest
            {
                AccountId = acc.Id,
                Direction = "Credit",
                Amount = amount,
                Description = desc
            });
        }

        AddCredit(taxAcc, run.TotalTaxes, $"Tax payable - {run.Name}");
        AddCredit(pensionAcc, pensionTotal, $"Pension payable - {run.Name}");
        AddCredit(otherAcc, otherDeductions, $"Other deductions - {run.Name}");
        AddCredit(netAcc, run.TotalNetPay, $"Net wages payable - {run.Name}");

        var created = await _finance.CreateJournalEntryAsync(new FinanceJournalCreateRequest
        {
            Reference = reference,
            EntryDate = run.PaymentDate,
            Description = $"Payroll run {run.Name} ({run.PayPeriodStart:yyyy-MM-dd} - {run.PayPeriodEnd:yyyy-MM-dd})",
            EntryType = "Payroll",
            PeriodId = period.Id,
            CreatedByUserName = approvedBy,
            Lines = lines
        }, ct) ?? throw new InvalidOperationException("Finance returned empty journal create response.");

        run.FinanceJournalEntryId = created.Id;
        run.FinancePostingStatus = "Created";
        run.FinancePostedAt = DateTime.UtcNow;

        if (_options.AutoPost)
        {
            var posted = await _finance.PostJournalEntryAsync(created.Id, ct);
            run.FinancePostingStatus = posted ? "Posted" : "Created";
            if (posted) run.FinancePostedAt = DateTime.UtcNow;
        }
    }

    private async Task<FinanceAccountInfo> RequireAccount(string code, CancellationToken ct)
    {
        var acc = await _finance.GetAccountByCodeAsync(code, ct);
        if (acc == null || acc.Id == Guid.Empty)
            throw new InvalidOperationException($"Finance COA account code '{code}' not found. Configure PayrollFinanceAccounts.");
        return acc;
    }
}
