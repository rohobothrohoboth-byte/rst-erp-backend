using Svc.HRM.Attendance.Models.DTOs;
using Svc.HRM.Attendance.Models.Entities.Local;
namespace Svc.HRM.Attendance.Services;

public interface ISyncService
{
    // Core Module Sync (Company, Branch, Department)
    Task SyncCompanyAsync(CompanyDto company, CancellationToken ct = default);
    Task SyncBranchAsync(BranchDto branch, CancellationToken ct = default);
    Task SyncDepartmentAsync(DepartmentDto department, CancellationToken ct = default);
    
    // Core HRMM Sync (Position, JobGrade, JgStep) - ADD THESE
    // Position sync - Accept LocalPosition entity
       Task SyncPositionAsync(LocalPosition position, CancellationToken ct = default);

       // JobGrade sync - Accept LocalJobGrade entity
       Task SyncJobGradeAsync(LocalJobGrade jobGrade, CancellationToken ct = default);

    // HRM.Profile Sync (Employee)
    Task SyncEmployeeAsync(LocalEmployee employee, CancellationToken ct = default);

    // Bulk sync for initial load
    Task BulkSyncCompaniesAsync(List<CompanyDto> companies, CancellationToken ct = default);
    Task BulkSyncBranchesAsync(List<BranchDto> branches, CancellationToken ct = default);
    Task BulkSyncDepartmentsAsync(List<DepartmentDto> departments, CancellationToken ct = default);
    Task BulkSyncEmployeesAsync(List<LocalEmployee> employees, CancellationToken ct = default);

    // Soft delete
    Task SoftDeleteCompanyAsync(Guid id, CancellationToken ct = default);
    Task SoftDeleteBranchAsync(Guid id, CancellationToken ct = default);
    Task SoftDeleteDepartmentAsync(Guid id, CancellationToken ct = default);
    Task SoftDeleteEmployeeAsync(Guid id, CancellationToken ct = default);
}