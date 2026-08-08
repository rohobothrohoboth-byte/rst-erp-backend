using Leave.Domain.DTOs;
using Leave.Domain.Entities;

namespace Leave.App.Interfaces;

public interface ILeaveLedgerService
{
    // Credit/Debit operations
    Task Credit(LedgerEntryDto dto, CancellationToken ct);
    Task Debit(LedgerEntryDto dto, CancellationToken ct);

    // Query methods
    Task<IEnumerable<LeaveLedger>> GetEmployeeLeaveLedgerAsync(string employeeId, CancellationToken ct = default);
    Task<LeaveLedger?> GetCurrentLeaveLedgerAsync(string employeeId, CancellationToken ct = default);
    Task<LeaveLedger?> GetLeaveLedgerByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<LeaveLedger>> GetEmployeeLeaveLedgerByTypeAsync(string employeeId, string leaveTypeId, CancellationToken ct = default);
    Task<LeaveLedger?> GetLeaveLedgerByEmployeeAndTypeAsync(string employeeId, string leaveTypeId, DateTime yearStart, CancellationToken ct = default);
    Task<double> GetAvailableLeaveDaysAsync(string employeeId, string leaveTypeId, CancellationToken ct = default);
}