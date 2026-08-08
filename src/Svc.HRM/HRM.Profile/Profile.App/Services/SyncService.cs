using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Profile.Domain.Entities.Local;
using Microsoft.Extensions.Logging;
namespace Profile.App.Services;

public class SyncService : ISyncService
{
    private readonly string _connectionString;
    private readonly ILogger<SyncService> _logger;

    public SyncService(IConfiguration configuration, ILogger<SyncService> logger)
    {
        _connectionString = configuration.GetConnectionString("HRMProDbCon")
            ?? throw new InvalidOperationException("HRMProDbCon connection string not found");
        _logger = logger;
    }

    public async Task SyncCompanyAsync(LocalCompany company, CancellationToken ct = default)
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

        _logger.LogInformation("Synced Company to HRM.Profile: {CompanyId}", company.Id);
    }

    public async Task SyncBranchAsync(LocalBranch branch, CancellationToken ct = default)
    {
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

        _logger.LogInformation("Synced Branch to HRM.Profile: {BranchId}", branch.Id);
    }

    public async Task SyncDepartmentAsync(LocalDepartment dept, CancellationToken ct = default)
    {
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

        _logger.LogInformation("Synced Department to HRM.Profile: {DepartmentId}", dept.Id);
    }
public async Task SyncJgStepAsync(LocalJgStep jgStep, CancellationToken ct = default)
{
    using var connection = new NpgsqlConnection(_connectionString);
    await connection.OpenAsync(ct);

    const string sql = @"
        INSERT INTO ""JgStep"" (""Id"", ""Name"", ""Salary"", ""Currency"",
            ""SalaryPayFreq"", ""JobGradeId"", ""SyncedAt"", ""IsDeleted"")
        VALUES (@Id, @Name, @Salary, @Currency, @SalaryPayFreq, @JobGradeId, @SyncedAt, false)
        ON CONFLICT (""Id"") DO UPDATE SET
            ""Name"" = EXCLUDED.""Name"",
            ""Salary"" = EXCLUDED.""Salary"",
            ""Currency"" = EXCLUDED.""Currency"",
            ""SalaryPayFreq"" = EXCLUDED.""SalaryPayFreq"",
            ""JobGradeId"" = EXCLUDED.""JobGradeId"",
            ""SyncedAt"" = EXCLUDED.""SyncedAt"",
            ""IsDeleted"" = false";

    await connection.ExecuteAsync(sql, new
    {
        jgStep.Id,
        jgStep.Name,
        jgStep.Salary,
        jgStep.Currency,
        jgStep.SalaryPayFreq,
        jgStep.JobGradeId,
        SyncedAt = DateTime.UtcNow
    });

    _logger.LogInformation("Synced JgStep to HRM.Profile: {JgStepId}", jgStep.Id);
}
    public async Task SyncPositionAsync(LocalPosition position, CancellationToken ct = default)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(ct);

        const string sql = @"
            INSERT INTO ""Positions"" (""Id"", ""Name"", ""NameAm"", ""NoOfPosition"", ""IsVacant"",
                ""DepartmentId"", ""JobGradeId"", ""SyncedAt"", ""IsDeleted"")
            VALUES (@Id, @Name, @NameAm, @NoOfPosition, @IsVacant, @DepartmentId, @JobGradeId, @SyncedAt, false)
            ON CONFLICT (""Id"") DO UPDATE SET
                ""Name"" = EXCLUDED.""Name"",
                ""NameAm"" = EXCLUDED.""NameAm"",
                ""NoOfPosition"" = EXCLUDED.""NoOfPosition"",
                ""IsVacant"" = EXCLUDED.""IsVacant"",
                ""DepartmentId"" = EXCLUDED.""DepartmentId"",
                ""JobGradeId"" = EXCLUDED.""JobGradeId"",
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
            SyncedAt = DateTime.UtcNow
        });

        _logger.LogInformation("Synced Position to HRM.Profile: {PositionId}", position.Id);
    }

    public async Task SyncJobGradeAsync(LocalJobGrade jobGrade, CancellationToken ct = default)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(ct);

        const string sql = @"
            INSERT INTO ""JobGrades"" (""Id"", ""Name"", ""StartSalary"", ""MaxSalary"", ""SyncedAt"", ""IsDeleted"")
            VALUES (@Id, @Name, @StartSalary, @MaxSalary, @SyncedAt, false)
            ON CONFLICT (""Id"") DO UPDATE SET
                ""Name"" = EXCLUDED.""Name"",
                ""StartSalary"" = EXCLUDED.""StartSalary"",
                ""MaxSalary"" = EXCLUDED.""MaxSalary"",
                ""SyncedAt"" = EXCLUDED.""SyncedAt"",
                ""IsDeleted"" = false";

        await connection.ExecuteAsync(sql, new
        {
            jobGrade.Id,
            jobGrade.Name,
            jobGrade.StartSalary,
            jobGrade.MaxSalary,
            SyncedAt = DateTime.UtcNow
        });

        _logger.LogInformation("Synced JobGrade to HRM.Profile: {JobGradeId}", jobGrade.Id);
    }
}