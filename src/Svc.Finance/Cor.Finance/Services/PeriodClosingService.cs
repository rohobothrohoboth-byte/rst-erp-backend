// Services/PeriodClosingService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Cor.Finance.Persistence;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Services;
using Newtonsoft.Json;
using Cor.Finance.Models.Enums;

namespace Cor.Finance.Services;

public class PeriodClosingService : IPeriodClosingService
{
    private readonly IPeriodRepository _periodRepository;
    private readonly IAuditService _auditService;
    private readonly ILogger<PeriodClosingService> _logger;

    public PeriodClosingService(
        IPeriodRepository periodRepository,
        IAuditService auditService,
        ILogger<PeriodClosingService> logger)
    {
        _periodRepository = periodRepository;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<PeriodResponseDto> CreatePeriodAsync(CreatePeriodDto dto, Guid userId)
    {
        _logger.LogInformation($"Creating period: {dto.Name}");

        dto.Validate();

        var hasOverlap = await _periodRepository.HasOverlappingPeriodAsync(dto.StartDate, dto.EndDate);
        if (hasOverlap)
        {
            throw new InvalidOperationException("Period overlaps with existing period");
        }

        var period = new FinancialPeriod
        {
            Name = dto.Name,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            PeriodType = dto.PeriodType,
            Notes = dto.Notes,
            CreatedBy = userId,
            Status = PeriodStatus.OPEN
        };

        period.GenerateNameIfNotProvided();

        var created = await _periodRepository.CreateAsync(period);

        await _auditService.LogAsync(new AuditLog
        {
            UserId = userId,
            UserEmail = userId.ToString(),
            Action = "CREATE_PERIOD",
            EntityType = "FinancialPeriod",
            EntityId = created.Id.ToString(),
            NewValues = AuditService.SerializeObject(created) ?? string.Empty, // ✅ Fixed
            MetadataJson = AuditService.SerializeObject(new { dto }) ?? string.Empty // ✅ Fixed
        });

        return PeriodResponseDto.FromEntity(created);
    }

    public async Task<PeriodResponseDto> UpdatePeriodAsync(Guid id, UpdatePeriodDto dto, Guid userId)
    {
        _logger.LogInformation($"Updating period: {id}");

        var period = await _periodRepository.GetByIdAsync(id);
        if (period == null)
        {
            throw new KeyNotFoundException($"Period with ID {id} not found");
        }

        if (period.IsClosed)
        {
            throw new InvalidOperationException("Cannot update a closed period");
        }

        var oldValues = AuditService.SerializeObject(period) ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(dto.Name))
            period.Name = dto.Name;

        if (dto.StartDate.HasValue)
            period.StartDate = dto.StartDate.Value;

        if (dto.EndDate.HasValue)
            period.EndDate = dto.EndDate.Value;

        if (dto.PeriodType.HasValue)
            period.PeriodType = dto.PeriodType.Value;

        if (!string.IsNullOrWhiteSpace(dto.Notes))
            period.Notes = dto.Notes;

        period.ValidateDates();

        var updated = await _periodRepository.UpdateAsync(period);

        var newValues = AuditService.SerializeObject(updated) ?? string.Empty;
        var changes = GetChanges(oldValues, newValues);

        await _auditService.LogAsync(new AuditLog
        {
            UserId = userId,
            UserEmail = userId.ToString(),
            Action = "UPDATE_PERIOD",
            EntityType = "FinancialPeriod",
            EntityId = updated.Id.ToString(),
            OldValues = oldValues,
            NewValues = newValues,
            ChangesJson = changes ?? string.Empty // ✅ Fixed
        });

        return PeriodResponseDto.FromEntity(updated);
    }

    public async Task<PeriodResponseDto> ClosePeriodAsync(Guid id, ClosePeriodDto dto, Guid userId)
    {
        _logger.LogInformation($"Closing period: {id}");

        var period = await _periodRepository.GetByIdAsync(id);
        if (period == null)
        {
            throw new KeyNotFoundException($"Period with ID {id} not found");
        }

        if (period.IsClosed)
        {
            throw new InvalidOperationException("Period is already closed");
        }

        var unpostedCount = await _periodRepository.GetUnpostedEntriesCountAsync(id);
        if (unpostedCount > 0 && !dto.ForceClose)
        {
            throw new InvalidOperationException(
                $"Cannot close period. {unpostedCount} unposted journal entries found. Use force close to override.");
        }

        var oldValues = AuditService.SerializeObject(period) ?? string.Empty;

        period.IsClosed = true;
        period.Status = PeriodStatus.CLOSED;
        period.ClosedDate = DateTime.UtcNow;
        period.ClosedBy = userId;
        period.ClosingMetadataJson = AuditService.SerializeObject(new
        {
            reason = dto.Reason ?? "Period closed",
            forceClosed = dto.ForceClose,
            notes = dto.Notes,
            unpostedEntriesCount = unpostedCount,
            totalEntriesCount = await _periodRepository.GetTotalEntriesCountAsync(id),
            closedAt = DateTime.UtcNow
        }) ?? string.Empty; // ✅ Fixed

        var updated = await _periodRepository.UpdateAsync(period);

        var newValues = AuditService.SerializeObject(updated) ?? string.Empty;

        await _auditService.LogAsync(new AuditLog
        {
            UserId = userId,
            UserEmail = userId.ToString(),
            Action = "CLOSE_PERIOD",
            EntityType = "FinancialPeriod",
            EntityId = updated.Id.ToString(),
            OldValues = oldValues,
            NewValues = newValues,
            ChangesJson = AuditService.SerializeObject(new
            {
                IsClosed = new { old = false, @new = true },
                Status = new { old = PeriodStatus.OPEN.ToString(), @new = PeriodStatus.CLOSED.ToString() }
            }) ?? string.Empty, // ✅ Fixed
            MetadataJson = AuditService.SerializeObject(new
            {
                forceClose = dto.ForceClose,
                notes = dto.Notes,
                reason = dto.Reason,
                unpostedCount
            }) ?? string.Empty // ✅ Fixed
        });

        return PeriodResponseDto.FromEntity(updated);
    }

    public async Task<PeriodResponseDto> OpenPeriodAsync(Guid id, Guid userId)
    {
        _logger.LogInformation($"Opening period: {id}");

        var period = await _periodRepository.GetByIdAsync(id);
        if (period == null)
        {
            throw new KeyNotFoundException($"Period with ID {id} not found");
        }

        if (!period.IsClosed)
        {
            throw new InvalidOperationException("Period is already open");
        }

        var oldValues = AuditService.SerializeObject(period) ?? string.Empty;

        period.IsClosed = false;
        period.Status = PeriodStatus.OPEN;
        period.ClosedDate = null;
        period.ClosedBy = null;

        var updated = await _periodRepository.UpdateAsync(period);

        var newValues = AuditService.SerializeObject(updated) ?? string.Empty;

        await _auditService.LogAsync(new AuditLog
        {
            UserId = userId,
            UserEmail = userId.ToString(),
            Action = "OPEN_PERIOD",
            EntityType = "FinancialPeriod",
            EntityId = updated.Id.ToString(),
            OldValues = oldValues,
            NewValues = newValues,
            ChangesJson = AuditService.SerializeObject(new
            {
                IsClosed = new { old = true, @new = false },
                Status = new { old = PeriodStatus.CLOSED.ToString(), @new = PeriodStatus.OPEN.ToString() }
            }) ?? string.Empty, // ✅ Fixed
            MetadataJson = AuditService.SerializeObject(new { reason = "Period reopened" }) ?? string.Empty // ✅ Fixed
        });

        return PeriodResponseDto.FromEntity(updated);
    }

    public async Task DeletePeriodAsync(Guid id, Guid userId)
    {
        _logger.LogInformation($"Deleting period: {id}");

        var period = await _periodRepository.GetByIdAsync(id);
        if (period == null)
        {
            throw new KeyNotFoundException($"Period with ID {id} not found");
        }

        if (period.IsClosed)
        {
            throw new InvalidOperationException("Cannot delete a closed period");
        }

        var entryCount = await _periodRepository.GetTotalEntriesCountAsync(id);
        if (entryCount > 0)
        {
            throw new InvalidOperationException($"Cannot delete period with {entryCount} journal entries");
        }

        var oldValues = AuditService.SerializeObject(period) ?? string.Empty;

        await _periodRepository.DeleteAsync(id);

        await _auditService.LogAsync(new AuditLog
        {
            UserId = userId,
            UserEmail = userId.ToString(),
            Action = "DELETE_PERIOD",
            EntityType = "FinancialPeriod",
            EntityId = id.ToString(),
            OldValues = oldValues,
            MetadataJson = AuditService.SerializeObject(new { reason = "Period deleted" }) ?? string.Empty // ✅ Fixed
        });
    }

    public async Task<PeriodResponseDto> GetPeriodByIdAsync(Guid id)
    {
        var period = await _periodRepository.GetByIdAsync(id);
        if (period == null)
        {
            throw new KeyNotFoundException($"Period with ID {id} not found");
        }

        period.TotalEntries = await _periodRepository.GetTotalEntriesCountAsync(id);
        period.UnpostedEntries = await _periodRepository.GetUnpostedEntriesCountAsync(id);
        period.PostedEntries = period.TotalEntries - period.UnpostedEntries;

        return PeriodResponseDto.FromEntity(period);
    }

    public async Task<(IEnumerable<PeriodResponseDto> periods, int total, int page, int totalPages)>
        GetPeriodsAsync(PeriodFilterDto filter)
    {
        var (periods, total) = await _periodRepository.GetFilteredAsync(filter);

        var periodList = periods.ToList();
        foreach (var period in periodList)
        {
            period.TotalEntries = await _periodRepository.GetTotalEntriesCountAsync(period.Id);
            period.UnpostedEntries = await _periodRepository.GetUnpostedEntriesCountAsync(period.Id);
            period.PostedEntries = period.TotalEntries - period.UnpostedEntries;
        }

        var responseDtos = periodList.Select(PeriodResponseDto.FromEntity);
        var totalPages = (int)Math.Ceiling((double)total / filter.Limit);

        return (responseDtos, total, filter.Page, totalPages);
    }

    public async Task<PeriodStatsDto> GetPeriodStatsAsync(Guid periodId)
    {
        var period = await GetPeriodByIdAsync(periodId);

        var totalEntries = period.TotalEntries;
        var postedEntries = period.PostedEntries;
        var unpostedEntries = period.UnpostedEntries;

        var totalTransactions = 0;

        var today = DateTime.UtcNow;
        var totalDays = (period.EndDate - period.StartDate).Days;
        var elapsedDays = (today - period.StartDate).Days;
        var daysRemaining = Math.Max(0, totalDays - elapsedDays);
        var completionPercentage = totalEntries > 0
            ? Math.Min(100, (double)postedEntries / totalEntries * 100)
            : 0;

        var canBeClosed = unpostedEntries == 0 && !period.IsClosed;

        return new PeriodStatsDto
        {
            TotalJournalEntries = totalEntries,
            PostedEntries = postedEntries,
            UnpostedEntries = unpostedEntries,
            TotalTransactions = totalTransactions,
            TotalDebit = 0,
            TotalCredit = 0,
            PeriodStart = period.StartDate,
            PeriodEnd = period.EndDate,
            DaysRemaining = daysRemaining,
            CompletionPercentage = completionPercentage,
            CanBeClosed = canBeClosed,
            ClosingReason = unpostedEntries > 0
                ? $"{unpostedEntries} unposted journal entries found"
                : null
        };
    }

   // Services/PeriodClosingService.cs

   public async Task<(bool canClose, string reason)> ValidateClosingAsync(Guid periodId)
   {
       var period = await _periodRepository.GetByIdAsync(periodId);
       if (period == null)
       {
           return (false, "Period not found");
       }

       if (period.IsClosed)
       {
           return (false, "Period is already closed");
       }

       var unpostedCount = await _periodRepository.GetUnpostedEntriesCountAsync(periodId);
       if (unpostedCount > 0)
       {
           return (false, $"{unpostedCount} unposted journal entries found");
       }

       return (true, string.Empty); // ✅ Return empty string for no reason
   }
    public async Task<(IEnumerable<AuditLog> logs, int total)> GetAuditTrailAsync(
        Guid periodId,
        int page,
        int limit)
    {
        return await _auditService.GetAuditLogsAsync(
            "FinancialPeriod",
            periodId.ToString(),
            page,
            limit);
    }

    public async Task<object> ExportPeriodDataAsync(Guid periodId, Guid userId)
    {
        var period = await GetPeriodByIdAsync(periodId);

        var exportData = new
        {
            period = period,
            exportedAt = DateTime.UtcNow,
            exportedBy = userId
        };

        await _auditService.LogAsync(new AuditLog
        {
            UserId = userId,
            UserEmail = userId.ToString(),
            Action = "EXPORT_PERIOD",
            EntityType = "FinancialPeriod",
            EntityId = periodId.ToString(),
            MetadataJson = AuditService.SerializeObject(new { exportDate = DateTime.UtcNow }) ?? string.Empty // ✅ Fixed
        });

        return exportData;
    }

    private string? GetChanges(string oldJson, string newJson) // ✅ Fixed return type
    {
        if (string.IsNullOrWhiteSpace(oldJson) || string.IsNullOrWhiteSpace(newJson))
            return null;

        var oldObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(oldJson);
        var newObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(newJson);
        var changes = new Dictionary<string, object>();

        if (oldObj != null && newObj != null)
        {
            var allKeys = oldObj.Keys.Union(newObj.Keys)
                .Where(k => k != "CreatedAt" && k != "UpdatedAt" && k != "Id");

            foreach (var key in allKeys)
            {
                oldObj.TryGetValue(key, out var oldValue);
                newObj.TryGetValue(key, out var newValue);

                var oldStr = oldValue?.ToString();
                var newStr = newValue?.ToString();

                if (oldStr != newStr)
                {
                    changes[key] = new { old = oldValue, @new = newValue };
                }
            }
        }

        return changes.Any() ? AuditService.SerializeObject(changes) : null;
    }
}