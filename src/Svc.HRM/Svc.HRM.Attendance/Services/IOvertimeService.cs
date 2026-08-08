using Svc.HRM.Attendance.Models.DTOs;

namespace Svc.HRM.Attendance.Services;

public interface IOvertimeService
{
    // Overtime Request
    Task<OvertimeDto> CreateOvertimeRequestAsync(OvertimeRequestDto dto, CancellationToken ct = default);
    Task<OvertimeDto> UpdateOvertimeRequestAsync(Guid id, OvertimeRequestDto dto, CancellationToken ct = default);
    Task<OvertimeDto> GetOvertimeRequestAsync(Guid id, CancellationToken ct = default);
    Task<List<OvertimeDto>> GetEmployeeOvertimeRequestsAsync(Guid employeeId, CancellationToken ct = default);
    Task<List<OvertimeDto>> GetPendingOvertimeRequestsAsync(CancellationToken ct = default);
    Task<OvertimeDto> ApproveOvertimeRequestAsync(Guid id, OvertimeApproveDto dto, CancellationToken ct = default);
    Task<OvertimeDto> RejectOvertimeRequestAsync(Guid id, string reason, string? rejectedBy = null, CancellationToken ct = default);
    Task DeleteOvertimeRequestAsync(Guid id, CancellationToken ct = default);
}