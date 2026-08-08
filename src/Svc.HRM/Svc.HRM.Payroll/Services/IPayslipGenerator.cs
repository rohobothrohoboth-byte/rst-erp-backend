using Svc.HRM.Payroll.Models.DTOs;

namespace Svc.HRM.Payroll.Services;

public interface IPayslipGenerator
{
    Task<PayslipDto> GeneratePayslipAsync(Guid payrollEmployeeId, CancellationToken ct = default);
}