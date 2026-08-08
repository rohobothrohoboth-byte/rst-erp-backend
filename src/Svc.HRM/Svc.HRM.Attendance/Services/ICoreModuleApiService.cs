using Svc.HRM.Attendance.Models.DTOs;
using Svc.HRM.Attendance.Models.Entities.Local;
namespace Svc.HRM.Attendance.Services;

public interface ICoreModuleApiService
{
    Task<List<CompanyDto>> GetAllCompaniesAsync(CancellationToken ct = default);
    Task<List<BranchDto>> GetAllBranchesAsync(CancellationToken ct = default);
    Task<List<DepartmentDto>> GetAllDepartmentsAsync(CancellationToken ct = default);
}
