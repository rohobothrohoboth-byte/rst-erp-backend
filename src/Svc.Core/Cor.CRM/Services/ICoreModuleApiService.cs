using Cor.CRM.Models.DTOs;

namespace Cor.CRM.Services;

public interface ICoreModuleApiService
{
    Task<List<LocalCompanyDto>> GetAllCompaniesAsync(CancellationToken ct = default);
    Task<List<BranchDto>> GetAllBranchesAsync(CancellationToken ct = default);
    Task<List<DepartmentDto>> GetAllDepartmentsAsync(CancellationToken ct = default);
}
