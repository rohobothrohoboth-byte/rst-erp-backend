using Cor.Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Services;

/// <summary>
/// Single authoritative accounting calculation source for ledger-based reports.
/// Reports consume this service; the UI does not rebuild accounting balances.
/// </summary>
public sealed class AccountingReportService
{
    private readonly FinanceDbContext _context;

    public AccountingReportService(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<AccountLedgerSnapshot>> GetAccountSnapshotsAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? branchId,
        Guid? accountId,
        CancellationToken ct)
    {
        var startUtc = NormalizeUtc(startDate).Date;
        var endUtc = NormalizeUtc(endDate).Date.AddDays(1).AddTicks(-1);

        if (endUtc < startUtc)
            throw new ArgumentException("End date must be greater than or equal to start date.");

        var accountsQuery = _context.ChartOfAccounts
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.IsActive);

        if (accountId.HasValue)
            accountsQuery = accountsQuery.Where(x => x.Id == accountId.Value);

        var accounts = await accountsQuery
            .Select(x => new AccountSeed
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                AccountType = x.AccountType,
                NormalBalance = x.NormalBalance,
                OpeningBalance = x.OpeningBalance ?? 0m,
                OpeningBalanceDate = x.OpeningBalanceDate
            })
            .ToListAsync(ct);

        if (accounts.Count == 0)
            return Array.Empty<AccountLedgerSnapshot>();

        var accountIds = accounts.Select(x => x.Id).ToHashSet();

        var entriesQuery = _context.JournalEntries
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.IsPosted && x.EntryDate <= endUtc);

        if (branchId.HasValue)
            entriesQuery = entriesQuery.Where(x => x.BranchId == branchId.Value);

        var entries = await entriesQuery
            .Select(x => new EntrySeed { Id = x.Id, EntryDate = x.EntryDate })
            .ToListAsync(ct);

        var entryDates = entries.ToDictionary(x => x.Id, x => NormalizeUtc(x.EntryDate));
        var entryIds = entries.Select(x => x.Id).ToList();

        var lines = entryIds.Count == 0
            ? new List<LineSeed>()
            : await _context.JournalLines
                .AsNoTracking()
                .Where(x => entryIds.Contains(x.JournalEntryId) && !x.IsDeleted && accountIds.Contains(x.AccountId))
                .Select(x => new LineSeed
                {
                    JournalEntryId = x.JournalEntryId,
                    AccountId = x.AccountId,
                    Direction = x.Direction,
                    Amount = x.Amount
                })
                .ToListAsync(ct);

        var result = new List<AccountLedgerSnapshot>(accounts.Count);

        foreach (var account in accounts)
        {
            var accountLines = lines.Where(x => x.AccountId == account.Id);
            var openingBalance = IsOpeningBalanceEffective(account.OpeningBalanceDate, startUtc)
                ? account.OpeningBalance
                : 0m;

            decimal openingMovement = 0m;
            decimal periodDebit = 0m;
            decimal periodCredit = 0m;
            decimal periodMovement = 0m;

            foreach (var line in accountLines)
            {
                if (!entryDates.TryGetValue(line.JournalEntryId, out var entryDate))
                    continue;

                var signed = GetSignedMovement(account.NormalBalance, line.Direction, line.Amount);

                if (entryDate < startUtc)
                {
                    openingMovement += signed;
                }
                else if (entryDate <= endUtc)
                {
                    periodMovement += signed;

                    if (string.Equals(line.Direction, "Debit", StringComparison.OrdinalIgnoreCase))
                        periodDebit += line.Amount;
                    else if (string.Equals(line.Direction, "Credit", StringComparison.OrdinalIgnoreCase))
                        periodCredit += line.Amount;
                }
            }

            var effectiveOpening = openingBalance + openingMovement;
            var closingBalance = effectiveOpening + periodMovement;

            result.Add(new AccountLedgerSnapshot
            {
                AccountId = account.Id,
                AccountCode = account.Code,
                AccountName = account.Name,
                AccountType = account.AccountType,
                NormalBalance = account.NormalBalance,
                OpeningBalance = effectiveOpening,
                PeriodDebits = periodDebit,
                PeriodCredits = periodCredit,
                ClosingBalance = closingBalance
            });
        }

        return result;
    }

    public static bool IsDebitNormal(string? normalBalance) =>
        string.Equals(normalBalance, "Debit", StringComparison.OrdinalIgnoreCase);

    public static decimal GetSignedMovement(string? normalBalance, string direction, decimal amount)
    {
        var debitNormal = IsDebitNormal(normalBalance);
        var isDebit = string.Equals(direction, "Debit", StringComparison.OrdinalIgnoreCase);

        return debitNormal
            ? (isDebit ? amount : -amount)
            : (isDebit ? -amount : amount);
    }

    public static DateTime NormalizeUtc(DateTime value) =>
        value.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(value, DateTimeKind.Utc)
            : value.ToUniversalTime();

    private static bool IsOpeningBalanceEffective(DateTime? openingBalanceDate, DateTime periodStartUtc) =>
        !openingBalanceDate.HasValue || NormalizeUtc(openingBalanceDate.Value).Date <= periodStartUtc;

    private sealed class AccountSeed
    {
        public Guid Id { get; init; }
        public string Code { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string AccountType { get; init; } = string.Empty;
        public string NormalBalance { get; init; } = "Debit";
        public decimal OpeningBalance { get; init; }
        public DateTime? OpeningBalanceDate { get; init; }
    }

    private sealed class EntrySeed
    {
        public Guid Id { get; init; }
        public DateTime EntryDate { get; init; }
    }

    private sealed class LineSeed
    {
        public Guid JournalEntryId { get; init; }
        public Guid AccountId { get; init; }
        public string Direction { get; init; } = string.Empty;
        public decimal Amount { get; init; }
    }
}

public sealed class AccountLedgerSnapshot
{
    public Guid AccountId { get; init; }
    public string AccountCode { get; init; } = string.Empty;
    public string AccountName { get; init; } = string.Empty;
    public string AccountType { get; init; } = string.Empty;
    public string NormalBalance { get; init; } = "Debit";
    public decimal OpeningBalance { get; init; }
    public decimal PeriodDebits { get; init; }
    public decimal PeriodCredits { get; init; }
    public decimal ClosingBalance { get; init; }
}
