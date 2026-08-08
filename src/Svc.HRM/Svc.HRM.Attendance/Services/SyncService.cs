
using Svc.HRM.Attendance.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Threading;
using System.Threading.Tasks;
using Shared.Helpers.Services;
using Svc.HRM.Attendance.Models.DTOs;
using Svc.HRM.Attendance.Models.Entities.Local;
namespace Svc.HRM.Attendance.Services;

public class SyncService : ISyncService
{
    private readonly AttendanceDbContext _context;
    private readonly ILogger<SyncService> _logger;
    private readonly string _connectionString;
    private readonly ICacheService _cache;

    public SyncService(IConfiguration configuration, AttendanceDbContext context, ILogger<SyncService> logger, ICacheService cache)
    {
        _connectionString = configuration.GetConnectionString("HrmPayrollDb")
            ?? throw new InvalidOperationException("HrmPayrollDb connection string not found");
        _logger = logger;
        _cache = cache;
        _context = context;
    }

    // ==================== COMPANY SYNC ====================

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

            // Clear cache
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
        try
        {
            _logger.LogInformation("Starting bulk sync for {Count} companies", companies.Count);

            foreach (var company in companies)
            {
                await SyncCompanyAsync(company, ct);
            }

            _logger.LogInformation("Completed bulk sync for {Count} companies", companies.Count);

            // Clear all companies cache after bulk sync
            await _cache.RemoveAsync("companies_all", ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in bulk sync for companies");
            throw;
        }
    }

    public async Task SoftDeleteCompanyAsync(Guid id, CancellationToken ct = default)
    {
        var company = await _context.LocalCompanies.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (company != null)
        {
            company.IsDeleted = true;
            company.SyncedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);

            // Clear cache
            await _cache.RemoveAsync($"company_{id}", ct);
            await _cache.RemoveAsync("companies_all", ct);

            _logger.LogInformation("Soft deleted company: {CompanyId}", id);
        }
    }

    // ==================== BRANCH SYNC ====================

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

            // Clear cache
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

            // Clear all branches cache after bulk sync
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

            // Clear cache
            await _cache.RemoveAsync($"branch_{id}", ct);
            await _cache.RemoveAsync("branches_all", ct);
            if (branch.CompId.HasValue)
            {
                await _cache.RemoveAsync($"branches_by_company_{branch.CompId.Value}", ct);
            }

            _logger.LogInformation("Soft deleted branch: {BranchId}", id);
        }
    }

    // ==================== DEPARTMENT SYNC ====================

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

            // Clear cache
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

            // Clear all departments cache after bulk sync
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

            // Clear cache
            await _cache.RemoveAsync($"department_{id}", ct);
            await _cache.RemoveAsync("departments_all", ct);
            if (department.BranchId.HasValue)
            {
                await _cache.RemoveAsync($"departments_by_branch_{department.BranchId.Value}", ct);
            }

            _logger.LogInformation("Soft deleted department: {DepartmentId}", id);
        }
    }

    // ==================== POSITION SYNC ====================

    public async Task SyncPositionAsync(LocalPosition position, CancellationToken ct = default)
    {
        // Validate required fields
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

        // Clear cache
        await _cache.RemoveAsync($"position_{position.Id}", ct);
        await _cache.RemoveAsync("positions_all", ct);
        await _cache.RemoveAsync($"positions_by_department_{position.DepartmentId}", ct);
    }

    // ==================== JOBGRADE SYNC ====================

    public async Task SyncJobGradeAsync(LocalJobGrade jobGrade, CancellationToken ct = default)
    {
        // Validate required fields
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

        // Clear cache
        await _cache.RemoveAsync($"jobgrade_{jobGrade.Id}", ct);
        await _cache.RemoveAsync("jobgrades_all", ct);
    }

    // ==================== EMPLOYEE SYNC ====================

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
                ""Nationality"", ""Email"", ""Phone"", ""DateAdd"", ""DateMod"", ""SyncedAt"", ""IsDeleted"")
            VALUES (@Id, @Code, @EmploymentType, @EmploymentNature, @WorkArrangement, @EmpState,
                @EmploymentDate, @PersonId, @JobGradeId, @PositionId, @DepartmentId, @AppUserId,
                @FirstName, @FirstNameAm, @MiddleName, @MiddleNameAm, @LastName, @LastNameAm,
                @Gender, @Nationality, @Email, @Phone, @DateAdd, @DateMod, @SyncedAt, false)
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
            DateAdd = DateTime.UtcNow,
            DateMod = DateTime.UtcNow,
            SyncedAt = DateTime.UtcNow
        });

        _logger.LogInformation("Synced Employee: {EmployeeId} - {FirstName} {LastName}",
            employee.Id, employee.FirstName, employee.LastName);

        // Clear cache
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

            // Clear all employees cache after bulk sync
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

            // Clear cache
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
}