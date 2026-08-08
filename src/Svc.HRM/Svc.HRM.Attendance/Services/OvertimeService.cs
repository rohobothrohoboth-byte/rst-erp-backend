using Microsoft.EntityFrameworkCore;
using Svc.HRM.Attendance.Models.DTOs;
using Svc.HRM.Attendance.Models.Entities;
using Svc.HRM.Attendance.Models.Enums;
using Svc.HRM.Attendance.Persistence;
using Shared.Helpers.Services;
using System.Text.Json;

namespace Svc.HRM.Attendance.Services;

public class OvertimeService : IOvertimeService
{
    private readonly AttendanceDbContext _context;
    private readonly ICacheService _cache;
    private readonly ILogger<OvertimeService> _logger;

    public OvertimeService(AttendanceDbContext context, ICacheService cache, ILogger<OvertimeService> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<OvertimeDto> CreateOvertimeRequestAsync(OvertimeRequestDto dto, CancellationToken ct = default)
    {
        // Check for existing request on same day
        var existing = await _context.OvertimeRequests
            .AnyAsync(x => x.EmployeeId == dto.EmployeeId &&
                          x.Date == dto.Date &&
                          x.Status != OvertimeStatus.Rejected.ToString() &&
                          !x.IsDeleted, ct);
        if (existing)
            throw new InvalidOperationException("Overtime request already exists for this date");

        var entity = new LocalOvertimeRequest
        {
            EmployeeId = dto.EmployeeId,
            Date = dto.Date,
            HoursRequested = dto.HoursRequested,
            Reason = dto.Reason,
            Status = OvertimeStatus.Pending.ToString(),
            RequestedBy = "System"
        };

        await _context.OvertimeRequests.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);

        await _cache.RemoveAsync($"overtime_requests_employee_{dto.EmployeeId}", ct);
        await _cache.RemoveAsync("overtime_requests_pending", ct);
        return MapToDto(entity);
    }

    public async Task<OvertimeDto> UpdateOvertimeRequestAsync(Guid id, OvertimeRequestDto dto, CancellationToken ct = default)
    {
        var entity = await _context.OvertimeRequests
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity == null) throw new KeyNotFoundException($"Overtime request {id} not found");

        if (entity.Status != OvertimeStatus.Pending.ToString())
            throw new InvalidOperationException($"Cannot update overtime request in {entity.Status} status");

        entity.Date = dto.Date;
        entity.HoursRequested = dto.HoursRequested;
        entity.Reason = dto.Reason;
        entity.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        await _cache.RemoveAsync($"overtime_requests_employee_{entity.EmployeeId}", ct);
        await _cache.RemoveAsync("overtime_requests_pending", ct);
        await _cache.RemoveAsync($"overtime_request_{id}", ct);
        return MapToDto(entity);
    }

    public async Task<OvertimeDto> GetOvertimeRequestAsync(Guid id, CancellationToken ct = default)
    {
        var cacheKey = $"overtime_request_{id}";
        var cached = await _cache.GetAsync<OvertimeDto>(cacheKey, ct);
        if (cached != null)
            return cached;

        var entity = await _context.OvertimeRequests
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity == null) throw new KeyNotFoundException($"Overtime request {id} not found");

        var dto = MapToDto(entity);
        await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(15), ct);
        return dto;
    }

    public async Task<List<OvertimeDto>> GetEmployeeOvertimeRequestsAsync(Guid employeeId, CancellationToken ct = default)
    {
        var cacheKey = $"overtime_requests_employee_{employeeId}";
        var cached = await _cache.GetAsync<List<OvertimeDto>>(cacheKey, ct);
        if (cached != null)
            return cached;

        var entities = await _context.OvertimeRequests
            .Where(x => x.EmployeeId == employeeId && !x.IsDeleted)
            .OrderByDescending(x => x.Date)
            .ToListAsync(ct);

        var dtos = entities.Select(MapToDto).ToList();
        await _cache.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(15), ct);
        return dtos;
    }

    public async Task<List<OvertimeDto>> GetPendingOvertimeRequestsAsync(CancellationToken ct = default)
    {
        var cacheKey = "overtime_requests_pending";
        var cached = await _cache.GetAsync<List<OvertimeDto>>(cacheKey, ct);
        if (cached != null)
            return cached;

        var entities = await _context.OvertimeRequests
            .Where(x => x.Status == OvertimeStatus.Pending.ToString() && !x.IsDeleted)
            .OrderBy(x => x.Date)
            .ToListAsync(ct);

        var dtos = entities.Select(MapToDto).ToList();
        await _cache.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(5), ct);
        return dtos;
    }

    public async Task<OvertimeDto> ApproveOvertimeRequestAsync(Guid id, OvertimeApproveDto dto, CancellationToken ct = default)
    {
        var entity = await _context.OvertimeRequests
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity == null) throw new KeyNotFoundException($"Overtime request {id} not found");

        if (entity.Status != OvertimeStatus.Pending.ToString())
            throw new InvalidOperationException($"Cannot approve overtime request in {entity.Status} status");

        var hoursApproved = dto.HoursApproved > 0 ? dto.HoursApproved : entity.HoursRequested;

        entity.Status = OvertimeStatus.Approved.ToString();
        entity.HoursApproved = hoursApproved;
        entity.ApprovedAt = DateTime.UtcNow;
        entity.ApprovedBy = dto.ApprovedBy ?? "System";

        await _context.SaveChangesAsync(ct);

        // Publish event for payroll integration
        // await _eventPublisher.PublishOvertimeApprovedAsync(entity.EmployeeId, entity.Date, hoursApproved, ct);

        await _cache.RemoveAsync($"overtime_requests_employee_{entity.EmployeeId}", ct);
        await _cache.RemoveAsync("overtime_requests_pending", ct);
        await _cache.RemoveAsync($"overtime_request_{id}", ct);
        return MapToDto(entity);
    }

    public async Task<OvertimeDto> RejectOvertimeRequestAsync(Guid id, string reason, string? rejectedBy = null, CancellationToken ct = default)
    {
        var entity = await _context.OvertimeRequests
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity == null) throw new KeyNotFoundException($"Overtime request {id} not found");

        if (entity.Status != OvertimeStatus.Pending.ToString())
            throw new InvalidOperationException($"Cannot reject overtime request in {entity.Status} status");

        entity.Status = OvertimeStatus.Rejected.ToString();
        entity.RejectedReason = reason;
        entity.ApprovedAt = DateTime.UtcNow;
        entity.ApprovedBy = rejectedBy ?? "System";

        await _context.SaveChangesAsync(ct);

        await _cache.RemoveAsync($"overtime_requests_employee_{entity.EmployeeId}", ct);
        await _cache.RemoveAsync("overtime_requests_pending", ct);
        await _cache.RemoveAsync($"overtime_request_{id}", ct);
        return MapToDto(entity);
    }

    public async Task DeleteOvertimeRequestAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _context.OvertimeRequests
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity == null) throw new KeyNotFoundException($"Overtime request {id} not found");

        entity.IsDeleted = true;
        await _context.SaveChangesAsync(ct);

        await _cache.RemoveAsync($"overtime_requests_employee_{entity.EmployeeId}", ct);
        await _cache.RemoveAsync("overtime_requests_pending", ct);
        await _cache.RemoveAsync($"overtime_request_{id}", ct);
    }

    #region Mapping

    private static OvertimeDto MapToDto(LocalOvertimeRequest entity)
    {
        return new OvertimeDto
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            Date = entity.Date,
            HoursRequested = entity.HoursRequested,
            HoursApproved = entity.HoursApproved,
            Reason = entity.Reason,
            Status = entity.Status,
            ApprovedAt = entity.ApprovedAt,
            ApprovedBy = entity.ApprovedBy,
            RejectedReason = entity.RejectedReason
        };
    }

    #endregion
}