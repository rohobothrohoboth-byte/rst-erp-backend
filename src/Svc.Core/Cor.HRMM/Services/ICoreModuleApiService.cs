using Cor.HRMM.Models.Entities.Local;

namespace Cor.HRMM.Services;

public interface ICoreModuleApiService
{
    Task<List<LocalCompany>> GetAllCompaniesAsync(CancellationToken ct = default);
    Task<List<LocalBranch>> GetAllBranchesAsync(CancellationToken ct = default);
    Task<List<LocalDepartment>> GetAllDepartmentsAsync(CancellationToken ct = default);
}
