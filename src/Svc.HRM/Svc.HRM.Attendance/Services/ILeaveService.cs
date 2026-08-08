using Svc.HRM.Attendance.Models.DTOs;

namespace Svc.HRM.Attendance.Services;

public interface ILeaveService
{
    // Leave Request
    Task<LeaveRequestDto> CreateLeaveRequestAsync(LeaveRequestCreateDto dto, CancellationToken ct = default);
    Task<LeaveRequestDto> UpdateLeaveRequestAsync(Guid id, LeaveRequestCreateDto dto, CancellationToken ct = default);
    Task<LeaveRequestDto> GetLeaveRequestAsync(Guid id, CancellationToken ct = default);
    Task<List<LeaveRequestDto>> GetEmployeeLeaveRequestsAsync(Guid employeeId, CancellationToken ct = default);
    Task<List<LeaveRequestDto>> GetPendingLeaveRequestsAsync(CancellationToken ct = default);
    Task<LeaveRequestDto> ApproveLeaveRequestAsync(Guid id, LeaveApproveDto dto, CancellationToken ct = default);
    Task<LeaveRequestDto> RejectLeaveRequestAsync(Guid id, string reason, string? rejectedBy = null, CancellationToken ct = default);
    Task DeleteLeaveRequestAsync(Guid id, CancellationToken ct = default);

    // Leave Balance
    Task<LeaveBalanceDto> GetLeaveBalanceAsync(Guid employeeId, string leaveType, int year, CancellationToken ct = default);
    Task<List<LeaveBalanceDto>> GetEmployeeLeaveBalancesAsync(Guid employeeId, int year, CancellationToken ct = default);
    Task InitializeLeaveBalanceAsync(Guid employeeId, int year, CancellationToken ct = default);
    Task UpdateLeaveBalanceAsync(Guid employeeId, string leaveType, double daysUsed, CancellationToken ct = default);

    // Leave Calendar
    Task<List<LeaveRequestDto>> GetLeaveCalendarAsync(int year, int? month = null, CancellationToken ct = default);
    Task<List<LeaveRequestDto>> GetDepartmentLeaveCalendarAsync(Guid departmentId, int year, int? month = null, CancellationToken ct = default);
}