using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Services;

public interface ICoreModuleApiService
{
    Task<List<CompanyDto>> GetAllCompaniesAsync(CancellationToken ct = default);
    Task<List<BranchDto>> GetAllBranchesAsync(CancellationToken ct = default);
    Task<List<DepartmentDto>> GetAllDepartmentsAsync(CancellationToken ct = default);
    Task<BranchDto?> GetBranchAsync(Guid id, CancellationToken ct = default);
    Task<DepartmentDto?> GetDepartmentAsync(Guid id, CancellationToken ct = default);
}