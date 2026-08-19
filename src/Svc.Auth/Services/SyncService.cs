using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using Svc.Auth.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Shared.Helpers.Services;
using Svc.Auth.HealthChecks;

namespace Svc.Auth.Services;



public class SyncService : ISyncService
{
    private readonly string _connectionString;
    private readonly ILogger<SyncService> _logger;
    private readonly ICacheService _cache;
    private readonly IAlertService _alert;

    public SyncService(IConfiguration configuration, ILogger<SyncService> logger, ICacheService cache, IAlertService alert)
    {
        _connectionString = configuration.GetConnectionString("authMgrCon")
            ?? throw new InvalidOperationException("authMgrCon connection string not found");
        _logger = logger;
        _cache = cache;
        _alert = alert;
    }

    // ============================================================
    // SYNC COMPANIES
    // ============================================================

    public async Task SyncCompanyAsync(CompanyDto company, CancellationToken ct = default)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(ct);

            const string sql = @"
                INSERT INTO ""Companies"" (""Id"", ""Name"", ""NameAm"", ""TaxId"", ""Phone"",
                    ""Email"", ""Address"", ""LogoUrl"", ""DateAdd"", ""DateMod"", ""SyncedAt"", ""IsDeleted"")
                VALUES (@Id, @Name, @NameAm, @TaxId, @Phone, @Email, @Address, @LogoUrl, @DateAdd, @DateMod, @SyncedAt, false)
                ON CONFLICT (""Id"") DO UPDATE SET
                    ""Name"" = EXCLUDED.""Name"",
                    ""NameAm"" = EXCLUDED.""NameAm"",
                    ""TaxId"" = EXCLUDED.""TaxId"",
                    ""Phone"" = EXCLUDED.""Phone"",
                    ""Email"" = EXCLUDED.""Email"",
                    ""Address"" = EXCLUDED.""Address"",
                    ""LogoUrl"" = EXCLUDED.""LogoUrl"",
                    ""DateMod"" = EXCLUDED.""DateMod"",
                    ""SyncedAt"" = EXCLUDED.""SyncedAt"",
                    ""IsDeleted"" = false";

            await connection.ExecuteAsync(sql, new
            {
                company.Id,
                company.Name,
                company.NameAm,
                company.TaxId,
                company.Phone,
                company.Email,
                company.Address,
                company.LogoUrl,
                DateAdd = DateTime.UtcNow,
                DateMod = DateTime.UtcNow,
                SyncedAt = DateTime.UtcNow
            });

            // ✅ Invalidate cache AFTER successful sync
            await _cache.RemoveAsync($"company_{company.Id}", ct);
            await _cache.RemoveAsync("companies_all", ct);

            _logger.LogInformation("✅ Synced Company: {CompanyId} - {CompanyName}", company.Id, company.Name);
            await _alert.SendSuccessAsync("Company Synced", $"Company {company.Name} synced successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to sync company: {CompanyId}", company.Id);
            SyncHealthCheck.RecordSyncFailure(ex);
            await _alert.SendErrorAsync("Sync Failed", $"Failed to sync company {company.Name}", ex);
            throw;
        }
    }

    // ============================================================
    // SYNC BRANCHES
    // ============================================================

    public async Task SyncBranchAsync(BranchDto branch, CancellationToken ct = default)
    {
        try
        {
            if (branch.CompId == Guid.Empty)
            {
                _logger.LogWarning("⚠️ Skipping Branch {BranchId} - CompId is empty", branch.Id);
                return;
            }

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(ct);

            const string sql = @"
                INSERT INTO ""Branches"" (""Id"", ""Name"", ""NameAm"", ""Code"", ""Location"",
                    ""OpenDate"", ""BranchType"", ""BranchStat"", ""CompId"",
                    ""DateAdd"", ""DateMod"", ""SyncedAt"", ""IsDeleted"")
                VALUES (@Id, @Name, @NameAm, @Code, @Location, @OpenDate, @BranchType, @BranchStat, @CompId,
                    @DateAdd, @DateMod, @SyncedAt, false)
                ON CONFLICT (""Id"") DO UPDATE SET
                    ""Name"" = EXCLUDED.""Name"",
                    ""NameAm"" = EXCLUDED.""NameAm"",
                    ""Code"" = EXCLUDED.""Code"",
                    ""Location"" = EXCLUDED.""Location"",
                    ""OpenDate"" = EXCLUDED.""OpenDate"",
                    ""BranchType"" = EXCLUDED.""BranchType"",
                    ""BranchStat"" = EXCLUDED.""BranchStat"",
                    ""CompId"" = EXCLUDED.""CompId"",
                    ""DateMod"" = EXCLUDED.""DateMod"",
                    ""SyncedAt"" = EXCLUDED.""SyncedAt"",
                    ""IsDeleted"" = false";

            await connection.ExecuteAsync(sql, new
            {
                branch.Id,
                branch.Name,
                branch.NameAm,
                branch.Code,
                branch.Location,
                branch.OpenDate,
                branch.BranchType,
                branch.BranchStat,
                branch.CompId,
                DateAdd = DateTime.UtcNow,
                DateMod = DateTime.UtcNow,
                SyncedAt = DateTime.UtcNow
            });

            // ✅ Invalidate cache AFTER successful sync
            await _cache.RemoveAsync($"branch_{branch.Id}", ct);
            await _cache.RemoveAsync("branches_all", ct);
            await _cache.RemoveAsync($"branches_by_company_{branch.CompId}", ct);

            _logger.LogInformation("✅ Synced Branch: {BranchId} - {BranchName}", branch.Id, branch.Name);
            SyncHealthCheck.RecordSyncSuccess();
            await _alert.SendSuccessAsync("Branch Synced", $"Branch {branch.Name} synced successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to sync branch: {BranchId}", branch.Id);
            SyncHealthCheck.RecordSyncFailure(ex);
            await _alert.SendErrorAsync("Sync Failed", $"Failed to sync branch {branch.Name}", ex);
            throw;
        }
    }

    // ============================================================
    // SYNC DEPARTMENTS
    // ============================================================

 public async Task SyncDepartmentAsync(DepartmentDto dept, CancellationToken ct = default)
 {
     try
     {
         using var connection = new NpgsqlConnection(_connectionString);
         await connection.OpenAsync(ct);

         // ✅ Branch መኖሩን ያረጋግጡ
         var branchExists = await connection.ExecuteScalarAsync<int>(
             "SELECT COUNT(1) FROM \"Branches\" WHERE \"Id\" = @BranchId AND \"IsDeleted\" = false",
             new { BranchId = dept.BranchId });

         if (branchExists == 0)
         {
             // ❌ ስህተት አይጣሉ - በቀላሉ ይመለሱ
             _logger.LogWarning("⚠️ Branch {BranchId} not found for Department {DepartmentId}. Skipping sync.",
                 dept.BranchId, dept.Id);
             return; // ✅ እዚህ በቀላሉ ይመለሱ
         }

         // ✅ ዲፓርትመንቱን ሲንክ ያድርጉ
         const string sql = @"
             INSERT INTO ""Departments"" (""Id"", ""Name"", ""NameAm"", ""DeptStat"", ""BranchId"",
                 ""DateAdd"", ""DateMod"", ""SyncedAt"", ""IsDeleted"")
             VALUES (@Id, @Name, @NameAm, @DeptStat, @BranchId,
                 @DateAdd, @DateMod, @SyncedAt, false)
             ON CONFLICT (""Id"") DO UPDATE SET
                 ""Name"" = EXCLUDED.""Name"",
                 ""NameAm"" = EXCLUDED.""NameAm"",
                 ""DeptStat"" = EXCLUDED.""DeptStat"",
                 ""BranchId"" = EXCLUDED.""BranchId"",
                 ""DateMod"" = EXCLUDED.""DateMod"",
                 ""SyncedAt"" = EXCLUDED.""SyncedAt"",
                 ""IsDeleted"" = false";

         await connection.ExecuteAsync(sql, new
         {
             dept.Id,
             dept.Name,
             dept.NameAm,
             dept.DeptStat,
             dept.BranchId,
             DateAdd = DateTime.UtcNow,
             DateMod = DateTime.UtcNow,
             SyncedAt = DateTime.UtcNow
         });

         _logger.LogInformation("✅ Synced Department: {DepartmentId} - {DepartmentName}", dept.Id, dept.Name);
     }
     catch (Npgsql.PostgresException ex) when (ex.SqlState == "23503")
     {
         // Foreign key violation - Branch doesn't exist
         // ✅ ስህተት አይጣሉ - በቀላሉ ይመዝግቡ
         _logger.LogWarning(ex, "⚠️ Foreign key violation for department {DepartmentId}. Branch {BranchId} not found. Skipping.",
             dept.Id, dept.BranchId);
         // ✅ እዚህ ምንም አይጣሉ
     }
     catch (Exception ex)
     {
         _logger.LogError(ex, "❌ Failed to sync department: {DepartmentId}", dept.Id);
         throw; // ✅ ሌሎች ስህተቶች ብቻ ይጣሉ
     }
 }

    // ============================================================
    // SYNC POSITIONS
    // ============================================================

    public async Task SyncPositionAsync(PositionDto position, CancellationToken ct = default)
    {
        try
        {
            if (position.DepartmentId == Guid.Empty)
            {
                _logger.LogWarning("⚠️ Skipping Position {PositionId} - DepartmentId is empty", position.Id);
                return;
            }

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(ct);

            // ✅ Check if Department exists
            const string checkDeptSql = @"
                SELECT COUNT(*) FROM ""Departments""
                WHERE ""Id"" = @DepartmentId AND ""IsDeleted"" = false";

            var deptExists = await connection.ExecuteScalarAsync<long>(checkDeptSql, new { position.DepartmentId });

            if (deptExists == 0)
            {
                _logger.LogWarning("⚠️ Skipping Position {PositionId} - Department {DepartmentId} does not exist",
                    position.Id, position.DepartmentId);
                return;
            }

            const string sql = @"
                INSERT INTO ""Positions"" (""Id"", ""Name"", ""NameAm"", ""NoOfPosition"", ""IsVacant"",
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
                position.IsVacant,
                position.DepartmentId,
                position.JobGradeId,
                DateAdd = DateTime.UtcNow,
                DateMod = DateTime.UtcNow,
                SyncedAt = DateTime.UtcNow
            });

            // ✅ Invalidate cache
            await _cache.RemoveAsync($"position_{position.Id}", ct);
            await _cache.RemoveAsync("positions_all", ct);
            await _cache.RemoveAsync($"positions_by_department_{position.DepartmentId}", ct);

            _logger.LogInformation("✅ Synced Position: {PositionId} - {PositionName}", position.Id, position.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to sync position: {PositionId}", position.Id);
            throw;
        }
    }

    // ============================================================
    // SYNC JOB GRADES
    // ============================================================

    public async Task SyncJobGradeAsync(JobGradeDto jobGrade, CancellationToken ct = default)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(ct);

            const string sql = @"
                INSERT INTO ""JobGrades"" (""Id"", ""Name"", ""StartSalary"", ""MaxSalary"",
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

            await _cache.RemoveAsync($"jobgrade_{jobGrade.Id}", ct);
            await _cache.RemoveAsync("jobgrades_all", ct);

            _logger.LogInformation("✅ Synced JobGrade: {JobGradeId} - {JobGradeName}", jobGrade.Id, jobGrade.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to sync job grade: {JobGradeId}", jobGrade.Id);
            throw;
        }
    }

    // ============================================================
    // SYNC EMPLOYEES
    // ============================================================

    public async Task SyncEmployeeAsync(EmployeeDto employee, CancellationToken ct = default)
    {
        try
        {
            if (employee.PositionId == Guid.Empty)
            {
                _logger.LogWarning("⚠️ Skipping Employee {EmployeeId} - PositionId is empty", employee.Id);
                return;
            }

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(ct);

            // ✅ Verify Position exists
            const string checkPosSql = @"
                SELECT COUNT(*) FROM ""Positions""
                WHERE ""Id"" = @PositionId AND ""IsDeleted"" = false";

            var posExists = await connection.ExecuteScalarAsync<long>(checkPosSql, new { employee.PositionId });

            if (posExists == 0)
            {
                _logger.LogWarning("⚠️ Position {PositionId} does not exist, skipping Employee {EmployeeId}",
                    employee.PositionId, employee.Id);
                return;
            }

            const string sql = @"
                INSERT INTO ""Employees"" (""Id"", ""Code"", ""EmploymentType"", ""EmploymentNature"",
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
                employee.EmploymentDate,
                employee.PersonId,
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

            // ✅ Invalidate cache AFTER successful sync
            await _cache.RemoveAsync($"employee_{employee.Id}", ct);
            await _cache.RemoveAsync("employees_all", ct);
            await _cache.RemoveAsync($"employees_by_department_{employee.DepartmentId}", ct);
            await _cache.RemoveAsync($"employees_by_position_{employee.PositionId}", ct);

            _logger.LogInformation("✅ Synced Employee: {EmployeeId} - {FirstName} {LastName}",
                employee.Id, employee.FirstName, employee.LastName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to sync employee: {EmployeeId}", employee.Id);
            throw;
        }
    }

    // ============================================================
    // SOFT DELETE
    // ============================================================

    public async Task SoftDeleteBranchAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(ct);
            const string sql = @"UPDATE ""Branches"" SET ""IsDeleted"" = true, ""SyncedAt"" = @SyncedAt WHERE ""Id"" = @Id";
            await connection.ExecuteAsync(sql, new { Id = id, SyncedAt = DateTime.UtcNow });

            await _cache.RemoveAsync($"branch_{id}", ct);
            await _cache.RemoveAsync("branches_all", ct);

            _logger.LogInformation("✅ Soft deleted branch: {BranchId}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to soft delete branch: {BranchId}", id);
            throw;
        }
    }

    public async Task SoftDeleteDepartmentAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(ct);
            const string sql = @"UPDATE ""Departments"" SET ""IsDeleted"" = true, ""SyncedAt"" = @SyncedAt WHERE ""Id"" = @Id";
            await connection.ExecuteAsync(sql, new { Id = id, SyncedAt = DateTime.UtcNow });

            await _cache.RemoveAsync($"department_{id}", ct);
            await _cache.RemoveAsync("departments_all", ct);

            _logger.LogInformation("✅ Soft deleted department: {DepartmentId}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to soft delete department: {DepartmentId}", id);
            throw;
        }
    }

    // ============================================================
    // RETRY LOGIC
    // ============================================================

    public async Task SyncWithRetryAsync(Func<Task> syncAction, int maxRetries = 3)
    {
        int retryCount = 0;
        Exception? lastException = null;

        while (retryCount < maxRetries)
        {
            try
            {
                await syncAction();
                if (retryCount > 0)
                {
                    _logger.LogInformation("✅ Sync completed successfully after {RetryCount} attempts.", retryCount + 1);
                }
                return;
            }
            catch (Exception ex)
            {
                lastException = ex;
                retryCount++;
                _logger.LogWarning(ex, "⚠️ Sync attempt {RetryCount} failed. Retrying in {Delay}ms...",
                    retryCount, Math.Pow(2, retryCount) * 1000);

                if (retryCount < maxRetries)
                {
                    var delay = TimeSpan.FromSeconds(Math.Pow(2, retryCount));
                    await Task.Delay(delay);
                }
            }
        }

        _logger.LogError(lastException, "❌ Sync failed after {MaxRetries} attempts.", maxRetries);
        throw lastException ?? new Exception("Sync failed with unknown error.");
    }
}