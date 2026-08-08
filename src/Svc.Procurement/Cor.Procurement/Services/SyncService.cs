using Cor.Procurement.Models.DTOs;
using Cor.Procurement.Models.Entities.Local;
using Cor.Procurement.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shared.Helpers.Services;
using Cor.Procurement.Models.Enums;
using System.Text.Json;
namespace Cor.Procurement.Services;



public class SyncResult
{
    public bool HasChanges { get; set; }
    public int CompaniesSynced { get; set; }
    public int BranchesSynced { get; set; }
    public int DepartmentsSynced { get; set; }
    public int PositionsSynced { get; set; }
    public int JobGradesSynced { get; set; }
    public int EmployeesSynced { get; set; }
    public DateTime SyncTime { get; set; } = DateTime.UtcNow;
    public string? ErrorMessage { get; set; }
    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);
}

public class SyncService : ISyncService
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<SyncService> _logger;
    private readonly string _connectionString;
    private readonly ICacheService _cache;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;  // ? Add this

    public SyncService(
        IConfiguration configuration,
        ProcurementDbContext context,
        ILogger<SyncService> logger,
        ICacheService cache,
        IServiceProvider serviceProvider)  // ? Add serviceProvider parameter
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection connection string not found");
        _logger = logger;
        _cache = cache;
        _context = context;
        _configuration = configuration;
        _serviceProvider = serviceProvider;  // ? Initialize
    }

    // ============================================================
    // ? FULL SYNC
    // ============================================================

 public async Task FullSyncAsync(CancellationToken ct = default)
 {
     _logger.LogInformation("?? Starting full sync from all services...");

     using var scope = _serviceProvider.CreateScope();
     var coreApi = scope.ServiceProvider.GetRequiredService<ICoreModuleApiService>();
     var hrmmApi = scope.ServiceProvider.GetRequiredService<ICoreHrmmApiService>();
     var hrmProApi = scope.ServiceProvider.GetRequiredService<IHrmProApiService>();
     var financeApi = scope.ServiceProvider.GetRequiredService<IFinanceApiService>();
      var inventoryApi = scope.ServiceProvider.GetRequiredService<IInventoryApiService>();

     try
     {
         // ? Parallel API calls (these are thread-safe)
         var companiesTask = coreApi.GetAllCompaniesAsync(ct);
         var branchesTask = coreApi.GetAllBranchesAsync(ct);
         var departmentsTask = coreApi.GetAllDepartmentsAsync(ct);
         var positionsTask = hrmmApi.GetAllPositionsAsync(ct);
         var jobGradesTask = hrmmApi.GetAllJobGradesAsync(ct);
         var employeesTask = hrmProApi.GetAllEmployeesAsync(ct);

         await Task.WhenAll(companiesTask, branchesTask, departmentsTask,
                            positionsTask, jobGradesTask, employeesTask);

         var companies = await companiesTask;
         var branches = await branchesTask;
         var departments = await departmentsTask;
         var positions = await positionsTask;
         var jobGrades = await jobGradesTask;
         var employees = await employeesTask;

         // ? Sync companies sequentially (or with separate DbContext)
         _logger.LogInformation("?? Syncing companies...");
         foreach (var company in companies)
         {
             await SyncCompanyAsync(company, ct);
         }
         _logger.LogInformation("? Synced {Count} companies", companies.Count);

         // ? Sync branches
         _logger.LogInformation("?? Syncing branches...");
         foreach (var branch in branches)
         {
             await SyncBranchAsync(branch, ct);
         }
         _logger.LogInformation("? Synced {Count} branches", branches.Count);

         // ? Sync departments
         _logger.LogInformation("?? Syncing departments...");
         foreach (var dept in departments)
         {
             await SyncDepartmentAsync(dept, ct);
         }
         _logger.LogInformation("? Synced {Count} departments", departments.Count);

         // ? Sync positions
         _logger.LogInformation("?? Syncing positions...");
         foreach (var positionDto in positions)
         {
             if (string.IsNullOrEmpty(positionDto.Name) || string.IsNullOrEmpty(positionDto.NameAm))
             {
                 _logger.LogWarning("?? Skipping position {PositionId} - missing Name or NameAm", positionDto.Id);
                 continue;
             }
             var localPosition = MapToLocalPosition(positionDto);
             await SyncPositionAsync(localPosition, ct);
         }
         _logger.LogInformation("? Synced {Count} positions", positions.Count);

         // ? Sync job grades
         _logger.LogInformation("?? Syncing job grades...");
         foreach (var jobGradeDto in jobGrades)
         {
             if (string.IsNullOrEmpty(jobGradeDto.Name))
             {
                 _logger.LogWarning("?? Skipping job grade {JobGradeId} - missing Name", jobGradeDto.Id);
                 continue;
             }
             var localJobGrade = MapToLocalJobGrade(jobGradeDto);
             await SyncJobGradeAsync(localJobGrade, ct);
         }
         _logger.LogInformation("? Synced {Count} job grades", jobGrades.Count);

         // ? Sync employees
         _logger.LogInformation("?? Syncing employees...");
         foreach (var employeeDto in employees)
         {
             var localEmployee = MapToLocalEmployee(employeeDto);
             await SyncEmployeeAsync(localEmployee, ct);
         }
         _logger.LogInformation("? Synced {Count} employees", employees.Count);
_logger.LogInformation("🔄 Syncing financial periods from Finance module...");
        var financialPeriods = await financeApi.GetAllFinancialPeriodsAsync(ct);
        foreach (var period in financialPeriods)
        {
            await SyncFinancialPeriodAsync(period, ct);
        }
        _logger.LogInformation("✅ Synced {Count} financial periods from Finance module", financialPeriods.Count);

        // Vendors
        _logger.LogInformation("🔄 Syncing vendors from Finance module...");
        var vendors = await financeApi.GetAllVendorsAsync(ct);
        foreach (var vendor in vendors)
        {
            await SyncVendorAsync(vendor, ct);
        }
        _logger.LogInformation("✅ Synced {Count} vendors from Finance module", vendors.Count);
         _logger.LogInformation("🔄 Syncing warehouses from Inventory module...");
                var warehouses = await inventoryApi.GetAllWarehousesAsync(ct);
                foreach (var warehouse in warehouses)
                {
                    await SyncWarehouseAsync(warehouse, ct);
                }
                _logger.LogInformation("✅ Synced {Count} warehouses from Inventory module", warehouses.Count);

         _logger.LogInformation("=========================================");
         _logger.LogInformation("? Full sync completed successfully!");
         _logger.LogInformation("Total records synced:");
         _logger.LogInformation("  Companies:   {Companies}", companies.Count);
         _logger.LogInformation("  Branches:    {Branches}", branches.Count);
         _logger.LogInformation("  Departments: {Departments}", departments.Count);
         _logger.LogInformation("  Positions:   {Positions}", positions.Count);
         _logger.LogInformation("  JobGrades:   {JobGrades}", jobGrades.Count);
         _logger.LogInformation("  Employees:   {Employees}", employees.Count);
         _logger.LogInformation("=========================================");
     }
     catch (Exception ex)
     {
         _logger.LogError(ex, "? Full sync failed");
         throw;
     }
 }

    // ============================================================
    // ? MAPPING METHODS
    // ============================================================

    private LocalPosition MapToLocalPosition(PositionDto dto)
    {
        return new LocalPosition
        {
            Id = dto.Id,
            Name = dto.Name ?? string.Empty,
            NameAm = dto.NameAm ?? string.Empty,
            NoOfPosition = dto.NoOfPosition,
            IsVacant = dto.IsVacant ?? "Unknown",
            DepartmentId = dto.DepartmentId,
            JobGradeId = dto.JobGradeId
        };
    }

    private LocalJobGrade MapToLocalJobGrade(JobGradeDto dto)
    {
        return new LocalJobGrade
        {
            Id = dto.Id,
            Name = dto.Name ?? string.Empty,
            StartSalary = dto.StartSalary,
            MaxSalary = dto.MaxSalary
        };
    }

   private LocalEmployee MapToLocalEmployee(EmployeeDto dto)
   {
       return new LocalEmployee
       {
           Id = dto.Id,
           Code = dto.Code ?? string.Empty,
           FirstName = dto.FirstName ?? string.Empty,
           FirstNameAm = dto.FirstNameAm ?? string.Empty,
           MiddleName = dto.MiddleName ?? string.Empty,
           MiddleNameAm = dto.MiddleNameAm ?? string.Empty,
           LastName = dto.LastName ?? string.Empty,
           LastNameAm = dto.LastNameAm ?? string.Empty,
           Gender = dto.Gender ?? string.Empty,
           Nationality = dto.Nationality ?? string.Empty,
           Email = dto.Email ?? string.Empty,
           Phone = dto.Phone ?? string.Empty,
           PersonId = dto.PersonId,
           PositionId = dto.PositionId,
           DepartmentId = dto.DepartmentId,
           JobGradeId = dto.JobGradeId,
           AppUserId = dto.AppUserId,
           BranchId = dto.BranchId,
           EmpState = dto.EmpState ?? "Active",
           EmploymentType = dto.EmploymentType ?? "FullTime",
           EmploymentNature = dto.EmploymentNature ?? "Permanent",
           WorkArrangement = dto.WorkArrangement ?? "OnSite",
           EmploymentDate = dto.EmploymentDate,
           // ✅ FIX: Handle null IsActive
           IsActive = dto.IsActive , // Default to true if null
           DateAdd = DateTime.UtcNow,
           SyncedAt = DateTime.UtcNow,
           IsDeleted = false,
           Position = null!,
           Department = null!,
           JobGrade = null!
       };
   }

    // ============================================================
    // ? ORIGINAL METHODS
    // ============================================================
// ============================================================
// WAREHOUSE SYNC METHODS
// ============================================================

public async Task SyncWarehouseAsync(WarehouseDto warehouse, CancellationToken ct = default)
{
    try
    {
        var existing = await _context.LocalWarehouses
            .FirstOrDefaultAsync(x => x.Id == warehouse.Id, ct);

        if (existing == null)
        {
            await _context.LocalWarehouses.AddAsync(new LocalWarehouse
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                Code = warehouse.Code,
                Location = warehouse.Location,
                Address = warehouse.Address,
                City = warehouse.City,
                State = warehouse.State,
                Country = warehouse.Country,
                ZipCode = warehouse.ZipCode,
                Phone = warehouse.Phone,
                Email = warehouse.Email,
                WarehouseType = warehouse.WarehouseType,
                Status = warehouse.Status ?? "Active",
                IsActive = warehouse.IsActive,
                SyncedAt = DateTime.UtcNow,
                SourceId = warehouse.SourceId,
                DateAdd = DateTime.UtcNow
            }, ct);
            _logger.LogInformation("Synced new warehouse: {WarehouseName} ({WarehouseCode})", warehouse.Name, warehouse.Code);
        }
        else
        {
            existing.Name = warehouse.Name;
            existing.Code = warehouse.Code;
            existing.Location = warehouse.Location;
            existing.Address = warehouse.Address;
            existing.City = warehouse.City;
            existing.State = warehouse.State;
            existing.Country = warehouse.Country;
            existing.ZipCode = warehouse.ZipCode;
            existing.Phone = warehouse.Phone;
            existing.Email = warehouse.Email;
            existing.WarehouseType = warehouse.WarehouseType;
            existing.Status = warehouse.Status ?? "Active";
            existing.IsActive = warehouse.IsActive;
            existing.SyncedAt = DateTime.UtcNow;
            existing.SourceId = warehouse.SourceId;
            existing.DateMod = DateTime.UtcNow;
            _logger.LogInformation("Updated warehouse: {WarehouseName} ({WarehouseCode})", warehouse.Name, warehouse.Code);
        }

        await _context.SaveChangesAsync(ct);

        // Invalidate cache
        await _cache.RemoveAsync($"warehouse_{warehouse.Id}", ct);
        await _cache.RemoveAsync($"warehouse_code_{warehouse.Code}", ct);
        await _cache.RemoveAsync("warehouses_all", ct);
        await _cache.RemoveAsync("warehouses_active", ct);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error syncing warehouse: {WarehouseName} ({WarehouseCode})", warehouse.Name, warehouse.Code);
        throw;
    }
}

public async Task BulkSyncWarehousesAsync(List<WarehouseDto> warehouses, CancellationToken ct = default)
{
    try
    {
        _logger.LogInformation("Starting bulk sync for {Count} warehouses", warehouses.Count);

        foreach (var warehouse in warehouses)
        {
            await SyncWarehouseAsync(warehouse, ct);
        }

        _logger.LogInformation("Completed bulk sync for {Count} warehouses", warehouses.Count);
        await _cache.RemoveAsync("warehouses_all", ct);
        await _cache.RemoveAsync("warehouses_active", ct);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error in bulk sync for warehouses");
        throw;
    }
}

public async Task SoftDeleteWarehouseAsync(Guid id, CancellationToken ct = default)
{
    var warehouse = await _context.LocalWarehouses.FirstOrDefaultAsync(x => x.Id == id, ct);
    if (warehouse != null)
    {
        warehouse.IsDeleted = true;
        warehouse.SyncedAt = DateTime.UtcNow;
        warehouse.DateMod = DateTime.UtcNow;
        await _context.SaveChangesAsync(ct);

        await _cache.RemoveAsync($"warehouse_{id}", ct);
        await _cache.RemoveAsync($"warehouse_code_{warehouse.Code}", ct);
        await _cache.RemoveAsync("warehouses_all", ct);
        await _cache.RemoveAsync("warehouses_active", ct);
        _logger.LogInformation("Soft deleted warehouse: {WarehouseId} ({WarehouseCode})", id, warehouse.Code);
    }
}
    public async Task SyncCompanyAsync(CompanyDto company, CancellationToken ct = default)
    {
        try
        {
            var existing = await _context.LocalCompanies
                .FirstOrDefaultAsync(x => x.Id == company.Id, ct);

            if (existing == null)
            {
                await _context.LocalCompanies.AddAsync(new LocalCompany
                {
                    Id = company.Id,
                    Name = company.Name,
                    NameAm = company.NameAm,
                    TaxId = company.TaxId,
                    Phone = company.Phone,
                    Email = company.Email,
                    Address = company.Address,
                    SyncedAt = DateTime.UtcNow,
                    IsDeleted = false
                }, ct);
                _logger.LogInformation("Synced new company: {CompanyName}", company.Name);
            }
            else
            {
                existing.Name = company.Name;
                existing.NameAm = company.NameAm;
                existing.TaxId = company.TaxId;
                existing.Phone = company.Phone;
                existing.Email = company.Email;
                existing.Address = company.Address;
                existing.SyncedAt = DateTime.UtcNow;
                existing.IsDeleted = false;
                _logger.LogInformation("Updated company: {CompanyName}", company.Name);
            }

            await _context.SaveChangesAsync(ct);
            await _cache.RemoveAsync($"company_{company.Id}", ct);
            await _cache.RemoveAsync("companies_all", ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing company: {CompanyName}", company.Name);
            throw;
        }
    }

    public async Task BulkSyncCompaniesAsync(List<CompanyDto> companies, CancellationToken ct = default)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(ct);

        const string sql = @"
            INSERT INTO ""LocalCompanies"" (""Id"", ""Name"", ""NameAm"", ""TaxId"",
                ""Phone"", ""Email"", ""Address"", ""SyncedAt"", ""IsDeleted"")
            VALUES (@Id, @Name, @NameAm, @TaxId, @Phone, @Email, @Address, @SyncedAt, false)
            ON CONFLICT (""Id"") DO UPDATE SET
                ""Name"" = EXCLUDED.""Name"",
                ""NameAm"" = EXCLUDED.""NameAm"",
                ""TaxId"" = EXCLUDED.""TaxId"",
                ""Phone"" = EXCLUDED.""Phone"",
                ""Email"" = EXCLUDED.""Email"",
                ""Address"" = EXCLUDED.""Address"",
                ""SyncedAt"" = EXCLUDED.""SyncedAt"",
                ""IsDeleted"" = false";

        await connection.ExecuteAsync(sql, companies.Select(c => new
        {
            c.Id,
            c.Name,
            c.NameAm,
            c.TaxId,
            c.Phone,
            c.Email,
            c.Address,
            SyncedAt = DateTime.UtcNow
        }));
    }

    public async Task SoftDeleteCompanyAsync(Guid id, CancellationToken ct = default)
    {
        var company = await _context.LocalCompanies.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (company != null)
        {
            company.IsDeleted = true;
            company.SyncedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
            await _cache.RemoveAsync($"company_{id}", ct);
            await _cache.RemoveAsync("companies_all", ct);
            _logger.LogInformation("Soft deleted company: {CompanyId}", id);
        }
    }

    public async Task SyncBranchAsync(BranchDto branch, CancellationToken ct = default)
    {
        try
        {
            var existing = await _context.LocalBranches
                .FirstOrDefaultAsync(x => x.Id == branch.Id, ct);

            if (existing == null)
            {
                await _context.LocalBranches.AddAsync(new LocalBranch
                {
                    Id = branch.Id,
                    Name = branch.Name,
                    NameAm = branch.NameAm,
                    Code = branch.Code,
                    Location = branch.Location,
                    CompId = branch.CompId,
                    SyncedAt = DateTime.UtcNow,
                    IsDeleted = false
                }, ct);
                _logger.LogInformation("Synced new branch: {BranchName}", branch.Name);
            }
            else
            {
                existing.Name = branch.Name;
                existing.NameAm = branch.NameAm;
                existing.Code = branch.Code;
                existing.Location = branch.Location;
                existing.CompId = branch.CompId;
                existing.SyncedAt = DateTime.UtcNow;
                existing.IsDeleted = false;
                _logger.LogInformation("Updated branch: {BranchName}", branch.Name);
            }

            await _context.SaveChangesAsync(ct);
            await _cache.RemoveAsync($"branch_{branch.Id}", ct);
            await _cache.RemoveAsync("branches_all", ct);
            if (branch.CompId != Guid.Empty)
            {
                await _cache.RemoveAsync($"branches_by_company_{branch.CompId}", ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing branch: {BranchName}", branch.Name);
            throw;
        }
    }

    public async Task BulkSyncBranchesAsync(List<BranchDto> branches, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Starting bulk sync for {Count} branches", branches.Count);

            foreach (var branch in branches)
            {
                await SyncBranchAsync(branch, ct);
            }

            _logger.LogInformation("Completed bulk sync for {Count} branches", branches.Count);
            await _cache.RemoveAsync("branches_all", ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in bulk sync for branches");
            throw;
        }
    }

    public async Task SoftDeleteBranchAsync(Guid id, CancellationToken ct = default)
    {
        var branch = await _context.LocalBranches.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (branch != null)
        {
            branch.IsDeleted = true;
            branch.SyncedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
            await _cache.RemoveAsync($"branch_{id}", ct);
            await _cache.RemoveAsync("branches_all", ct);
            if (branch.CompId.HasValue)
            {
                await _cache.RemoveAsync($"branches_by_company_{branch.CompId.Value}", ct);
            }
            _logger.LogInformation("Soft deleted branch: {BranchId}", id);
        }
    }

    public async Task SyncDepartmentAsync(DepartmentDto department, CancellationToken ct = default)
    {
        try
        {
            var existing = await _context.LocalDepartments
                .FirstOrDefaultAsync(x => x.Id == department.Id, ct);

            if (existing == null)
            {
                await _context.LocalDepartments.AddAsync(new LocalDepartment
                {
                    Id = department.Id,
                    Name = department.Name,
                    NameAm = department.NameAm,
                    BranchId = department.BranchId,
                    SyncedAt = DateTime.UtcNow,
                    IsDeleted = false
                }, ct);
                _logger.LogInformation("Synced new department: {DepartmentName}", department.Name);
            }
            else
            {
                existing.Name = department.Name;
                existing.NameAm = department.NameAm;
                existing.BranchId = department.BranchId;
                existing.SyncedAt = DateTime.UtcNow;
                existing.IsDeleted = false;
                _logger.LogInformation("Updated department: {DepartmentName}", department.Name);
            }

            await _context.SaveChangesAsync(ct);
            await _cache.RemoveAsync($"department_{department.Id}", ct);
            await _cache.RemoveAsync("departments_all", ct);
            if (department.BranchId != Guid.Empty)
            {
                await _cache.RemoveAsync($"departments_by_branch_{department.BranchId}", ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing department: {DepartmentName}", department.Name);
            throw;
        }
    }

    public async Task BulkSyncDepartmentsAsync(List<DepartmentDto> departments, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Starting bulk sync for {Count} departments", departments.Count);

            foreach (var department in departments)
            {
                await SyncDepartmentAsync(department, ct);
            }

            _logger.LogInformation("Completed bulk sync for {Count} departments", departments.Count);
            await _cache.RemoveAsync("departments_all", ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in bulk sync for departments");
            throw;
        }
    }

    public async Task SoftDeleteDepartmentAsync(Guid id, CancellationToken ct = default)
    {
        var department = await _context.LocalDepartments.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (department != null)
        {
            department.IsDeleted = true;
            department.SyncedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
            await _cache.RemoveAsync($"department_{id}", ct);
            await _cache.RemoveAsync("departments_all", ct);
            if (department.BranchId.HasValue)
            {
                await _cache.RemoveAsync($"departments_by_branch_{department.BranchId.Value}", ct);
            }
            _logger.LogInformation("Soft deleted department: {DepartmentId}", id);
        }
    }

    public async Task SyncPositionAsync(LocalPosition position, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(position.Name) || string.IsNullOrEmpty(position.NameAm))
        {
            _logger.LogWarning("Skipping position {PositionId} - Name or NameAm is null or empty", position.Id);
            return;
        }

        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(ct);

        const string sql = @"
            INSERT INTO ""LocalPositions"" (""Id"", ""Name"", ""NameAm"", ""NoOfPosition"", ""IsVacant"",
                ""DepartmentId"", ""JobGradeId"",
                ""DateAdd"", ""DateMod"", ""SyncedAt"", ""IsDeleted"")
            VALUES (@Id, @Name, @NameAm, @NoOfPosition, @IsVacant, @DepartmentId, @JobGradeId,
                @DateAdd, @DateMod, @SyncedAt, false)
            ON CONFLICT (""Id"") DO UPDATE SET
                ""Name"" = EXCLUDED.""Name"",
                ""NameAm"" = EXCLUDED.""NameAm"",
                ""NoOfPosition"" = EXCLUDED.""NoOfPosition"",
                ""IsVacant"" = EXCLUDED.""IsVacant"",
                ""DepartmentId"" = EXCLUDED.""DepartmentId"",
                ""JobGradeId"" = EXCLUDED.""JobGradeId"",
                ""DateMod"" = EXCLUDED.""DateMod"",
                ""SyncedAt"" = EXCLUDED.""SyncedAt"",
                ""IsDeleted"" = false";

        await connection.ExecuteAsync(sql, new
        {
            position.Id,
            position.Name,
            position.NameAm,
            position.NoOfPosition,
            IsVacant = position.IsVacant ?? "Unknown",
            position.DepartmentId,
            position.JobGradeId,
            DateAdd = DateTime.UtcNow,
            DateMod = DateTime.UtcNow,
            SyncedAt = DateTime.UtcNow
        });

        _logger.LogInformation("Synced Position: {PositionId} from Core HRMM", position.Id);

        await _cache.RemoveAsync($"position_{position.Id}", ct);
        await _cache.RemoveAsync("positions_all", ct);
        await _cache.RemoveAsync($"positions_by_department_{position.DepartmentId}", ct);
    }

    public async Task SyncJobGradeAsync(LocalJobGrade jobGrade, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(jobGrade.Name))
        {
            _logger.LogWarning("Skipping job grade {JobGradeId} - Name is null or empty", jobGrade.Id);
            return;
        }

        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(ct);

        const string sql = @"
            INSERT INTO ""LocalJobGrades"" (""Id"", ""Name"", ""StartSalary"", ""MaxSalary"",
                ""DateAdd"", ""DateMod"", ""SyncedAt"", ""IsDeleted"")
            VALUES (@Id, @Name, @StartSalary, @MaxSalary,
                @DateAdd, @DateMod, @SyncedAt, false)
            ON CONFLICT (""Id"") DO UPDATE SET
                ""Name"" = EXCLUDED.""Name"",
                ""StartSalary"" = EXCLUDED.""StartSalary"",
                ""MaxSalary"" = EXCLUDED.""MaxSalary"",
                ""DateMod"" = EXCLUDED.""DateMod"",
                ""SyncedAt"" = EXCLUDED.""SyncedAt"",
                ""IsDeleted"" = false";

        await connection.ExecuteAsync(sql, new
        {
            jobGrade.Id,
            jobGrade.Name,
            jobGrade.StartSalary,
            jobGrade.MaxSalary,
            DateAdd = DateTime.UtcNow,
            DateMod = DateTime.UtcNow,
            SyncedAt = DateTime.UtcNow
        });

        _logger.LogInformation("Synced JobGrade: {JobGradeId} from Core HRMM", jobGrade.Id);

        await _cache.RemoveAsync($"jobgrade_{jobGrade.Id}", ct);
        await _cache.RemoveAsync("jobgrades_all", ct);
    }

    public async Task SyncEmployeeAsync(LocalEmployee employee, CancellationToken ct = default)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(ct);

        // Check if Position exists
        const string checkPosSql = @"
            SELECT COUNT(*) FROM ""LocalPositions""
            WHERE ""Id"" = @PositionId AND ""IsDeleted"" = false";

        var posExists = await connection.ExecuteScalarAsync<long>(checkPosSql, new { PositionId = employee.PositionId });

        if (posExists == 0)
        {
            _logger.LogWarning("Position {PositionId} does not exist, skipping Employee {EmployeeId}",
                employee.PositionId, employee.Id);
            return;
        }

        // Check if Department exists
        const string checkDeptSql = @"
            SELECT COUNT(*) FROM ""LocalDepartments""
            WHERE ""Id"" = @DepartmentId AND ""IsDeleted"" = false";

        var deptExists = await connection.ExecuteScalarAsync<long>(checkDeptSql, new { DepartmentId = employee.DepartmentId });

        if (deptExists == 0)
        {
            _logger.LogWarning("Department {DepartmentId} does not exist, skipping Employee {EmployeeId}",
                employee.DepartmentId, employee.Id);
            return;
        }

        const string sql = @"
            INSERT INTO ""LocalEmployees"" (""Id"", ""Code"", ""EmploymentType"", ""EmploymentNature"",
                ""WorkArrangement"", ""EmpState"", ""EmploymentDate"", ""PersonId"", ""JobGradeId"",
                ""PositionId"", ""DepartmentId"", ""AppUserId"", ""FirstName"", ""FirstNameAm"",
                ""MiddleName"", ""MiddleNameAm"", ""LastName"", ""LastNameAm"", ""Gender"",
                ""Nationality"", ""Email"", ""Phone"", ""IsActive"", ""DateAdd"", ""DateMod"", ""SyncedAt"", ""IsDeleted"")
            VALUES (@Id, @Code, @EmploymentType, @EmploymentNature, @WorkArrangement, @EmpState,
                @EmploymentDate, @PersonId, @JobGradeId, @PositionId, @DepartmentId, @AppUserId,
                @FirstName, @FirstNameAm, @MiddleName, @MiddleNameAm, @LastName, @LastNameAm,
                @Gender, @Nationality, @Email, @Phone, @IsActive, @DateAdd, @DateMod, @SyncedAt, false)
            ON CONFLICT (""Id"") DO UPDATE SET
                ""Code"" = EXCLUDED.""Code"",
                ""EmploymentType"" = EXCLUDED.""EmploymentType"",
                ""EmploymentNature"" = EXCLUDED.""EmploymentNature"",
                ""WorkArrangement"" = EXCLUDED.""WorkArrangement"",
                ""EmpState"" = EXCLUDED.""EmpState"",
                ""EmploymentDate"" = EXCLUDED.""EmploymentDate"",
                ""PersonId"" = EXCLUDED.""PersonId"",
                ""JobGradeId"" = EXCLUDED.""JobGradeId"",
                ""PositionId"" = EXCLUDED.""PositionId"",
                ""DepartmentId"" = EXCLUDED.""DepartmentId"",
                ""AppUserId"" = EXCLUDED.""AppUserId"",
                ""FirstName"" = EXCLUDED.""FirstName"",
                ""FirstNameAm"" = EXCLUDED.""FirstNameAm"",
                ""MiddleName"" = EXCLUDED.""MiddleName"",
                ""MiddleNameAm"" = EXCLUDED.""MiddleNameAm"",
                ""LastName"" = EXCLUDED.""LastName"",
                ""LastNameAm"" = EXCLUDED.""LastNameAm"",
                ""Gender"" = EXCLUDED.""Gender"",
                ""Nationality"" = EXCLUDED.""Nationality"",
                ""Email"" = EXCLUDED.""Email"",
                ""Phone"" = EXCLUDED.""Phone"",
                ""IsActive"" = EXCLUDED.""IsActive"",
                ""DateMod"" = EXCLUDED.""DateMod"",
                ""SyncedAt"" = EXCLUDED.""SyncedAt"",
                ""IsDeleted"" = false";

        await connection.ExecuteAsync(sql, new
        {
            employee.Id,
            employee.Code,
            employee.EmploymentType,
            employee.EmploymentNature,
            employee.WorkArrangement,
            employee.EmpState,
            EmploymentDate = employee.EmploymentDate != DateTime.MinValue ? employee.EmploymentDate : DateTime.UtcNow,
            PersonId = employee.PersonId != Guid.Empty ? employee.PersonId : Guid.NewGuid(),
            employee.JobGradeId,
            employee.PositionId,
            employee.DepartmentId,
            employee.AppUserId,
            employee.FirstName,
            employee.FirstNameAm,
            employee.MiddleName,
            employee.MiddleNameAm,
            employee.LastName,
            employee.LastNameAm,
            employee.Gender,
            employee.Nationality,
            employee.Email,
            employee.Phone,
            // ✅ FIX: Ensure IsActive is never null
            IsActive = employee.IsActive ?? true, // Default to true if null
            DateAdd = DateTime.UtcNow,
            DateMod = DateTime.UtcNow,
            SyncedAt = DateTime.UtcNow
        });

        _logger.LogInformation("Synced Employee: {EmployeeId} - {FirstName} {LastName}",
            employee.Id, employee.FirstName, employee.LastName);

        await _cache.RemoveAsync($"employee_{employee.Id}", ct);
        await _cache.RemoveAsync("employees_all", ct);
        await _cache.RemoveAsync($"employees_by_department_{employee.DepartmentId}", ct);
        await _cache.RemoveAsync($"employees_by_position_{employee.PositionId}", ct);
        if (employee.AppUserId.HasValue)
        {
            await _cache.RemoveAsync($"employee_by_user_{employee.AppUserId}", ct);
        }
    }

    public async Task BulkSyncEmployeesAsync(List<LocalEmployee> employees, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Starting bulk sync for {Count} employees", employees.Count);

            foreach (var employee in employees)
            {
                await SyncEmployeeAsync(employee, ct);
            }

            _logger.LogInformation("Completed bulk sync for {Count} employees", employees.Count);
            await _cache.RemoveAsync("employees_all", ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in bulk sync for employees");
            throw;
        }
    }

    public async Task SoftDeleteEmployeeAsync(Guid id, CancellationToken ct = default)
    {
        var employee = await _context.LocalEmployees.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (employee != null)
        {
            employee.IsDeleted = true;
            employee.SyncedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);

            await _cache.RemoveAsync($"employee_{id}", ct);
            await _cache.RemoveAsync("employees_all", ct);
            await _cache.RemoveAsync($"employees_by_department_{employee.DepartmentId}", ct);
            await _cache.RemoveAsync($"employees_by_position_{employee.PositionId}", ct);
            if (employee.AppUserId.HasValue)
            {
                await _cache.RemoveAsync($"employee_by_user_{employee.AppUserId}", ct);
            }

            _logger.LogInformation("Soft deleted employee: {EmployeeId}", id);
        }
    }






    // ============================================================
    // FINANCIAL PERIOD SYNC METHODS
    // ============================================================

    // ============================================================
    // FINANCIAL PERIOD SYNC METHODS
    // ============================================================

    public async Task SyncFinancialPeriodAsync(FinancialPeriodDto period, CancellationToken ct = default)
    {
        try
        {
            var existing = await _context.FinancialPeriods
                .FirstOrDefaultAsync(x => x.Id == period.Id, ct);

            // ✅ Convert string to enum for PeriodType
            var periodType = Enum.TryParse<PeriodType>(period.PeriodType?.ToString(), true, out var parsedPeriodType)
                ? parsedPeriodType
                : PeriodType.MONTHLY;

            // ✅ Convert string to enum for PeriodStatus
            var periodStatus = Enum.TryParse<PeriodStatus>(period.Status?.ToString(), true, out var parsedPeriodStatus)
                ? parsedPeriodStatus
                : PeriodStatus.OPEN;

            if (existing == null)
            {
                await _context.FinancialPeriods.AddAsync(new FinancialPeriod
                {
                    Id = period.Id,
                    Name = period.Name,
                    Code = period.Code,
                    StartDate = period.StartDate,
                    EndDate = period.EndDate,
                    PeriodType = periodType,  // ✅ Now using enum
                    Status = periodStatus,     // ✅ Now using enum
                    IsClosed = period.IsClosed,
                    ClosedDate = period.ClosedDate,
                    ClosedBy = period.ClosedBy,
                    Notes = period.Notes,
                    FiscalYear = period.FiscalYear,
                    IsActive = period.IsActive,
                    SyncedAt = DateTime.UtcNow,
                    SourceId = period.SourceId,
                    DateAdd = DateTime.UtcNow
                }, ct);
                _logger.LogInformation("Synced new financial period: {PeriodName}", period.Name);
            }
            else
            {
                existing.Name = period.Name;
                existing.Code = period.Code;
                existing.StartDate = period.StartDate;
                existing.EndDate = period.EndDate;
                existing.PeriodType = periodType;  // ✅ Now using enum
                existing.Status = periodStatus;     // ✅ Now using enum
                existing.IsClosed = period.IsClosed;
                existing.ClosedDate = period.ClosedDate;
                existing.ClosedBy = period.ClosedBy;
                existing.Notes = period.Notes;
                existing.FiscalYear = period.FiscalYear;
                existing.IsActive = period.IsActive;
                existing.SyncedAt = DateTime.UtcNow;
                existing.SourceId = period.SourceId;
                existing.DateMod = DateTime.UtcNow;
                _logger.LogInformation("Updated financial period: {PeriodName}", period.Name);
            }

            await _context.SaveChangesAsync(ct);
            await _cache.RemoveAsync($"financialperiod_{period.Id}", ct);
            await _cache.RemoveAsync("financialperiods_all", ct);
            await _cache.RemoveAsync("financialperiods_active", ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing financial period: {PeriodName}", period.Name);
            throw;
        }
    }

    public async Task BulkSyncFinancialPeriodsAsync(List<FinancialPeriodDto> periods, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Starting bulk sync for {Count} financial periods", periods.Count);

            foreach (var period in periods)
            {
                await SyncFinancialPeriodAsync(period, ct);
            }

            _logger.LogInformation("Completed bulk sync for {Count} financial periods", periods.Count);
            await _cache.RemoveAsync("financialperiods_all", ct);
            await _cache.RemoveAsync("financialperiods_active", ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in bulk sync for financial periods");
            throw;
        }
    }

    public async Task SoftDeleteFinancialPeriodAsync(Guid id, CancellationToken ct = default)
    {
        var period = await _context.FinancialPeriods.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (period != null)
        {
            period.IsDeleted = true;
            period.SyncedAt = DateTime.UtcNow;
            period.DateMod = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);

            await _cache.RemoveAsync($"financialperiod_{id}", ct);
            await _cache.RemoveAsync("financialperiods_all", ct);
            await _cache.RemoveAsync("financialperiods_active", ct);
            _logger.LogInformation("Soft deleted financial period: {PeriodId}", id);
        }
    }

    // ============================================================
    // VENDOR SYNC METHODS
    // ============================================================

    // In SyncService.cs

    public async Task SyncVendorAsync(VendorDto vendor, CancellationToken ct = default)
    {
        try
        {
            var existing = await _context.Vendors
                .FirstOrDefaultAsync(x => x.Id == vendor.Id, ct);

            if (existing == null)
            {
                await _context.Vendors.AddAsync(new Vendor
                {
                    Id = vendor.Id,
                    Code = vendor.Code,
                    Name = vendor.Name,
                    NameAm = vendor.NameAm,
                    Description = vendor.Description,
                    Email = vendor.Email,
                    Phone = vendor.Phone,
                    Mobile = vendor.Mobile,
                    Address = vendor.Address,
                    City = vendor.City,
                    Country = vendor.Country,
                    TaxId = vendor.TaxId,
                    RegistrationNumber = vendor.RegistrationNumber,
                    VendorType = vendor.VendorType,
                    Status = vendor.Status,
                    PaymentTerms = vendor.PaymentTerms,
                    Currency = vendor.Currency,
                    BankName = vendor.BankName,
                    BankAccount = vendor.BankAccount,
                    Website = vendor.Website,
                    // ✅ Convert ContactPersonDto to JSON string
                    ContactPerson = SerializeContactPerson(vendor.ContactPerson),
                    Rating = vendor.Rating,
                    TotalSpent = vendor.TotalSpent,
                    TotalTransactions = vendor.TotalTransactions,
                    IsActive = vendor.IsActive,
                    SyncedAt = DateTime.UtcNow,
                    SourceId = vendor.SourceId,
                    DateAdd = DateTime.UtcNow
                }, ct);
                _logger.LogInformation("Synced new vendor: {VendorName} ({VendorCode})", vendor.Name, vendor.Code);
            }
            else
            {
                existing.Code = vendor.Code;
                existing.Name = vendor.Name;
                existing.NameAm = vendor.NameAm;
                existing.Description = vendor.Description;
                existing.Email = vendor.Email;
                existing.Phone = vendor.Phone;
                existing.Mobile = vendor.Mobile;
                existing.Address = vendor.Address;
                existing.City = vendor.City;
                existing.Country = vendor.Country;
                existing.TaxId = vendor.TaxId;
                existing.RegistrationNumber = vendor.RegistrationNumber;
                existing.VendorType = vendor.VendorType;
                existing.Status = vendor.Status;
                existing.PaymentTerms = vendor.PaymentTerms;
                existing.Currency = vendor.Currency;
                existing.BankName = vendor.BankName;
                existing.BankAccount = vendor.BankAccount;
                existing.Website = vendor.Website;
                // ✅ Convert ContactPersonDto to JSON string
                existing.ContactPerson = SerializeContactPerson(vendor.ContactPerson);
                existing.Rating = vendor.Rating;
                existing.TotalSpent = vendor.TotalSpent;
                existing.TotalTransactions = vendor.TotalTransactions;
                existing.IsActive = vendor.IsActive;
                existing.SyncedAt = DateTime.UtcNow;
                existing.SourceId = vendor.SourceId;
                existing.DateMod = DateTime.UtcNow;
                _logger.LogInformation("Updated vendor: {VendorName} ({VendorCode})", vendor.Name, vendor.Code);
            }

            await _context.SaveChangesAsync(ct);

            // Invalidate cache
            await _cache.RemoveAsync($"vendor_{vendor.Id}", ct);
            await _cache.RemoveAsync($"vendor_code_{vendor.Code}", ct);
            await _cache.RemoveAsync("vendors_all", ct);
            await _cache.RemoveAsync("vendors_active", ct);
            await _cache.RemoveAsync($"vendors_by_type_{vendor.VendorType}", ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing vendor: {VendorName} ({VendorCode})", vendor.Name, vendor.Code);
            throw;
        }
    }

    // ✅ Helper methods for ContactPerson serialization
    private static string SerializeContactPerson(ContactPersonDto? contact)
    {
        if (contact == null)
            return "{}";

        try
        {
            return JsonSerializer.Serialize(contact, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
        catch
        {
            return "{}";
        }
    }

    private static ContactPersonDto? DeserializeContactPerson(string? json)
    {
        if (string.IsNullOrEmpty(json))
            return null;

        try
        {
            return JsonSerializer.Deserialize<ContactPersonDto>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
        catch
        {
            return null;
        }
    }

    public async Task BulkSyncVendorsAsync(List<VendorDto> vendors, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Starting bulk sync for {Count} vendors", vendors.Count);

            foreach (var vendor in vendors)
            {
                await SyncVendorAsync(vendor, ct);
            }

            _logger.LogInformation("Completed bulk sync for {Count} vendors", vendors.Count);
            await _cache.RemoveAsync("vendors_all", ct);
            await _cache.RemoveAsync("vendors_active", ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in bulk sync for vendors");
            throw;
        }
    }

    public async Task SoftDeleteVendorAsync(Guid id, CancellationToken ct = default)
    {
        var vendor = await _context.Vendors.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (vendor != null)
        {
            vendor.IsDeleted = true;
            vendor.SyncedAt = DateTime.UtcNow;
            vendor.DateMod = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);

            await _cache.RemoveAsync($"vendor_{id}", ct);
            await _cache.RemoveAsync($"vendor_code_{vendor.Code}", ct);
            await _cache.RemoveAsync("vendors_all", ct);
            await _cache.RemoveAsync("vendors_active", ct);
            await _cache.RemoveAsync($"vendors_by_type_{vendor.VendorType}", ct);
            _logger.LogInformation("Soft deleted vendor: {VendorId} ({VendorCode})", id, vendor.Code);
        }
    }
}