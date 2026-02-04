using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;

namespace Leave.App.Services;

public interface ILeaveLedgerService
{
    Task Credit(LedgerEntryDto dto);
    Task Debit(LedgerEntryDto dto);
}

public sealed class LeaveLedgerService : ILeaveLedgerService
{
    private readonly IUnitOfWork _unitOfWork;
    public LeaveLedgerService(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task Credit(LedgerEntryDto dto) => await Write(dto, Math.Abs(dto.Amount));
    public async Task Debit(LedgerEntryDto dto) => await Write(dto, -Math.Abs(dto.Amount));

    private async Task Write(LedgerEntryDto dto, double amount)
    {
        //await _unitOfWork.Begin();
        //try
        //{
        //    var entry = new LeaveLedger
        //    {
        //        Date = DateTime.Now,
        //        Amount = amount,
        //        EntryType = dto.EntryType,
        //        SourceType = dto.SourceType,
        //        EmployeeId = dto.EmployeeId,
        //        LeaveTypeId = dto.LeaveTypeId,
        //        LeavePolicyId = dto.LeavePolicyId,
        //        ReferenceId = dto.ReferenceId
        //    };
        //    await _unitOfWork.Repository<LeaveLedger>().Add(entry);
        //    await _unitOfWork.Commit();
        //}
        //catch
        //{
        //    await _unitOfWork.Rollback();
        //    throw;
        //}

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
        await _unitOfWork.Repository<LeaveLedger>().Add(entry);
    }
}