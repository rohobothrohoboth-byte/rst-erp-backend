using Cor.HRMM.Models.Entities.Local;
using Dapper;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Shared.Helpers.Services;
namespace Cor.HRMM.Services;



public class SyncService : ISyncService
{
    private readonly string _connectionString;
    private readonly ILogger<SyncService> _logger;
    private readonly ICacheService _cache;

    public SyncService(IConfiguration configuration, ILogger<SyncService> logger, ICacheService cache)
    {
        _connectionString = configuration.GetConnectionString("coreHRMMDbCon")
            ?? throw new InvalidOperationException("coreHRMMDbCon connection string not found");
        _logger = logger;
        _cache = cache;
    }

    // ============================================================
    // SYNC COMPANIES
    // ============================================================

    public async Task SyncCompanyAsync(LocalCompany company, CancellationToken ct = default)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(ct);

            const string sql = @"
                INSERT INTO ""Companies"" (""Id"", ""Name"", ""NameAm"", ""TaxId"", ""Phone"",
                    ""Email"", ""Address"", ""LogoUrl"", ""SyncedAt"", ""IsDeleted"")
                VALUES (@Id, @Name, @NameAm, @TaxId, @Phone, @Email, @Address, @LogoUrl, @SyncedAt, false)
                ON CONFLICT (""Id"") DO UPDATE SET
                    ""Name"" = EXCLUDED.""Name"",
                    ""NameAm"" = EXCLUDED.""NameAm"",
                    ""TaxId"" = EXCLUDED.""TaxId"",
                    ""Phone"" = EXCLUDED.""Phone"",
                    ""Email"" = EXCLUDED.""Email"",
                    ""Address"" = EXCLUDED.""Address"",
                    ""LogoUrl"" = EXCLUDED.""LogoUrl"",
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
                SyncedAt = DateTime.UtcNow
            });

            // ✅ Invalidate cache
            await _cache.RemoveAsync($"company_{company.Id}", ct);
            await _cache.RemoveAsync("companies_all", ct);

            _logger.LogInformation("✅ Synced Company: {CompanyId} - {CompanyName}", company.Id, company.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to sync company: {CompanyId}", company.Id);
            throw;
        }
    }

    // ============================================================
    // SYNC BRANCHES
    // ============================================================

    public async Task SyncBranchAsync(LocalBranch branch, CancellationToken ct = default)
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
                    ""OpenDate"", ""BranchType"", ""BranchStat"", ""CompId"", ""SyncedAt"", ""IsDeleted"")
                VALUES (@Id, @Name, @NameAm, @Code, @Location, @OpenDate, @BranchType, @BranchStat, @CompId, @SyncedAt, false)
                ON CONFLICT (""Id"") DO UPDATE SET
                    ""Name"" = EXCLUDED.""Name"",
                    ""NameAm"" = EXCLUDED.""NameAm"",
                    ""Code"" = EXCLUDED.""Code"",
                    ""Location"" = EXCLUDED.""Location"",
                    ""OpenDate"" = EXCLUDED.""OpenDate"",
                    ""BranchType"" = EXCLUDED.""BranchType"",
                    ""BranchStat"" = EXCLUDED.""BranchStat"",
                    ""CompId"" = EXCLUDED.""CompId"",
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
                SyncedAt = DateTime.UtcNow
            });

            // ✅ Invalidate cache
            await _cache.RemoveAsync($"branch_{branch.Id}", ct);
            await _cache.RemoveAsync("branches_all", ct);
            await _cache.RemoveAsync($"branches_by_company_{branch.CompId}", ct);

            _logger.LogInformation("✅ Synced Branch: {BranchId} - {BranchName}", branch.Id, branch.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to sync branch: {BranchId}", branch.Id);
            throw;
        }
    }

    // ============================================================
    // SYNC DEPARTMENTS
    // ============================================================

    public async Task SyncDepartmentAsync(LocalDepartment dept, CancellationToken ct = default)
    {
        try
        {
            if (dept.BranchId == Guid.Empty)
            {
                _logger.LogWarning("⚠️ Skipping Department {DepartmentId} - BranchId is empty", dept.Id);
                return;
            }

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(ct);

            const string sql = @"
                INSERT INTO ""Departments"" (""Id"", ""Name"", ""NameAm"", ""DeptStat"", ""BranchId"", ""SyncedAt"", ""IsDeleted"")
                VALUES (@Id, @Name, @NameAm, @DeptStat, @BranchId, @SyncedAt, false)
                ON CONFLICT (""Id"") DO UPDATE SET
                    ""Name"" = EXCLUDED.""Name"",
                    ""NameAm"" = EXCLUDED.""NameAm"",
                    ""DeptStat"" = EXCLUDED.""DeptStat"",
                    ""BranchId"" = EXCLUDED.""BranchId"",
                    ""SyncedAt"" = EXCLUDED.""SyncedAt"",
                    ""IsDeleted"" = false";

            await connection.ExecuteAsync(sql, new
            {
                dept.Id,
                dept.Name,
                dept.NameAm,
                dept.DeptStat,
                dept.BranchId,
                SyncedAt = DateTime.UtcNow
            });

            // ✅ Invalidate cache
            await _cache.RemoveAsync($"department_{dept.Id}", ct);
            await _cache.RemoveAsync("departments_all", ct);
            await _cache.RemoveAsync($"departments_by_branch_{dept.BranchId}", ct);

            _logger.LogInformation("✅ Synced Department: {DepartmentId} - {DepartmentName}", dept.Id, dept.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to sync department: {DepartmentId}", dept.Id);
            throw;
        }
    }

    // ============================================================
    // SYNC POSITIONS
    // ============================================================



    // ============================================================
    // BULK OPERATIONS (Optional - for better performance)
    // ============================================================

    public async Task BulkSyncCompaniesAsync(List<LocalCompany> companies, CancellationToken ct = default)
    {
        if (companies == null || companies.Count == 0) return;

        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(ct);

        const string sql = @"
            INSERT INTO ""Companies"" (""Id"", ""Name"", ""NameAm"", ""TaxId"", ""Phone"",
                ""Email"", ""Address"", ""LogoUrl"", ""SyncedAt"", ""IsDeleted"")
            VALUES (@Id, @Name, @NameAm, @TaxId, @Phone, @Email, @Address, @LogoUrl, @SyncedAt, false)
            ON CONFLICT (""Id"") DO UPDATE SET
                ""Name"" = EXCLUDED.""Name"",
                ""NameAm"" = EXCLUDED.""NameAm"",
                ""TaxId"" = EXCLUDED.""TaxId"",
                ""Phone"" = EXCLUDED.""Phone"",
                ""Email"" = EXCLUDED.""Email"",
                ""Address"" = EXCLUDED.""Address"",
                ""LogoUrl"" = EXCLUDED.""LogoUrl"",
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
            c.LogoUrl,
            SyncedAt = DateTime.UtcNow
        }));

        _logger.LogInformation("✅ Bulk synced {Count} companies", companies.Count);
    }
}