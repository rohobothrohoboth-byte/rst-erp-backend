using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Queries;

public class GetTrialBalanceQry : IRequest<TrialBalanceDto>
{
    public DateTime AsOfDate { get; set; }
    public Guid? BranchId { get; set; }
}

public class GetTrialBalanceHandler : IRequestHandler<GetTrialBalanceQry, TrialBalanceDto>
{
    private readonly FinanceDbContext _context;

    public GetTrialBalanceHandler(FinanceDbContext context) => _context = context;

    public async Task<TrialBalanceDto> Handle(GetTrialBalanceQry request, CancellationToken ct)
    {
        var asOfDateUtc = EnsureUtc(request.AsOfDate).Date;
        var endDateUtc = asOfDateUtc.AddDays(1).AddTicks(-1);

        var accounts = await _context.ChartOfAccounts
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.IsActive)
            .Select(x => new { x.Id, x.Code, x.Name, x.AccountType, x.OpeningBalance })
            .ToListAsync(ct);

        var entriesQuery = _context.JournalEntries
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.IsPosted && x.EntryDate <= endDateUtc);

        if (request.BranchId.HasValue)
            entriesQuery = entriesQuery.Where(x => x.BranchId == request.BranchId.Value);

        var entryIds = await entriesQuery.Select(x => x.Id).ToListAsync(ct);

        var lines = entryIds.Count == 0
            ? new List<TrialBalanceSourceLine>()
            : await _context.JournalLines
                .AsNoTracking()
                .Where(x => entryIds.Contains(x.JournalEntryId) && !x.IsDeleted)
                .Select(x => new TrialBalanceSourceLine
                {
                    AccountId = x.AccountId,
                    Direction = x.Direction,
                    Amount = x.Amount
                })
                .ToListAsync(ct);

        var trialBalanceLines = new List<TrialBalanceLineDto>();

        foreach (var account in accounts)
        {
            var accountLines = lines.Where(x => x.AccountId == account.Id).ToList();
            var debit = accountLines.Where(x => x.Direction == "Debit").Sum(x => x.Amount);
            var credit = accountLines.Where(x => x.Direction == "Credit").Sum(x => x.Amount);
            var opening = account.OpeningBalance ?? 0m;
            var debitNormal = IsDebitNormal(account.AccountType);

            var reportedDebit = debit + (debitNormal ? opening : 0m);
            var reportedCredit = credit + (!debitNormal ? opening : 0m);
            var balance = debitNormal
                ? reportedDebit - reportedCredit
                : reportedCredit - reportedDebit;

            if (reportedDebit == 0m && reportedCredit == 0m)
                continue;

            trialBalanceLines.Add(new TrialBalanceLineDto
            {
                AccountId = account.Id.ToString(),
                AccountCode = account.Code,
                AccountName = account.Name,
                AccountType = account.AccountType,
                Debit = reportedDebit,
                Credit = reportedCredit,
                Balance = balance
            });
        }

        var totalDebits = trialBalanceLines.Sum(x => x.Debit);
        var totalCredits = trialBalanceLines.Sum(x => x.Credit);
        var difference = totalDebits - totalCredits;

        return new TrialBalanceDto
        {
            AsOfDate = request.AsOfDate,
            Lines = trialBalanceLines,
            TotalDebits = totalDebits,
            TotalCredits = totalCredits,
            Difference = difference,
            IsBalanced = difference == 0m
        };
    }

    private static bool IsDebitNormal(string? accountType) =>
        string.Equals(accountType, "Asset", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(accountType, "Expense", StringComparison.OrdinalIgnoreCase);

    private static DateTime EnsureUtc(DateTime value) =>
        value.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(value, DateTimeKind.Utc)
            : value.ToUniversalTime();

    private sealed class TrialBalanceSourceLine
    {
        public Guid AccountId { get; init; }
        public string Direction { get; init; } = string.Empty;
        public decimal Amount { get; init; }
    }
}
