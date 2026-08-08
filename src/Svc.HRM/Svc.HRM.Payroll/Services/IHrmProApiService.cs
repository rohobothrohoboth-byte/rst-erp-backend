using Svc.HRM.Payroll.Models.DTOs;

namespace Svc.HRM.Payroll.Services;

public interface IHrmProApiService
{
    // Return DTOs - this matches your implementation
    Task<List<EmployeeDto>> GetAllEmployeesAsync(CancellationToken ct = default);
    Task<EmployeeDto?> GetEmployeeAsync(Guid id, CancellationToken ct = default);
    Task<bool> UpdateEmployeeAppUserIdAsync(Guid employeeId, string appUserId, CancellationToken ct = default);
}