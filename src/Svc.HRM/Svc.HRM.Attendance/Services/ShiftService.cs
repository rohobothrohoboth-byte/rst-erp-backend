using Microsoft.EntityFrameworkCore;
using Svc.HRM.Attendance.Models.DTOs;
using Svc.HRM.Attendance.Models.Entities;
using Svc.HRM.Attendance.Persistence;
using Shared.Helpers.Services;
using System.Text.Json;

namespace Svc.HRM.Attendance.Services;

public class ShiftService : IShiftService
{
    private readonly AttendanceDbContext _context;
    private readonly ICacheService _cache;
    private readonly ILogger<ShiftService> _logger;

    public ShiftService(AttendanceDbContext context, ICacheService cache, ILogger<ShiftService> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    #region Shift CRUD

    public async Task<ShiftDto> CreateShiftAsync(ShiftCreateDto dto, CancellationToken ct = default)
    {
        var entity = new LocalShift
        {
            Name = dto.Name,
            NameAm = dto.NameAm,
            Description = dto.Description,
            StartTime = TimeSpan.Parse(dto.StartTime),
            EndTime = TimeSpan.Parse(dto.EndTime),
            BreakStartTime = TimeSpan.Parse(dto.BreakStartTime),
            BreakEndTime = TimeSpan.Parse(dto.BreakEndTime),
            BreakDurationHours = dto.BreakDurationHours,
            ColorCode = dto.ColorCode,
            IsActive = true
        };

        await _context.Shifts.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);

        await _cache.RemoveAsync("shifts_all", ct);
        return MapToDto(entity);
    }

    public async Task<ShiftDto> UpdateShiftAsync(Guid id, ShiftCreateDto dto, CancellationToken ct = default)
    {
        var entity = await _context.Shifts.FindAsync([id], ct);
        if (entity == null) throw new KeyNotFoundException($"Shift {id} not found");

        entity.Name = dto.Name;
        entity.NameAm = dto.NameAm;
        entity.Description = dto.Description;
        entity.StartTime = TimeSpan.Parse(dto.StartTime);
        entity.EndTime = TimeSpan.Parse(dto.EndTime);
        entity.BreakStartTime = TimeSpan.Parse(dto.BreakStartTime);
        entity.BreakEndTime = TimeSpan.Parse(dto.BreakEndTime);
        entity.BreakDurationHours = dto.BreakDurationHours;
        entity.ColorCode = dto.ColorCode;
        entity.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        await _cache.RemoveAsync($"shift_{id}", ct);
        await _cache.RemoveAsync("shifts_all", ct);
        return MapToDto(entity);
    }

    public async Task<ShiftDto> GetShiftAsync(Guid id, CancellationToken ct = default)
    {
        var cacheKey = $"shift_{id}";
        var cached = await _cache.GetAsync<ShiftDto>(cacheKey, ct);
        if (cached != null)
            return cached;

        var entity = await _context.Shifts
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity == null) throw new KeyNotFoundException($"Shift {id} not found");

        var dto = MapToDto(entity);
        await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(15), ct);
        return dto;
    }

    public async Task<List<ShiftDto>> GetAllShiftsAsync(CancellationToken ct = default)
    {
        var cacheKey = "shifts_all";
        var cached = await _cache.GetAsync<List<ShiftDto>>(cacheKey, ct);
        if (cached != null)
            return cached;

        var entities = await _context.Shifts
            .Where(x => !x.IsDeleted && x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

        var dtos = entities.Select(MapToDto).ToList();
        await _cache.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(15), ct);
        return dtos;
    }

    public async Task DeleteShiftAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _context.Shifts.FindAsync([id], ct);
        if (entity == null) throw new KeyNotFoundException($"Shift {id} not found");

        entity.IsDeleted = true;
        entity.IsActive = false;
        await _context.SaveChangesAsync(ct);

        await _cache.RemoveAsync($"shift_{id}", ct);
        await _cache.RemoveAsync("shifts_all", ct);
    }

    #endregion

    #region Shift Assignment

    public async Task<ShiftAssignmentDto> AssignShiftAsync(ShiftAssignmentCreateDto dto, CancellationToken ct = default)
    {
        // Check if shift exists
        var shift = await _context.Shifts
            .FirstOrDefaultAsync(x => x.Id == dto.ShiftId && !x.IsDeleted && x.IsActive, ct);
        if (shift == null)
            throw new KeyNotFoundException($"Shift {dto.ShiftId} not found or inactive");

        // Deactivate previous assignments
        var previous = await _context.ShiftAssignments
            .Where(x => x.EmployeeId == dto.EmployeeId && x.IsActive)
            .ToListAsync(ct);
        foreach (var item in previous)
        {
            item.IsActive = false;
            item.EndDate = dto.EffectiveDate.AddDays(-1);
        }

        var entity = new LocalShiftAssignment
        {
            EmployeeId = dto.EmployeeId,
            ShiftId = dto.ShiftId,
            EffectiveDate = dto.EffectiveDate,
            EndDate = dto.EndDate,
            IsActive = true,
            CreatedBy = "System"
        };

        await _context.ShiftAssignments.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);

        await _cache.RemoveAsync($"shift_assignments_employee_{dto.EmployeeId}", ct);
        return MapToAssignmentDto(entity);
    }

    public async Task<ShiftAssignmentDto> UpdateShiftAssignmentAsync(Guid id, ShiftAssignmentCreateDto dto, CancellationToken ct = default)
    {
        var entity = await _context.ShiftAssignments
            .Include(x => x.Shift)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity == null) throw new KeyNotFoundException($"Shift assignment {id} not found");

        entity.ShiftId = dto.ShiftId;
        entity.EffectiveDate = dto.EffectiveDate;
        entity.EndDate = dto.EndDate;
        entity.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        await _cache.RemoveAsync($"shift_assignments_employee_{entity.EmployeeId}", ct);
        return MapToAssignmentDto(entity);
    }

    public async Task<ShiftAssignmentDto> GetShiftAssignmentAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _context.ShiftAssignments
            .Include(x => x.Shift)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity == null) throw new KeyNotFoundException($"Shift assignment {id} not found");

        return MapToAssignmentDto(entity);
    }

    public async Task<List<ShiftAssignmentDto>> GetEmployeeShiftAssignmentsAsync(Guid employeeId, CancellationToken ct = default)
    {
        var cacheKey = $"shift_assignments_employee_{employeeId}";
        var cached = await _cache.GetAsync<List<ShiftAssignmentDto>>(cacheKey, ct);
        if (cached != null)
            return cached;

        var entities = await _context.ShiftAssignments
            .Include(x => x.Shift)
            .Where(x => x.EmployeeId == employeeId && !x.IsDeleted)
            .OrderByDescending(x => x.EffectiveDate)
            .ToListAsync(ct);

        var dtos = entities.Select(MapToAssignmentDto).ToList();
        await _cache.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(15), ct);
        return dtos;
    }

    public async Task<List<ShiftAssignmentDto>> GetActiveShiftAssignmentsAsync(CancellationToken ct = default)
    {
        var cacheKey = "shift_assignments_active";
        var cached = await _cache.GetAsync<List<ShiftAssignmentDto>>(cacheKey, ct);
        if (cached != null)
            return cached;

        var entities = await _context.ShiftAssignments
            .Include(x => x.Shift)
            .Where(x => x.IsActive && !x.IsDeleted)
            .ToListAsync(ct);

        var dtos = entities.Select(MapToAssignmentDto).ToList();
        await _cache.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(15), ct);
        return dtos;
    }

    public async Task UnassignShiftAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _context.ShiftAssignments
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity == null) throw new KeyNotFoundException($"Shift assignment {id} not found");

        entity.IsActive = false;
        entity.EndDate = DateTime.UtcNow;
        entity.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        await _cache.RemoveAsync($"shift_assignments_employee_{entity.EmployeeId}", ct);
        await _cache.RemoveAsync("shift_assignments_active", ct);
    }

    #endregion

    #region Employee Shift

    public async Task<LocalShift?> GetEmployeeShiftAsync(Guid employeeId, DateTime date, CancellationToken ct = default)
    {
        var assignment = await _context.ShiftAssignments
            .Include(x => x.Shift)
            .FirstOrDefaultAsync(x => x.EmployeeId == employeeId &&
                                      x.EffectiveDate <= date &&
                                      (!x.EndDate.HasValue || x.EndDate >= date) &&
                                      x.IsActive &&
                                      !x.IsDeleted, ct);

        return assignment?.Shift;
    }

    public async Task<ShiftDto> GetEmployeeCurrentShiftAsync(Guid employeeId, CancellationToken ct = default)
    {
        var cacheKey = $"employee_current_shift_{employeeId}";
        var cached = await _cache.GetAsync<ShiftDto>(cacheKey, ct);
        if (cached != null)
            return cached;

        var shift = await GetEmployeeShiftAsync(employeeId, DateTime.UtcNow, ct);
        if (shift == null)
            return null!;

        var dto = MapToDto(shift);
        await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(5), ct);
        return dto;
    }

    #endregion

    #region Mapping

    private static ShiftDto MapToDto(LocalShift entity)
    {
        return new ShiftDto
        {
            Id = entity.Id,
            Name = entity.Name,
            NameAm = entity.NameAm,
            Description = entity.Description,
            StartTime = entity.StartTime.ToString(@"hh\:mm"),
            EndTime = entity.EndTime.ToString(@"hh\:mm"),
            BreakStartTime = entity.BreakStartTime.ToString(@"hh\:mm"),
            BreakEndTime = entity.BreakEndTime.ToString(@"hh\:mm"),
            BreakDurationHours = entity.BreakDurationHours,
            TotalHours = entity.TotalHours,
            IsActive = entity.IsActive,
            ColorCode = entity.ColorCode
        };
    }

    private static ShiftAssignmentDto MapToAssignmentDto(LocalShiftAssignment entity)
    {
        return new ShiftAssignmentDto
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            ShiftId = entity.ShiftId,
            ShiftName = entity.Shift?.Name ?? "Unknown",
            EffectiveDate = entity.EffectiveDate,
            EndDate = entity.EndDate,
            IsActive = entity.IsActive
        };
    }

    #endregion
}