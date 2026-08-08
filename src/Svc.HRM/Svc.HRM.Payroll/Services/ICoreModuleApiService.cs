using Svc.HRM.Payroll.Models.Entities.Local;
using Microsoft.Extensions.Logging;
using Svc.HRM.Payroll.Models.DTOs;
namespace Svc.HRM.Payroll.Services;

public interface ICoreModuleApiService
{


       Task<List<CompanyDto>> GetAllCompaniesAsync(CancellationToken ct = default);
         Task<List<BranchDto>> GetAllBranchesAsync(CancellationToken ct = default);
         Task<List<DepartmentDto>> GetAllDepartmentsAsync(CancellationToken ct = default);
}