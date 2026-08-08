using Microsoft.EntityFrameworkCore;
using Svc.HRM.Attendance.Models.DTOs;
using Svc.HRM.Attendance.Models.Entities;
using Svc.HRM.Attendance.Models.Enums;
using Svc.HRM.Attendance.Persistence;
using Shared.Helpers.Services;
using System.Text.Json;

namespace Svc.HRM.Attendance.Services;

public class LeaveService : ILeaveService
{
    private readonly AttendanceDbContext _context;
    private readonly ICacheService _cache;
    private readonly ILogger<LeaveService> _logger;

    public LeaveService(AttendanceDbContext context, ICacheService cache, ILogger<LeaveService> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    // ✅ Helper method to ensure UTC
    private DateTime EnsureUtc(DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Unspecified)
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        if (dateTime.Kind == DateTimeKind.Local)
            return dateTime.ToUniversalTime();
        return dateTime;
    }

    #region Leave Request

    public async Task<LeaveRequestDto> CreateLeaveRequestAsync(LeaveRequestCreateDto dto, CancellationToken ct = default)
    {
        // ✅ Ensure dates are UTC
        var startDate = EnsureUtc(dto.StartDate);
        var endDate = EnsureUtc(dto.EndDate);

        // Check for overlapping requests
        var overlapping = await _context.LeaveRequests
            .AnyAsync(x => x.EmployeeId == dto.EmployeeId &&
                          ((startDate >= x.StartDate && startDate <= x.EndDate) ||
                           (endDate >= x.StartDate && endDate <= x.EndDate)) &&
                          x.Status != LeaveStatus.Rejected.ToString() &&
                          !x.IsDeleted, ct);
        if (overlapping)
            throw new InvalidOperationException("You have an overlapping leave request");

        // Check balance
        var balance = await GetLeaveBalanceAsync(dto.EmployeeId, dto.LeaveType, DateTime.UtcNow.Year, ct);
        var daysRequested = (endDate - startDate).Days + 1;
        if (dto.IsPaid && daysRequested > balance.BalanceDays)
            throw new InvalidOperationException($"Insufficient leave balance. Available: {balance.BalanceDays} days");

        var entity = new LocalLeaveRequest
        {
            EmployeeId = dto.EmployeeId,
            LeaveType = dto.LeaveType,
            StartDate = startDate,
            EndDate = endDate,
            DaysRequested = daysRequested,
            Reason = dto.Reason,
            Status = LeaveStatus.Pending.ToString(),
            IsPaid = dto.IsPaid,
            RequestedBy = "System"
        };

        await _context.LeaveRequests.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);

        await _cache.RemoveAsync($"leave_requests_employee_{dto.EmployeeId}", ct);
        await _cache.RemoveAsync("leave_requests_pending", ct);
        return MapToDto(entity);
    }

    public async Task<LeaveRequestDto> UpdateLeaveRequestAsync(Guid id, LeaveRequestCreateDto dto, CancellationToken ct = default)
    {
        var entity = await _context.LeaveRequests
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity == null) throw new KeyNotFoundException($"Leave request {id} not found");

        if (entity.Status != LeaveStatus.Pending.ToString())
            throw new InvalidOperationException($"Cannot update leave request in {entity.Status} status");

        // ✅ Ensure dates are UTC
        var startDate = EnsureUtc(dto.StartDate);
        var endDate = EnsureUtc(dto.EndDate);

        entity.LeaveType = dto.LeaveType;
        entity.StartDate = startDate;
        entity.EndDate = endDate;
        entity.DaysRequested = (endDate - startDate).Days + 1;
        entity.Reason = dto.Reason;
        entity.IsPaid = dto.IsPaid;
        entity.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        await _cache.RemoveAsync($"leave_requests_employee_{entity.EmployeeId}", ct);
        await _cache.RemoveAsync("leave_requests_pending", ct);
        return MapToDto(entity);
    }

    public async Task<LeaveRequestDto> GetLeaveRequestAsync(Guid id, CancellationToken ct = default)
    {
        var cacheKey = $"leave_request_{id}";
        var cached = await _cache.GetAsync<LeaveRequestDto>(cacheKey, ct);
        if (cached != null)
            return cached;

        var entity = await _context.LeaveRequests
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity == null) throw new KeyNotFoundException($"Leave request {id} not found");

        var dto = MapToDto(entity);
        await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(15), ct);
        return dto;
    }

    public async Task<List<LeaveRequestDto>> GetEmployeeLeaveRequestsAsync(Guid employeeId, CancellationToken ct = default)
    {
        var cacheKey = $"leave_requests_employee_{employeeId}";
        var cached = await _cache.GetAsync<List<LeaveRequestDto>>(cacheKey, ct);
        if (cached != null)
            return cached;

        var entities = await _context.LeaveRequests
            .Where(x => x.EmployeeId == employeeId && !x.IsDeleted)
            .OrderByDescending(x => x.StartDate)
            .ToListAsync(ct);

        var dtos = entities.Select(MapToDto).ToList();
        await _cache.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(15), ct);
        return dtos;
    }

    public async Task<List<LeaveRequestDto>> GetPendingLeaveRequestsAsync(CancellationToken ct = default)
    {
        var cacheKey = "leave_requests_pending";
        var cached = await _cache.GetAsync<List<LeaveRequestDto>>(cacheKey, ct);
        if (cached != null)
            return cached;

        var entities = await _context.LeaveRequests
            .Where(x => x.Status == LeaveStatus.Pending.ToString() && !x.IsDeleted)
            .OrderBy(x => x.StartDate)
            .ToListAsync(ct);

        var dtos = entities.Select(MapToDto).ToList();
        await _cache.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(5), ct);
        return dtos;
    }

    public async Task<LeaveRequestDto> ApproveLeaveRequestAsync(Guid id, LeaveApproveDto dto, CancellationToken ct = default)
    {
        var entity = await _context.LeaveRequests
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity == null) throw new KeyNotFoundException($"Leave request {id} not found");

        if (entity.Status != LeaveStatus.Pending.ToString())
            throw new InvalidOperationException($"Cannot approve leave request in {entity.Status} status");

        var daysApproved = dto.DaysApproved > 0 ? dto.DaysApproved : entity.DaysRequested;

        entity.Status = LeaveStatus.Approved.ToString();
        entity.DaysApproved = daysApproved;
        entity.ApprovedAt = DateTime.UtcNow;
        entity.ApprovedBy = dto.ApprovedBy ?? "System";

        await _context.SaveChangesAsync(ct);

        // Update leave balance
        await UpdateLeaveBalanceAsync(entity.EmployeeId, entity.LeaveType, daysApproved, ct);

        await _cache.RemoveAsync($"leave_requests_employee_{entity.EmployeeId}", ct);
        await _cache.RemoveAsync("leave_requests_pending", ct);
        await _cache.RemoveAsync($"leave_request_{id}", ct);
        return MapToDto(entity);
    }

    public async Task<LeaveRequestDto> RejectLeaveRequestAsync(Guid id, string reason, string? rejectedBy = null, CancellationToken ct = default)
    {
        var entity = await _context.LeaveRequests
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity == null) throw new KeyNotFoundException($"Leave request {id} not found");

        if (entity.Status != LeaveStatus.Pending.ToString())
            throw new InvalidOperationException($"Cannot reject leave request in {entity.Status} status");

        entity.Status = LeaveStatus.Rejected.ToString();
        entity.RejectedReason = reason;
        entity.ApprovedAt = DateTime.UtcNow;
        entity.ApprovedBy = rejectedBy ?? "System";

        await _context.SaveChangesAsync(ct);

        await _cache.RemoveAsync($"leave_requests_employee_{entity.EmployeeId}", ct);
        await _cache.RemoveAsync("leave_requests_pending", ct);
        await _cache.RemoveAsync($"leave_request_{id}", ct);
        return MapToDto(entity);
    }

    public async Task DeleteLeaveRequestAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _context.LeaveRequests
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity == null) throw new KeyNotFoundException($"Leave request {id} not found");

        entity.IsDeleted = true;
        await _context.SaveChangesAsync(ct);

        await _cache.RemoveAsync($"leave_requests_employee_{entity.EmployeeId}", ct);
        await _cache.RemoveAsync("leave_requests_pending", ct);
        await _cache.RemoveAsync($"leave_request_{id}", ct);
    }

    #endregion

    #region Leave Balance

    public async Task<LeaveBalanceDto> GetLeaveBalanceAsync(Guid employeeId, string leaveType, int year, CancellationToken ct = default)
    {
        var entity = await _context.LeaveBalances
            .FirstOrDefaultAsync(x => x.EmployeeId == employeeId &&
                                      x.LeaveType == leaveType &&
                                      x.Year == year &&
                                      !x.IsDeleted, ct);

        if (entity == null)
        {
            // Initialize balance if not found
            await InitializeLeaveBalanceAsync(employeeId, year, ct);
            entity = await _context.LeaveBalances
                .FirstOrDefaultAsync(x => x.EmployeeId == employeeId &&
                                          x.LeaveType == leaveType &&
                                          x.Year == year &&
                                          !x.IsDeleted, ct);
        }

        return MapToBalanceDto(entity!);
    }

    public async Task<List<LeaveBalanceDto>> GetEmployeeLeaveBalancesAsync(Guid employeeId, int year, CancellationToken ct = default)
    {
        var cacheKey = $"leave_balances_{employeeId}_{year}";
        var cached = await _cache.GetAsync<List<LeaveBalanceDto>>(cacheKey, ct);
        if (cached != null)
            return cached;

        var entities = await _context.LeaveBalances
            .Where(x => x.EmployeeId == employeeId && x.Year == year && !x.IsDeleted)
            .ToListAsync(ct);

        // Initialize default balances if none exist
        if (!entities.Any())
        {
            await InitializeLeaveBalanceAsync(employeeId, year, ct);
            entities = await _context.LeaveBalances
                .Where(x => x.EmployeeId == employeeId && x.Year == year && !x.IsDeleted)
                .ToListAsync(ct);
        }

        var dtos = entities.Select(MapToBalanceDto).ToList();
        await _cache.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(15), ct);
        return dtos;
    }

    public async Task InitializeLeaveBalanceAsync(Guid employeeId, int year, CancellationToken ct = default)
    {
        // Check if already initialized
        var exists = await _context.LeaveBalances
            .AnyAsync(x => x.EmployeeId == employeeId && x.Year == year && !x.IsDeleted, ct);
        if (exists) return;

        // Default leave allocations (in days)
        var leaveTypes = new Dictionary<string, double>
        {
            { LeaveType.Annual.ToString(), 20 },
            { LeaveType.Sick.ToString(), 10 },
            { LeaveType.Casual.ToString(), 5 }
        };

        foreach (var type in leaveTypes)
        {
            var balance = new LocalLeaveBalance
            {
                EmployeeId = employeeId,
                LeaveType = type.Key,
                TotalDays = type.Value,
                UsedDays = 0,
                Year = year,
                // ✅ Ensure UTC
                ExpiryDate = EnsureUtc(new DateTime(year, 12, 31)),
                IsActive = true
            };
            await _context.LeaveBalances.AddAsync(balance, ct);
        }

        await _context.SaveChangesAsync(ct);
        await _cache.RemoveAsync($"leave_balances_{employeeId}_{year}", ct);
    }

    public async Task UpdateLeaveBalanceAsync(Guid employeeId, string leaveType, double daysUsed, CancellationToken ct = default)
    {
        var year = DateTime.UtcNow.Year;
        var balance = await _context.LeaveBalances
            .FirstOrDefaultAsync(x => x.EmployeeId == employeeId &&
                                      x.LeaveType == leaveType &&
                                      x.Year == year &&
                                      !x.IsDeleted, ct);

        if (balance == null)
        {
            await InitializeLeaveBalanceAsync(employeeId, year, ct);
            balance = await _context.LeaveBalances
                .FirstOrDefaultAsync(x => x.EmployeeId == employeeId &&
                                          x.LeaveType == leaveType &&
                                          x.Year == year &&
                                          !x.IsDeleted, ct);
        }

        if (balance != null)
        {
            balance.UsedDays += daysUsed;
            balance.DateMod = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
            await _cache.RemoveAsync($"leave_balances_{employeeId}_{year}", ct);
        }
    }

    #endregion

    #region Leave Calendar

    public async Task<List<LeaveRequestDto>> GetLeaveCalendarAsync(int year, int? month = null, CancellationToken ct = default)
    {
        var cacheKey = $"leave_calendar_{year}_{month ?? 0}";
        var cached = await _cache.GetAsync<List<LeaveRequestDto>>(cacheKey, ct);
        if (cached != null)
            return cached;

        var query = _context.LeaveRequests
            .Where(x => x.StartDate.Year == year && !x.IsDeleted);

        if (month.HasValue)
            query = query.Where(x => x.StartDate.Month == month.Value);

        var entities = await query
            .OrderBy(x => x.StartDate)
            .ToListAsync(ct);

        var dtos = entities.Select(MapToDto).ToList();
        await _cache.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(15), ct);
        return dtos;
    }

    public async Task<List<LeaveRequestDto>> GetDepartmentLeaveCalendarAsync(Guid departmentId, int year, int? month = null, CancellationToken ct = default)
    {
        var cacheKey = $"leave_calendar_dept_{departmentId}_{year}_{month ?? 0}";
        var cached = await _cache.GetAsync<List<LeaveRequestDto>>(cacheKey, ct);
        if (cached != null)
            return cached;

        var query = _context.LeaveRequests
            .Where(x => x.StartDate.Year == year &&
                        x.Status == LeaveStatus.Approved.ToString() &&
                        !x.IsDeleted);

        if (month.HasValue)
            query = query.Where(x => x.StartDate.Month == month.Value);

        var entities = await query
            .OrderBy(x => x.StartDate)
            .ToListAsync(ct);

        var dtos = entities.Select(MapToDto).ToList();
        await _cache.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(15), ct);
        return dtos;
    }

    #endregion

    #region Mapping

    private static LeaveRequestDto MapToDto(LocalLeaveRequest entity)
    {
        return new LeaveRequestDto
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            LeaveType = entity.LeaveType,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            DaysRequested = entity.DaysRequested,
            DaysApproved = entity.DaysApproved,
            Reason = entity.Reason,
            Status = entity.Status,
            ApprovedAt = entity.ApprovedAt,
            ApprovedBy = entity.ApprovedBy,
            RejectedReason = entity.RejectedReason,
            IsPaid = entity.IsPaid
        };
    }

    private static LeaveBalanceDto MapToBalanceDto(LocalLeaveBalance entity)
    {
        return new LeaveBalanceDto
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            LeaveType = entity.LeaveType,
            TotalDays = entity.TotalDays,
            UsedDays = entity.UsedDays,
            BalanceDays = entity.BalanceDays,
            Year = entity.Year,
            ExpiryDate = entity.ExpiryDate
        };
    }

    #endregion
}