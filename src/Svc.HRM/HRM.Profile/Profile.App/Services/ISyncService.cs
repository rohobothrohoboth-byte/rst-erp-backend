using Profile.Domain.Entities.Local;
using Microsoft.Extensions.Logging;
namespace Profile.App.Services;

public interface ISyncService
{
    Task SyncCompanyAsync(LocalCompany company, CancellationToken ct = default);
    Task SyncBranchAsync(LocalBranch branch, CancellationToken ct = default);
    Task SyncDepartmentAsync(LocalDepartment dept, CancellationToken ct = default);
    Task SyncPositionAsync(LocalPosition position, CancellationToken ct = default);
    Task SyncJobGradeAsync(LocalJobGrade jobGrade, CancellationToken ct = default);
    Task SyncJgStepAsync(LocalJgStep jgStep, CancellationToken ct = default);

}