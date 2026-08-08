using Cor.HRMM.Models.Entities.Local;

namespace Cor.HRMM.Services;

public interface ISyncService
{
    Task SyncCompanyAsync(LocalCompany company, CancellationToken ct = default);
    Task SyncBranchAsync(LocalBranch branch, CancellationToken ct = default);
    Task SyncDepartmentAsync(LocalDepartment dept, CancellationToken ct = default);

}
