using Leave.Domain.Entities.Local;

namespace Leave.App.Services;

public interface ICoreModuleApiService
{
    Task<List<LocalCompany>> GetAllCompaniesAsync(CancellationToken ct = default);
    Task<List<LocalBranch>> GetAllBranchesAsync(CancellationToken ct = default);
    Task<List<LocalDepartment>> GetAllDepartmentsAsync(CancellationToken ct = default);
}