using Svc.HRM.Attendance.Models.DTOs;
using Svc.HRM.Attendance.Models.Entities;

namespace Svc.HRM.Attendance.Services;

public interface IShiftService
{
    // Shift CRUD
    Task<ShiftDto> CreateShiftAsync(ShiftCreateDto dto, CancellationToken ct = default);
    Task<ShiftDto> UpdateShiftAsync(Guid id, ShiftCreateDto dto, CancellationToken ct = default);
    Task<ShiftDto> GetShiftAsync(Guid id, CancellationToken ct = default);
    Task<List<ShiftDto>> GetAllShiftsAsync(CancellationToken ct = default);
    Task DeleteShiftAsync(Guid id, CancellationToken ct = default);

    // Shift Assignment
    Task<ShiftAssignmentDto> AssignShiftAsync(ShiftAssignmentCreateDto dto, CancellationToken ct = default);
    Task<ShiftAssignmentDto> UpdateShiftAssignmentAsync(Guid id, ShiftAssignmentCreateDto dto, CancellationToken ct = default);
    Task<ShiftAssignmentDto> GetShiftAssignmentAsync(Guid id, CancellationToken ct = default);
    Task<List<ShiftAssignmentDto>> GetEmployeeShiftAssignmentsAsync(Guid employeeId, CancellationToken ct = default);
    Task<List<ShiftAssignmentDto>> GetActiveShiftAssignmentsAsync(CancellationToken ct = default);
    Task UnassignShiftAsync(Guid id, CancellationToken ct = default);

    // Employee Shift
    Task<LocalShift?> GetEmployeeShiftAsync(Guid employeeId, DateTime date, CancellationToken ct = default);
    Task<ShiftDto> GetEmployeeCurrentShiftAsync(Guid employeeId, CancellationToken ct = default);
}