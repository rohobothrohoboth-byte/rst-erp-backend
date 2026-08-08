using Leave.Domain.Entities.Local;
using Microsoft.Extensions.Logging;
using Leave.Domain.DTOs;
namespace Leave.App.Services;

public interface ISyncService
{
    Task SyncCompanyAsync(LocalCompany company, CancellationToken ct = default);
    Task SyncBranchAsync(LocalBranch branch, CancellationToken ct = default);
    Task SyncDepartmentAsync(LocalDepartment dept, CancellationToken ct = default);
    Task SyncPositionAsync(LocalPosition position, CancellationToken ct = default);
    Task SyncJobGradeAsync(LocalJobGrade jobGrade, CancellationToken ct = default);
    Task SyncJgStepAsync(LocalJgStep jgStep, CancellationToken ct = default);
     Task SyncPositionRequirementAsync(PositionReqDto positionReq, CancellationToken ct = default);
Task UpdatePositionWithRequirementsAsync(Guid positionId, PositionReqDto positionReq, CancellationToken ct = default);
Task SyncEmployeeAsync(LocalEmployee employee, CancellationToken ct = default);
}