using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.Finance.Models.Entities;


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

    public GetGeneralLedgerHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<GeneralLedgerDto> Handle(GetGeneralLedgerQry request, CancellationToken ct)
    {
        var startDateUtc = EnsureUtc(request.StartDate);
        var endDateUtc = EnsureUtc(request.EndDate).Date.AddDays(1).AddTicks(-1);

        // Get the account if specified
        ChartOfAccounts? account = null;
        if (request.AccountId.HasValue)
        {
            account = await _context.ChartOfAccounts
                .FirstOrDefaultAsync(x => x.Id == request.AccountId.Value && !x.IsDeleted, ct);
        }

        // Get journal entries in the date range
        var query = _context.JournalEntries
            .Where(x => !x.IsDeleted && x.IsPosted && x.EntryDate >= startDateUtc && x.EntryDate <= endDateUtc);

        if (request.BranchId.HasValue)
        {
            query = query.Where(x => x.BranchId == request.BranchId.Value);
        }

        var entries = await query
            .OrderBy(x => x.EntryDate)
            .ToListAsync(ct);

        var entryIds = entries.Select(e => e.Id).ToList();

        // Get journal lines - fix the type issue
        var linesQuery = _context.JournalLines
            .Where(x => entryIds.Contains(x.JournalEntryId) && !x.IsDeleted);

        if (request.AccountId.HasValue)
        {
            linesQuery = linesQuery.Where(x => x.AccountId == request.AccountId.Value);
        }

        var lines = await linesQuery
            .Include(x => x.Account) // Add Include here instead
            .ToListAsync(ct);

        // Get opening balance
        var openingBalance = await GetOpeningBalance(startDateUtc, account?.Id, ct);

        var entriesList = new List<GeneralLedgerEntryDto>();
        var runningBalance = openingBalance;

        foreach (var entry in entries)
        {
            var entryLines = lines.Where(x => x.JournalEntryId == entry.Id).ToList();
            var totalDebit = entryLines.Sum(x => x.Direction == "Debit" ? x.Amount : 0);
            var totalCredit = entryLines.Sum(x => x.Direction == "Credit" ? x.Amount : 0);

            // Calculate balance based on account type
            if (account != null)
            {
                if (account.AccountType == "Asset" || account.AccountType == "Expense")
                {
                    runningBalance += totalDebit - totalCredit;
                }
                else
                {
                    runningBalance += totalCredit - totalDebit;
                }
            }
            else
            {
                // For all accounts, show net change
                runningBalance += totalDebit - totalCredit;
            }

            entriesList.Add(new GeneralLedgerEntryDto
            {
                Date = entry.EntryDate,
                Reference = entry.Reference,
                Description = entry.Description,
                AccountCode = account?.Code ?? "All Accounts",
                AccountName = account?.Name ?? "All Accounts",
                Debit = totalDebit,
                Credit = totalCredit,
                Balance = runningBalance
            });
        }

        return new GeneralLedgerDto
        {
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Entries = entriesList,
            OpeningBalance = openingBalance,
            ClosingBalance = runningBalance,
            TotalDebits = entriesList.Sum(x => x.Debit),
            TotalCredits = entriesList.Sum(x => x.Credit)
        };
    }

    private async Task<decimal> GetOpeningBalance(DateTime startDateUtc, Guid? accountId, CancellationToken ct)
    {
        // Get all journal entries before the start date
        var query = _context.JournalEntries
            .Where(x => !x.IsDeleted && x.IsPosted && x.EntryDate < startDateUtc);

        var entries = await query.ToListAsync(ct);
        var entryIds = entries.Select(e => e.Id).ToList();

        var linesQuery = _context.JournalLines
            .Where(x => entryIds.Contains(x.JournalEntryId) && !x.IsDeleted);

        if (accountId.HasValue)
        {
            linesQuery = linesQuery.Where(x => x.AccountId == accountId.Value);
        }

        var lines = await linesQuery
            .Include(x => x.Account) // Add Include here
            .ToListAsync(ct);

        // If specific account, get its opening balance
        if (accountId.HasValue)
        {
            var account = await _context.ChartOfAccounts
                .FirstOrDefaultAsync(x => x.Id == accountId.Value && !x.IsDeleted, ct);

            if (account != null)
            {
                var totalDebit = lines.Where(x => x.Direction == "Debit").Sum(x => x.Amount);
                var totalCredit = lines.Where(x => x.Direction == "Credit").Sum(x => x.Amount);
                var openingBalance = account.OpeningBalance ?? 0;

                if (account.AccountType == "Asset" || account.AccountType == "Expense")
                {
                    return openingBalance + totalDebit - totalCredit;
                }
                else
                {
                    return openingBalance + totalCredit - totalDebit;
                }
            }
        }

        // For all accounts, return sum of all balances
        var debitTotal = lines.Where(x => x.Direction == "Debit").Sum(x => x.Amount);
        var creditTotal = lines.Where(x => x.Direction == "Credit").Sum(x => x.Amount);
        return debitTotal - creditTotal;
    }

    private static DateTime EnsureUtc(DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Unspecified)
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        return dateTime.ToUniversalTime();
    }
}