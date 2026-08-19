using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using Cor.Finance.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Queries;

public class GetGeneralLedgerQry : IRequest<GeneralLedgerDto>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid? PeriodId { get; set; }
    public Guid? AccountId { get; set; }
    public Guid? BranchId { get; set; }
}

public class GetGeneralLedgerHandler : IRequestHandler<GetGeneralLedgerQry, GeneralLedgerDto>
{
    private readonly FinanceDbContext _context;
    public GetGeneralLedgerHandler(FinanceDbContext context) => _context = context;

    public async Task<GeneralLedgerDto> Handle(GetGeneralLedgerQry request, CancellationToken ct)
    {
        var period = request.PeriodId.HasValue ? await _context.FinancialPeriods.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.PeriodId.Value && !x.IsDeleted, ct) : null;
        if (request.PeriodId.HasValue && period == null) throw new KeyNotFoundException($"Financial period {request.PeriodId.Value} was not found.");
        var startDate = period?.StartDate ?? request.StartDate ?? throw new ArgumentException("A periodId or startDate/endDate is required.");
        var endDate = period?.EndDate ?? request.EndDate ?? throw new ArgumentException("A periodId or startDate/endDate is required.");
        if (endDate < startDate) throw new ArgumentException("EndDate must be greater than or equal to StartDate.");

        var snapshots = await new AccountingReportService(_context).GetAccountSnapshotsAsync(startDate,endDate,request.BranchId,request.AccountId,ct);
        if (request.AccountId.HasValue && snapshots.Count == 0) throw new KeyNotFoundException($"Account {request.AccountId.Value} was not found.");
        var openingByAccount=snapshots.ToDictionary(x=>x.AccountId,x=>x.OpeningBalance);
        var runningByAccount=snapshots.ToDictionary(x=>x.AccountId,x=>x.OpeningBalance);
        var metadata=snapshots.ToDictionary(x=>x.AccountId);
        var startUtc=AccountingReportService.NormalizeUtc(startDate).Date;
        var endUtc=AccountingReportService.NormalizeUtc(endDate).Date.AddDays(1).AddTicks(-1);
        var query=_context.JournalEntries.AsNoTracking().Where(x=>!x.IsDeleted&&x.IsPosted&&x.EntryDate>=startUtc&&x.EntryDate<=endUtc);
        if(request.BranchId.HasValue) query=query.Where(x=>x.BranchId==request.BranchId.Value);
        var entries=await query.OrderBy(x=>x.EntryDate).ThenBy(x=>x.Reference).Select(x=>new EntryRow{Id=x.Id,EntryDate=x.EntryDate,Reference=x.Reference,Description=x.Description}).ToListAsync(ct);
        var ids=entries.Select(x=>x.Id).ToList();
        var lines=ids.Count==0?new List<LineRow>():await _context.JournalLines.AsNoTracking().Where(x=>ids.Contains(x.JournalEntryId)&&!x.IsDeleted).Where(x=>!request.AccountId.HasValue||x.AccountId==request.AccountId.Value).Select(x=>new LineRow{JournalEntryId=x.JournalEntryId,AccountId=x.AccountId,Direction=x.Direction,Amount=x.Amount}).ToListAsync(ct);
        var result=new List<GeneralLedgerEntryDto>();
        foreach(var entry in entries)
        foreach(var group in lines.Where(x=>x.JournalEntryId==entry.Id).GroupBy(x=>x.AccountId).OrderBy(x=>metadata.GetValueOrDefault(x.Key)?.AccountCode))
        {
            if(!metadata.TryGetValue(group.Key,out var account)) continue;
            var debit=group.Where(x=>string.Equals(x.Direction,"Debit",StringComparison.OrdinalIgnoreCase)).Sum(x=>x.Amount);
            var credit=group.Where(x=>string.Equals(x.Direction,"Credit",StringComparison.OrdinalIgnoreCase)).Sum(x=>x.Amount);
            var movement=group.Sum(x=>AccountingReportService.GetSignedMovement(account.NormalBalance,x.Direction,x.Amount));
            var current=runningByAccount.GetValueOrDefault(group.Key)+movement; runningByAccount[group.Key]=current;
            result.Add(new GeneralLedgerEntryDto{Date=AccountingReportService.NormalizeUtc(entry.EntryDate),Reference=entry.Reference,Description=entry.Description,AccountCode=account.AccountCode,AccountName=account.AccountName,Debit=debit,Credit=credit,Balance=current});
        }
        return new GeneralLedgerDto{StartDate=startUtc,EndDate=AccountingReportService.NormalizeUtc(endDate).Date,PeriodId=period?.Id,PeriodName=period?.Name,BranchId=request.BranchId,AccountId=request.AccountId,Entries=result,OpeningBalance=request.AccountId.HasValue?openingByAccount.GetValueOrDefault(request.AccountId.Value):openingByAccount.Values.Sum(),ClosingBalance=request.AccountId.HasValue?runningByAccount.GetValueOrDefault(request.AccountId.Value):runningByAccount.Values.Sum(),TotalDebits=result.Sum(x=>x.Debit),TotalCredits=result.Sum(x=>x.Credit)};
    }
    private sealed class EntryRow{public Guid Id{get;init;} public DateTime EntryDate{get;init;} public string Reference{get;init;}=string.Empty; public string Description{get;init;}=string.Empty;}
    private sealed class LineRow{public Guid JournalEntryId{get;init;} public Guid AccountId{get;init;} public string Direction{get;init;}=string.Empty; public decimal Amount{get;init;}}
}
