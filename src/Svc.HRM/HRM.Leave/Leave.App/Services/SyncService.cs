using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Leave.Domain.Entities.Local;
using Microsoft.Extensions.Logging;
using Leave.Domain.DTOs;
using Shared.Helpers.Services;
namespace Leave.App.Services;

public class SyncService : ISyncService
{
    private readonly string _connectionString;
    private readonly ILogger<SyncService> _logger;
private readonly ICacheService _cache;
    public SyncService(IConfiguration configuration, ILogger<SyncService> logger,ICacheService cache)
    {
        _connectionString = configuration.GetConnectionString("HRMLeaveDbCon")
            ?? throw new InvalidOperationException("HRMProDbCon connection string not found");
        _logger = logger;
         _cache = cache;
    }










    public async Task SyncCompanyAsync(LocalCompany company, CancellationToken ct = default)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(ct);

        const string sql = @"
            INSERT INTO ""LocalCompanies"" (""Id"", ""Name"", ""NameAm"", ""TaxId"", ""Phone"",
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

        _logger.LogInformation("Synced Company to HRM.Leave: {CompanyId}", company.Id);
    }

    public async Task SyncBranchAsync(LocalBranch branch, CancellationToken ct = default)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(ct);

        const string sql = @"
            INSERT INTO ""LocalBranches"" (""Id"", ""Name"", ""NameAm"", ""Code"", ""Location"",
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

        _logger.LogInformation("Synced Branch to HRM.Leave: {BranchId}", branch.Id);
    }
     // Leave.App/Services/SyncService.cs

     public async Task SyncPositionRequirementAsync(PositionReqDto positionReq, CancellationToken ct = default)
     {
         using var connection = new NpgsqlConnection(_connectionString);
         await connection.OpenAsync(ct);

         // ✅ Use the correct table name: LocalPositionReq
         const string sql = @"
             INSERT INTO ""LocalPositionReq"" (""Id"", ""Gender"", ""ProfessionType"",
                 ""SaturdayWorkOption"", ""SundayWorkOption"", ""WorkingHours"",
                 ""PositionId"", ""DateAdd"", ""DateMod"", ""IsDeleted"", ""SyncedAt"")
             VALUES (@Id, @Gender, @ProfessionType, @SaturdayWorkOption, @SundayWorkOption,
                 @WorkingHours, @PositionId, @DateAdd, @DateMod, false, @SyncedAt)
             ON CONFLICT (""Id"") DO UPDATE SET
                 ""Gender"" = EXCLUDED.""Gender"",
                 ""ProfessionType"" = EXCLUDED.""ProfessionType"",
                 ""SaturdayWorkOption"" = EXCLUDED.""SaturdayWorkOption"",
                 ""SundayWorkOption"" = EXCLUDED.""SundayWorkOption"",
                 ""WorkingHours"" = EXCLUDED.""WorkingHours"",
                 ""PositionId"" = EXCLUDED.""PositionId"",
                 ""DateMod"" = EXCLUDED.""DateMod"",
                 ""IsDeleted"" = false,
                 ""SyncedAt"" = EXCLUDED.""SyncedAt""";

         await connection.ExecuteAsync(sql, new
         {
             positionReq.Id,
             positionReq.Gender,
             positionReq.ProfessionType,
             positionReq.SaturdayWorkOption,
             positionReq.SundayWorkOption,
             positionReq.WorkingHours,
             positionReq.PositionId,
             DateAdd = DateTime.UtcNow,
             DateMod = DateTime.UtcNow,
             SyncedAt = DateTime.UtcNow,
             IsDeleted = false
         });

         _logger.LogInformation("Synced PositionRequirement to HRM.Leave: {PositionReqId}", positionReq.Id);
     }
public async Task UpdatePositionWithRequirementsAsync(Guid positionId, PositionReqDto positionReq, CancellationToken ct = default)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(ct);

        const string sql = @"
            UPDATE ""LocalPositionReq""
            SET
                ""SaturdayWorkOption"" = @SaturdayWorkOption,
                ""SundayWorkOption"" = @SundayWorkOption,
                ""WorkingHours"" = @WorkingHours,
                ""Gender"" = @Gender,
                ""ProfessionType"" = @ProfessionType,
                ""SyncedAt"" = @SyncedAt
            WHERE ""Id"" = @PositionId";

        await connection.ExecuteAsync(sql, new
        {
            positionId,
            positionReq.SaturdayWorkOption,
            positionReq.SundayWorkOption,
            positionReq.WorkingHours,
            positionReq.Gender,
            positionReq.ProfessionType,
            SyncedAt = DateTime.UtcNow
        });

        _logger.LogInformation("Updated Position {PositionId} with requirements", positionId);
    }
    public async Task SyncDepartmentAsync(LocalDepartment dept, CancellationToken ct = default)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(ct);

        const string sql = @"
            INSERT INTO ""LocalDepartments"" (""Id"", ""Name"", ""NameAm"", ""DeptStat"", ""BranchId"", ""SyncedAt"", ""IsDeleted"")
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

        _logger.LogInformation("Synced Department to HRM.Leave: {DepartmentId}", dept.Id);
    }
public async Task SyncJgStepAsync(LocalJgStep jgStep, CancellationToken ct = default)
{
    using var connection = new NpgsqlConnection(_connectionString);
    await connection.OpenAsync(ct);

    const string sql = @"
        INSERT INTO ""LocalJgStep"" (""Id"", ""Name"", ""Salary"", ""Currency"",
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

    _logger.LogInformation("Synced JgStep to HRM.Leave: {JgStepId}", jgStep.Id);
}
   public async Task SyncPositionAsync(LocalPosition position, CancellationToken ct = default)
   {
       using var connection = new NpgsqlConnection(_connectionString);
       await connection.OpenAsync(ct);

       // ✅ Include DateAdd in the INSERT and UPDATE
       const string sql = @"
           INSERT INTO ""LocalPositions"" (""Id"", ""Name"", ""NameAm"", ""NoOfPosition"", ""IsVacant"",
               ""DepartmentId"", ""JobGradeId"", ""DateAdd"", ""DateMod"", ""IsDeleted"", ""SyncedAt"")
           VALUES (@Id, @Name, @NameAm, @NoOfPosition, @IsVacant, @DepartmentId, @JobGradeId,
               @DateAdd, @DateMod, false, @SyncedAt)
           ON CONFLICT (""Id"") DO UPDATE SET
               ""Name"" = EXCLUDED.""Name"",
               ""NameAm"" = EXCLUDED.""NameAm"",
               ""NoOfPosition"" = EXCLUDED.""NoOfPosition"",
               ""IsVacant"" = EXCLUDED.""IsVacant"",
               ""DepartmentId"" = EXCLUDED.""DepartmentId"",
               ""JobGradeId"" = EXCLUDED.""JobGradeId"",
               ""DateMod"" = EXCLUDED.""DateMod"",
               ""IsDeleted"" = false,
               ""SyncedAt"" = EXCLUDED.""SyncedAt""";

       await connection.ExecuteAsync(sql, new
       {
           position.Id,
           position.Name,
           position.NameAm,
           position.NoOfPosition,
           position.IsVacant,
           position.DepartmentId,
           position.JobGradeId,
           // ✅ Ensure DateAdd has a value (use current time if null)
           DateAdd = position.DateAdd != DateTime.MinValue ? position.DateAdd : DateTime.UtcNow,
           DateMod = DateTime.UtcNow,
           SyncedAt = DateTime.UtcNow,
           IsDeleted = false
       });

       _logger.LogInformation("Synced Position to HRM.Leave: {PositionId}", position.Id);
   }

   // Leave.App/Services/SyncService.cs

  public async Task SyncJobGradeAsync(LocalJobGrade jobGrade, CancellationToken ct = default)
  {
      using var connection = new NpgsqlConnection(_connectionString);
      await connection.OpenAsync(ct);

      // ✅ Match the number of columns with values
      const string sql = @"
          INSERT INTO ""LocalJobGrades"" (""Id"", ""Name"", ""StartSalary"", ""MaxSalary"",
              ""DateAdd"", ""SyncedAt"", ""IsDeleted"")
          VALUES (@Id, @Name, @StartSalary, @MaxSalary, @DateAdd, @SyncedAt, false)
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
          DateAdd = DateTime.UtcNow,
          SyncedAt = DateTime.UtcNow,
          IsDeleted = false
      });

      _logger.LogInformation("Synced JobGrade to HRM.Leave: {JobGradeId}", jobGrade.Id);
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
}