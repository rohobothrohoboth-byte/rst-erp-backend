using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;

namespace Leave.App.Services;

public interface ILeaveLedgerService
{
    Task Credit(LedgerEntryDto dto, CancellationToken ct);
    Task Debit(LedgerEntryDto dto, CancellationToken ct);
}

public sealed class LeaveLedgerService : ILeaveLedgerService
{
    private readonly IUnitOfWork _uow;
    public LeaveLedgerService(IUnitOfWork uow) { _uow = uow; }
    public async Task Credit(LedgerEntryDto dto, CancellationToken ct) => await Write(dto, Math.Abs(dto.Amount), ct);
    public async Task Debit(LedgerEntryDto dto, CancellationToken ct) => await Write(dto, -Math.Abs(dto.Amount), ct);

    private async Task Write(LedgerEntryDto dto, double amount, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var entry = new LeaveLedger
            {
                Date = DateTime.Now,
                Amount = amount,
                EntryType = dto.EntryType,
                SourceType = dto.SourceType,
                EmployeeId = dto.EmployeeId,
                LeaveTypeId = dto.LeaveTypeId,
                LeavePolicyId = dto.LeavePolicyId,
                ReferenceId = dto.ReferenceId
            };
            await _uow.Add(entry, ct);
            //await _uow.Commit(ct);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
        //var entry = new LeaveLedger
        //{
        //    Date = DateTime.Now,
        //    Amount = amount,
        //    EntryType = dto.EntryType,
        //    SourceType = dto.SourceType,
        //    EmployeeId = dto.EmployeeId,
        //    LeaveTypeId = dto.LeaveTypeId,
        //    LeavePolicyId = dto.LeavePolicyId,
        //    ReferenceId = dto.ReferenceId
        //};
        //await _uow.Add(entry, ct);

    }
}