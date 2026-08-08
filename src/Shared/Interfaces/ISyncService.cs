// Shared/Interfaces/ISyncService.cs
public interface ISyncService
{
    Task SyncCompanyAsync(LocalCompany company, CancellationToken ct);
    Task SyncBranchAsync(LocalBranch branch, CancellationToken ct);
    Task SyncDepartmentAsync(LocalDepartment department, CancellationToken ct);
    Task SyncPositionAsync(LocalPosition position, CancellationToken ct);
    Task SyncJobGradeAsync(LocalJobGrade jobGrade, CancellationToken ct);
    Task SyncJgStepAsync(LocalJgStep jgStep, CancellationToken ct);
    Task SyncEmployeeAsync(LocalEmployee employee, CancellationToken ct);
}