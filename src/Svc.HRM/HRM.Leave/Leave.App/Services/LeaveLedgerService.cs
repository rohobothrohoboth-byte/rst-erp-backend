using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;

namespace Leave.App.Services;

public interface ILeaveLedgerService
{
    Task CreditAsync(LedgerEntryDto dto);
    Task DebitAsync(LedgerEntryDto dto);
}

public sealed class LeaveLedgerService : ILeaveLedgerService
{
    private readonly IUnitOfWork _unitOfWork;
    public LeaveLedgerService(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task CreditAsync(LedgerEntryDto dto) => await Write(dto, Math.Abs(dto.Amount));
    public async Task DebitAsync(LedgerEntryDto dto) => await Write(dto, -Math.Abs(dto.Amount));

    private async Task Write(LedgerEntryDto dto, double amount)
    {
        await _unitOfWork.Begin();
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
            await _unitOfWork.Repository<LeaveLedger>().Add(entry);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}