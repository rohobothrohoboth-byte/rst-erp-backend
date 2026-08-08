using Cor.Finance.Models.DTOs;

namespace Cor.Finance.Services;

public interface ICoreModuleApiService
{
    Task<List<CompanyDto>> GetAllCompaniesAsync(CancellationToken ct = default);
    Task<List<BranchDto>> GetAllBranchesAsync(CancellationToken ct = default);
    Task<List<DepartmentDto>> GetAllDepartmentsAsync(CancellationToken ct = default);
}
