using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using Cor.Finance.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Queries;

public class GetTrialBalanceQry : IRequest<TrialBalanceDto>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? AsOfDate { get; set; }
    public Guid? PeriodId { get; set; }
    public Guid? BranchId { get; set; }
    public bool IncludeZeroBalances { get; set; }
}

public class GetTrialBalanceHandler : IRequestHandler<GetTrialBalanceQry, TrialBalanceDto>
{
    private readonly FinanceDbContext _context;

    public GetTrialBalanceHandler(FinanceDbContext context) => _context = context;

    public async Task<TrialBalanceDto> Handle(GetTrialBalanceQry request, CancellationToken ct)
    {
        var period = request.PeriodId.HasValue
            ? await _context.FinancialPeriods
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.PeriodId.Value && !x.IsDeleted, ct)
            : null;

        if (request.PeriodId.HasValue && period == null)
            throw new KeyNotFoundException($"Financial period {request.PeriodId.Value} was not found.");

        var startDate = period?.StartDate
            ?? request.StartDate
            ?? request.AsOfDate
            ?? throw new ArgumentException("A periodId, startDate/endDate, or asOfDate is required.");

        var endDate = period?.EndDate
            ?? request.EndDate
            ?? request.AsOfDate
            ?? throw new ArgumentException("A periodId, startDate/endDate, or asOfDate is required.");

        if (endDate < startDate)
            throw new ArgumentException("EndDate must be greater than or equal to StartDate.");

        var service = new AccountingReportService(_context);
        var snapshots = await service.GetAccountSnapshotsAsync(
            startDate,
            endDate,
            request.BranchId,
            null,
            ct);

        var lines = snapshots
            .Where(x => request.IncludeZeroBalances || x.OpeningBalance != 0m || x.PeriodDebits != 0m || x.PeriodCredits != 0m || x.ClosingBalance != 0m)
            .Select(x => new TrialBalanceLineDto
            {
                AccountId = x.AccountId.ToString(),
                AccountCode = x.AccountCode,
                AccountName = x.AccountName,
                AccountType = x.AccountType,
                OpeningDebit = x.OpeningBalance > 0m ? x.OpeningBalance : 0m,
                OpeningCredit = x.OpeningBalance < 0m ? Math.Abs(x.OpeningBalance) : 0m,
                Debit = x.PeriodDebits,
                Credit = x.PeriodCredits,
                ClosingDebit = x.ClosingBalance > 0m ? x.ClosingBalance : 0m,
                ClosingCredit = x.ClosingBalance < 0m ? Math.Abs(x.ClosingBalance) : 0m,
                Balance = x.ClosingBalance
            })
            .OrderBy(x => x.AccountCode)
            .ToList();

        var totalOpeningDebit = lines.Sum(x => x.OpeningDebit);
        var totalOpeningCredit = lines.Sum(x => x.OpeningCredit);
        var totalDebits = lines.Sum(x => x.Debit);
        var totalCredits = lines.Sum(x => x.Credit);
        var totalClosingDebit = lines.Sum(x => x.ClosingDebit);
        var totalClosingCredit = lines.Sum(x => x.ClosingCredit);
        var difference = totalClosingDebit - totalClosingCredit;

        var normalizedEndDate = AccountingReportService.NormalizeUtc(endDate).Date;

        return new TrialBalanceDto
        {
            AsOfDate = normalizedEndDate,
            StartDate = AccountingReportService.NormalizeUtc(startDate).Date,
            EndDate = normalizedEndDate,
            PeriodId = period?.Id,
            PeriodName = period?.Name,
            BranchId = request.BranchId,
            Lines = lines,
            TotalOpeningDebit = totalOpeningDebit,
            TotalOpeningCredit = totalOpeningCredit,
            TotalDebits = totalDebits,
            TotalCredits = totalCredits,
            TotalClosingDebit = totalClosingDebit,
            TotalClosingCredit = totalClosingCredit,
            Difference = difference,
            IsBalanced = Math.Abs(difference) < 0.01m
        };
    }
}
