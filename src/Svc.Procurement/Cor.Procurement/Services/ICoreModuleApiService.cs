using Cor.Procurement.Models.DTOs;

namespace Cor.Procurement.Services;

public interface ICoreModuleApiService
{
    Task<List<CompanyDto>> GetAllCompaniesAsync(CancellationToken ct = default);
    Task<List<BranchDto>> GetAllBranchesAsync(CancellationToken ct = default);
    Task<List<DepartmentDto>> GetAllDepartmentsAsync(CancellationToken ct = default);
}
