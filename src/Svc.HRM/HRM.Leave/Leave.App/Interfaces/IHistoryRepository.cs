// Leave.App/Interfaces/IHistoryRepository.cs
using Leave.Domain.DTOs;
using Leave.Domain.Entities;

namespace Leave.App.Interfaces;

public interface IHistoryRepository
{
    Task ArchivePoliciesAsync(List<EmpLeavePolicy> policies, string archiveReason, Guid? processedBy, int processedYear, CancellationToken ct);
    Task<List<EmpLeavePolicyHistoryDto>> GetHistoryByYearAsync(int year, CancellationToken ct);
    Task<List<EmpLeavePolicyHistoryDto>> GetHistoryByEmployeeAsync(Guid employeeId, CancellationToken ct);
    Task RestoreFromHistoryAsync(Guid historyId, CancellationToken ct);
     Task DeleteHistoryAsync(Guid historyId, CancellationToken ct);

}