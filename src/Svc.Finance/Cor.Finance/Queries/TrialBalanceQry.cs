using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.Finance.Models.Entities;

namespace Cor.Finance.Queries;

public class GetTrialBalanceQry : IRequest<TrialBalanceDto>
{
    public DateTime AsOfDate { get; set; }
    public Guid? BranchId { get; set; }
}

public class GetTrialBalanceHandler : IRequestHandler<GetTrialBalanceQry, TrialBalanceDto>
{
    private readonly FinanceDbContext _context;

    public GetTrialBalanceHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<TrialBalanceDto> Handle(GetTrialBalanceQry request, CancellationToken ct)
    {
        var asOfDateUtc = EnsureUtc(request.AsOfDate);
        var endDateUtc = asOfDateUtc.Date.AddDays(1).AddTicks(-1);

        // Get all accounts
        var accounts = await _context.ChartOfAccounts
            .Where(x => !x.IsDeleted && x.IsActive)
            .ToListAsync(ct);

        // Get all journal entries up to the date
        var entries = await _context.JournalEntries
            .Where(x => !x.IsDeleted && x.IsPosted && x.EntryDate <= endDateUtc)
            .ToListAsync(ct);

        var entryIds = entries.Select(e => e.Id).ToList();

        // Get all journal lines
        var lines = await _context.JournalLines
            .Where(x => entryIds.Contains(x.JournalEntryId) && !x.IsDeleted)
            .Include(x => x.Account)
            .ToListAsync(ct);

        var trialBalanceLines = new List<TrialBalanceLineDto>();
        decimal totalDebits = 0;
        decimal totalCredits = 0;

        foreach (var account in accounts)
        {
            var accountLines = lines.Where(x => x.AccountId == account.Id).ToList();
            var debit = accountLines.Sum(x => x.Direction == "Debit" ? x.Amount : 0);
            var credit = accountLines.Sum(x => x.Direction == "Credit" ? x.Amount : 0);
            var balance = debit - credit + (account.OpeningBalance ?? 0);

            // Skip zero balance accounts for cleaner report
            if (balance != 0 || debit != 0 || credit != 0)
            {
                trialBalanceLines.Add(new TrialBalanceLineDto
                {
                    AccountId = account.Id.ToString(),
                    AccountCode = account.Code,
                    AccountName = account.Name,
                    AccountType = account.AccountType,
                    Debit = debit + (account.AccountType == "Asset" || account.AccountType == "Expense" ? account.OpeningBalance ?? 0 : 0),
                    Credit = credit + (account.AccountType == "Liability" || account.AccountType == "Equity" || account.AccountType == "Revenue" ? account.OpeningBalance ?? 0 : 0),
                    Balance = balance
                });
            }
        }

        totalDebits = trialBalanceLines.Sum(x => x.Debit);
        totalCredits = trialBalanceLines.Sum(x => x.Credit);
        var difference = totalDebits - totalCredits;

        return new TrialBalanceDto
        {
            AsOfDate = request.AsOfDate,
            Lines = trialBalanceLines,
            TotalDebits = totalDebits,
            TotalCredits = totalCredits,
            Difference = difference,
            IsBalanced = Math.Abs(difference) < 0.01m
        };
    }

    private static DateTime EnsureUtc(DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Unspecified)
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        return dateTime.ToUniversalTime();
    }
}