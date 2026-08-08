using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using Leave.Utility.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Leave.Utility.Repos;

public class HistoryRepository : IHistoryRepository
{
    private readonly HrmLeaveDbContext _context;
    private readonly ILogger<HistoryRepository> _logger;

    public HistoryRepository(HrmLeaveDbContext context, ILogger<HistoryRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ArchivePoliciesAsync(List<EmpLeavePolicy> policies, string archiveReason, Guid? processedBy, int processedYear, CancellationToken ct)
    {
        var historyRecords = policies.Select(p => new EmpLeavePolicyHistory
        {
            Id = Guid.NewGuid(),
            OriginalId = p.Id,
            EmployeeId = p.EmployeeId,
            LeaveTypeId = p.LeaveTypeId,
            LeavePolicyId = p.LeavePolicyId,
            AssignedEntitlement = p.AssignedEntitlement,
            UsedEntitlement = p.UsedEntitlement,
            CarryForward = p.CarryForward,
            EffectiveFrom = DateTime.SpecifyKind(p.EffectiveFrom, DateTimeKind.Utc),
            EffectiveTo = p.EffectiveTo.HasValue ? DateTime.SpecifyKind(p.EffectiveTo.Value, DateTimeKind.Utc) : (DateTime?)null,
            IsActive = p.IsActive,
            AssignmentReason = p.AssignmentReason,
            Reason = p.Reason,
            DateAdd = DateTime.SpecifyKind(p.DateAdd, DateTimeKind.Utc),
            DateMod = p.DateMod.HasValue ? DateTime.SpecifyKind(p.DateMod.Value, DateTimeKind.Utc) : (DateTime?)null,
            IsDeleted = p.IsDeleted,
            ArchivedDate = DateTime.UtcNow,
            ArchiveReason = archiveReason,
            ProcessedBy = processedBy,
            ProcessedYear = processedYear
        }).ToList();

        await _context.Set<EmpLeavePolicyHistory>().AddRangeAsync(historyRecords, ct);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation($"Archived {historyRecords.Count} policies with reason: {archiveReason}");
    }

    public async Task<List<EmpLeavePolicyHistoryDto>> GetHistoryByYearAsync(int year, CancellationToken ct)
    {
        var histories = await _context.Set<EmpLeavePolicyHistory>()
            .Where(h => h.ProcessedYear == year)
            .OrderByDescending(h => h.ArchivedDate)
            .ToListAsync(ct);

        return histories.Select(h => new EmpLeavePolicyHistoryDto
        {
            Id = h.Id,
            OriginalId = h.OriginalId,
            EmployeeId = h.EmployeeId,
            LeaveTypeId = h.LeaveTypeId,
            LeavePolicyId = h.LeavePolicyId,
            AssignedEntitlement = h.AssignedEntitlement,
            UsedEntitlement = h.UsedEntitlement,
            CarryForward = h.CarryForward,
            EffectiveFrom = h.EffectiveFrom,
            EffectiveTo = h.EffectiveTo,
            IsActive = h.IsActive,
            AssignmentReason = h.AssignmentReason,
            Reason = h.Reason,
            DateAdd = h.DateAdd,
            DateMod = h.DateMod,
            IsDeleted = h.IsDeleted,
            ArchivedDate = h.ArchivedDate,
            ArchiveReason = h.ArchiveReason,
            ProcessedBy = h.ProcessedBy,
            ProcessedYear = h.ProcessedYear
        }).ToList();
    }

    public async Task<List<EmpLeavePolicyHistoryDto>> GetHistoryByEmployeeAsync(Guid employeeId, CancellationToken ct)
    {
        var histories = await _context.Set<EmpLeavePolicyHistory>()
            .Where(h => h.EmployeeId == employeeId)
            .OrderByDescending(h => h.ArchivedDate)
            .ToListAsync(ct);

        return histories.Select(h => new EmpLeavePolicyHistoryDto
        {
            Id = h.Id,
            OriginalId = h.OriginalId,
            EmployeeId = h.EmployeeId,
            LeaveTypeId = h.LeaveTypeId,
            LeavePolicyId = h.LeavePolicyId,
            AssignedEntitlement = h.AssignedEntitlement,
            UsedEntitlement = h.UsedEntitlement,
            CarryForward = h.CarryForward,
            EffectiveFrom = h.EffectiveFrom,
            EffectiveTo = h.EffectiveTo,
            IsActive = h.IsActive,
            AssignmentReason = h.AssignmentReason,
            Reason = h.Reason,
            DateAdd = h.DateAdd,
            DateMod = h.DateMod,
            IsDeleted = h.IsDeleted,
            ArchivedDate = h.ArchivedDate,
            ArchiveReason = h.ArchiveReason,
            ProcessedBy = h.ProcessedBy,
            ProcessedYear = h.ProcessedYear
        }).ToList();
    }
public async Task DeleteHistoryAsync(Guid historyId, CancellationToken ct)
{
    var history = await _context.Set<EmpLeavePolicyHistory>().FindAsync(new object[] { historyId }, ct);
    if (history != null)
    {
        _context.Set<EmpLeavePolicyHistory>().Remove(history);
        await _context.SaveChangesAsync(ct);
    }
}

public async Task RestoreFromHistoryAsync(Guid historyId, CancellationToken ct)
{
    var historyRecord = await _context.Set<EmpLeavePolicyHistory>()
        .AsNoTracking()
        .FirstOrDefaultAsync(h => h.Id == historyId, ct);

    if (historyRecord == null)
    {
        throw new Exception($"History record {historyId} not found");
    }

    // First, detach any existing tracked entity
    var existingTracked = _context.Set<EmpLeavePolicy>().Local
        .FirstOrDefault(p => p.Id == historyRecord.OriginalId);
    if (existingTracked != null)
    {
        _context.Entry(existingTracked).State = EntityState.Detached;
    }

    // Create or get the policy
    var existingPolicy = await _context.Set<EmpLeavePolicy>()
        .FirstOrDefaultAsync(p => p.Id == historyRecord.OriginalId, ct);

    if (existingPolicy != null)
    {
        existingPolicy.AssignedEntitlement = historyRecord.AssignedEntitlement;
        existingPolicy.UsedEntitlement = historyRecord.UsedEntitlement;
        existingPolicy.CarryForward = historyRecord.CarryForward;
        existingPolicy.EffectiveFrom = DateTime.SpecifyKind(historyRecord.EffectiveFrom, DateTimeKind.Utc);
        existingPolicy.EffectiveTo = historyRecord.EffectiveTo.HasValue
            ? DateTime.SpecifyKind(historyRecord.EffectiveTo.Value, DateTimeKind.Utc)
            : (DateTime?)null;
        existingPolicy.IsActive = historyRecord.IsActive;
      existingPolicy.AssignmentReason = historyRecord.AssignmentReason ?? string.Empty;
      existingPolicy.Reason = historyRecord.Reason ?? string.Empty;
        existingPolicy.IsDeleted = false;
        existingPolicy.DateMod = DateTime.UtcNow;

        _context.Set<EmpLeavePolicy>().Update(existingPolicy);
    }

    await _context.SaveChangesAsync(ct);
    _logger.LogInformation($"Restored policy from history {historyId}");
}

}