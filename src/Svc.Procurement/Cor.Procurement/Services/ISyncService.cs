using Cor.Procurement.Models.DTOs;
using Cor.Procurement.Models.Entities.Local;
namespace Cor.Procurement.Services;

public interface ISyncService
{
    // Company sync
    Task SyncCompanyAsync(CompanyDto company, CancellationToken ct = default);
    Task BulkSyncCompaniesAsync(List<CompanyDto> companies, CancellationToken ct = default);
    Task SoftDeleteCompanyAsync(Guid id, CancellationToken ct = default);

    // Branch sync
    Task SyncBranchAsync(BranchDto branch, CancellationToken ct = default);
    Task BulkSyncBranchesAsync(List<BranchDto> branches, CancellationToken ct = default);
    Task SoftDeleteBranchAsync(Guid id, CancellationToken ct = default);

    // Department sync
    Task SyncDepartmentAsync(DepartmentDto department, CancellationToken ct = default);
    Task BulkSyncDepartmentsAsync(List<DepartmentDto> departments, CancellationToken ct = default);
    Task SoftDeleteDepartmentAsync(Guid id, CancellationToken ct = default);

    // Position sync
    Task SyncPositionAsync(LocalPosition position, CancellationToken ct = default);

    // JobGrade sync
    Task SyncJobGradeAsync(LocalJobGrade jobGrade, CancellationToken ct = default);

    // Employee sync
    Task SyncEmployeeAsync(LocalEmployee employee, CancellationToken ct = default);
    Task BulkSyncEmployeesAsync(List<LocalEmployee> employees, CancellationToken ct = default);
    Task SoftDeleteEmployeeAsync(Guid id, CancellationToken ct = default);

    // Full sync
    Task FullSyncAsync(CancellationToken ct = default);





        Task SyncFinancialPeriodAsync(FinancialPeriodDto period, CancellationToken ct = default);
        Task BulkSyncFinancialPeriodsAsync(List<FinancialPeriodDto> periods, CancellationToken ct = default);
        Task SoftDeleteFinancialPeriodAsync(Guid id, CancellationToken ct = default);

        // Vendor sync
        Task SyncVendorAsync(VendorDto vendor, CancellationToken ct = default);
        Task BulkSyncVendorsAsync(List<VendorDto> vendors, CancellationToken ct = default);
        Task SoftDeleteVendorAsync(Guid id, CancellationToken ct = default);


         Task SyncWarehouseAsync(WarehouseDto warehouse, CancellationToken ct = default);
            Task BulkSyncWarehousesAsync(List<WarehouseDto> warehouses, CancellationToken ct = default);
            Task SoftDeleteWarehouseAsync(Guid id, CancellationToken ct = default);
}