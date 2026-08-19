using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Queries;

public class GetGeneralLedgerQry : IRequest<GeneralLedgerDto>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid? AccountId { get; set; }
    public Guid? BranchId { get; set; }
}

public class GetGeneralLedgerHandler : IRequestHandler<GetGeneralLedgerQry, GeneralLedgerDto>
{
    private readonly FinanceDbContext _context;

    public GetGeneralLedgerHandler(FinanceDbContext context) => _context = context;

    public async Task<GeneralLedgerDto> Handle(GetGeneralLedgerQry request, CancellationToken ct)
    {
        var startDateUtc = EnsureUtc(request.StartDate).Date;
        var endDateUtc = EnsureUtc(request.EndDate).Date.AddDays(1).AddTicks(-1);

        ChartOfAccounts? account = null;
        if (request.AccountId.HasValue)
        {
            account = await _context.ChartOfAccounts
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.AccountId.Value && !x.IsDeleted, ct);

            if (account == null)
                throw new KeyNotFoundException($"Account {request.AccountId.Value} was not found.");
        }

        var entriesQuery = _context.JournalEntries
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.IsPosted && x.EntryDate >= startDateUtc && x.EntryDate <= endDateUtc);

        if (request.BranchId.HasValue)
            entriesQuery = entriesQuery.Where(x => x.BranchId == request.BranchId.Value);

        var entries = await entriesQuery
            .OrderBy(x => x.EntryDate)
            .ThenBy(x => x.Reference)
            .ToListAsync(ct);

        var entryIds = entries.Select(x => x.Id).ToList();
        var linesQuery = _context.JournalLines
            .AsNoTracking()
            .Where(x => entryIds.Contains(x.JournalEntryId) && !x.IsDeleted);

        if (request.AccountId.HasValue)
            linesQuery = linesQuery.Where(x => x.AccountId == request.AccountId.Value);

        var lines = await linesQuery
            .Include(x => x.Account)
            .ToListAsync(ct);

        var openingBalances = await GetOpeningBalances(startDateUtc, request.BranchId, request.AccountId, ct);
        var runningBalances = new Dictionary<Guid, decimal>(openingBalances.ByAccount);
        var result = new List<GeneralLedgerEntryDto>();

        foreach (var entry in entries)
        {
            var entryLines = lines
                .Where(x => x.JournalEntryId == entry.Id)
                .GroupBy(x => new { x.AccountId, x.Account!.Code, x.Account.Name, x.Account.AccountType })
                .OrderBy(x => x.Key.Code);

            foreach (var group in entryLines)
            {
                var debit = group.Where(x => x.Direction == "Debit").Sum(x => x.Amount);
                var credit = group.Where(x => x.Direction == "Credit").Sum(x => x.Amount);
                var movement = IsDebitNormal(group.Key.AccountType) ? debit - credit : credit - debit;
                var current = runningBalances.GetValueOrDefault(group.Key.AccountId) + movement;

                runningBalances[group.Key.AccountId] = current;
                result.Add(new GeneralLedgerEntryDto
                {
                    Date = entry.EntryDate,
                    Reference = entry.Reference,
                    Description = entry.Description,
                    AccountCode = group.Key.Code,
                    AccountName = group.Key.Name,
                    Debit = debit,
                    Credit = credit,
                    Balance = current
                });
            }
        }

        return new GeneralLedgerDto
        {
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Entries = result,
            OpeningBalance = openingBalances.Total,
            ClosingBalance = request.AccountId.HasValue
                ? runningBalances.GetValueOrDefault(request.AccountId.Value)
                : runningBalances.Values.Sum(),
            TotalDebits = result.Sum(x => x.Debit),
            TotalCredits = result.Sum(x => x.Credit)
        };
    }

    private async Task<OpeningBalanceResult> GetOpeningBalances(
        DateTime startDateUtc,
        Guid? branchId,
        Guid? accountId,
        CancellationToken ct)
    {
        var entriesQuery = _context.JournalEntries
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.IsPosted && x.EntryDate < startDateUtc);

        if (branchId.HasValue)
            entriesQuery = entriesQuery.Where(x => x.BranchId == branchId.Value);

        var entryIds = await entriesQuery.Select(x => x.Id).ToListAsync(ct);

        var linesQuery = _context.JournalLines
            .AsNoTracking()
            .Where(x => entryIds.Contains(x.JournalEntryId) && !x.IsDeleted);

        if (accountId.HasValue)
            linesQuery = linesQuery.Where(x => x.AccountId == accountId.Value);

        var lines = await linesQuery
            .Include(x => x.Account)
            .ToListAsync(ct);

        var byAccount = lines
            .GroupBy(x => new { x.AccountId, x.Account!.AccountType })
            .ToDictionary(
                g => g.Key.AccountId,
                g => IsDebitNormal(g.Key.AccountType)
                    ? g.Sum(x => x.Direction == "Debit" ? x.Amount : -x.Amount)
                    : g.Sum(x => x.Direction == "Credit" ? x.Amount : -x.Amount));

        var accountsQuery = _context.ChartOfAccounts
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.IsActive);

        if (accountId.HasValue)
            accountsQuery = accountsQuery.Where(x => x.Id == accountId.Value);

        var accounts = await accountsQuery
            .Select(x => new { x.Id, x.OpeningBalance })
            .ToListAsync(ct);

        foreach (var item in accounts)
        {
            var opening = item.OpeningBalance ?? 0m;
            if (opening != 0m || byAccount.ContainsKey(item.Id))
                byAccount[item.Id] = opening + byAccount.GetValueOrDefault(item.Id);
        }

        return new OpeningBalanceResult(byAccount);
    }

    private static bool IsDebitNormal(string? accountType) =>
        string.Equals(accountType, "Asset", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(accountType, "Expense", StringComparison.OrdinalIgnoreCase);

    private static DateTime EnsureUtc(DateTime value) =>
        value.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(value, DateTimeKind.Utc)
            : value.ToUniversalTime();

    private sealed record OpeningBalanceResult(Dictionary<Guid, decimal> ByAccount)
    {
        public decimal Total => ByAccount.Values.Sum();
    }
}
