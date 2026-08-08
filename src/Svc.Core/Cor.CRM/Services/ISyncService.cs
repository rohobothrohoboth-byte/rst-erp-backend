using Cor.CRM.Models.DTOs;
using Cor.CRM.Models.Entities;
using Task = System.Threading.Tasks.Task;
using Cor.CRM.Models.Entities.Local;
namespace Cor.CRM.Services;

public interface ISyncService
{
    // Core Module Sync (Company, Branch, Department)
    Task SyncCompanyAsync(LocalCompanyDto company, CancellationToken ct = default);
    Task SyncBranchAsync(BranchDto branch, CancellationToken ct = default);
    Task SyncDepartmentAsync(DepartmentDto department, CancellationToken ct = default);

    // Core HRMM Sync (Position, JobGrade)
    Task SyncPositionAsync(LocalPosition position, CancellationToken ct = default);
    Task SyncJobGradeAsync(LocalJobGrade jobGrade, CancellationToken ct = default);

    // HRM.Profile Sync (Employee)
    Task SyncEmployeeAsync(LocalEmployee employee, CancellationToken ct = default);

    // Lead & Customer Sync
    Task SyncLeadAsync(Lead lead, CancellationToken ct = default);
    Task SyncCustomerAsync(Customer customer, CancellationToken ct = default);
   Task SoftDeletePositionAsync(Guid id, CancellationToken ct = default);
    Task SoftDeleteJobGradeAsync(Guid id, CancellationToken ct = default);
    // Bulk sync for initial load
    Task BulkSyncCompaniesAsync(List<LocalCompanyDto> companies, CancellationToken ct = default);
    Task BulkSyncBranchesAsync(List<BranchDto> branches, CancellationToken ct = default);
    Task BulkSyncDepartmentsAsync(List<DepartmentDto> departments, CancellationToken ct = default);
    Task BulkSyncEmployeesAsync(List<LocalEmployee> employees, CancellationToken ct = default);
    Task BulkSyncLeadsAsync(List<Lead> leads, CancellationToken ct = default);
    Task BulkSyncCustomersAsync(List<Customer> customers, CancellationToken ct = default);

    // Soft delete
    Task SoftDeleteCompanyAsync(Guid id, CancellationToken ct = default);
    Task SoftDeleteBranchAsync(Guid id, CancellationToken ct = default);
    Task SoftDeleteDepartmentAsync(Guid id, CancellationToken ct = default);
    Task SoftDeleteEmployeeAsync(Guid id, CancellationToken ct = default);
    Task SoftDeleteLeadAsync(Guid id, CancellationToken ct = default);
    Task SoftDeleteCustomerAsync(Guid id, CancellationToken ct = default);
}