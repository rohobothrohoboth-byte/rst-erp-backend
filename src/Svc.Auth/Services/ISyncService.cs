using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Services;

public interface ISyncService
{
    Task SyncCompanyAsync(CompanyDto company, CancellationToken ct = default);
    Task SyncBranchAsync(BranchDto branch, CancellationToken ct = default);
    Task SyncDepartmentAsync(DepartmentDto dept, CancellationToken ct = default);
    Task SyncPositionAsync(PositionDto position, CancellationToken ct = default);
    Task SyncJobGradeAsync(JobGradeDto jobGrade, CancellationToken ct = default);
    Task SyncEmployeeAsync(EmployeeDto employee, CancellationToken ct = default);
    Task SoftDeleteBranchAsync(Guid id, CancellationToken ct = default);
    Task SoftDeleteDepartmentAsync(Guid id, CancellationToken ct = default);
    Task SyncWithRetryAsync(Func<Task> syncAction, int maxRetries = 3);
}