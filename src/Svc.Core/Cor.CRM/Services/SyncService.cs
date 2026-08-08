using Cor.CRM.Models.DTOs;
using Cor.CRM.Models.Entities;
using Cor.CRM.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Shared.Helpers.Services;
using Task = System.Threading.Tasks.Task;
using Cor.CRM.Models.Entities.Local;
namespace Cor.CRM.Services;

public class SyncService : ISyncService
{
    private readonly CrmDbContext _context;
    private readonly ILogger<SyncService> _logger;
    private readonly string _connectionString;
    private readonly ICacheService _cache;

    public SyncService(IConfiguration configuration, CrmDbContext context, ILogger<SyncService> logger, ICacheService cache)
    {
        _connectionString = configuration.GetConnectionString("coreCRMDbCon")
            ?? throw new InvalidOperationException("coreCRMDbCon connection string not found");
        _logger = logger;
        _cache = cache;
        _context = context;
    }

    // ==================== COMPANY SYNC ====================
    public async Task SyncCompanyAsync(LocalCompanyDto company, CancellationToken ct = default)
    {
        try
        {
            var existing = await _context.LocalCompanies.FirstOrDefaultAsync(x => x.Id == company.Id, ct);
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

    public async Task BulkSyncCompaniesAsync(List<LocalCompanyDto> companies, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Starting bulk sync for {Count} companies", companies.Count);
            foreach (var company in companies)
            {
                await SyncCompanyAsync(company, ct);
            }
            _logger.LogInformation("Completed bulk sync for {Count} companies", companies.Count);
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
            var existing = await _context.LocalBranches.FirstOrDefaultAsync(x => x.Id == branch.Id, ct);
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
            _logger.LogInformation("Soft deleted branch: {BranchId}", id);
        }
    }

    // ==================== DEPARTMENT SYNC ====================
    public async Task SyncDepartmentAsync(DepartmentDto department, CancellationToken ct = default)
    {
        try
        {
            var existing = await _context.LocalDepartments.FirstOrDefaultAsync(x => x.Id == department.Id, ct);
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
            _logger.LogInformation("Soft deleted department: {DepartmentId}", id);
        }
    }

    // ==================== POSITION SYNC ====================
    public async Task SyncPositionAsync(LocalPosition position, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(position.Name) || string.IsNullOrEmpty(position.NameAm))
        {
            _logger.LogWarning("Skipping position {PositionId} - Name or NameAm is null", position.Id);
            return;
        }

        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(ct);

        const string sql = @"
            INSERT INTO ""LocalPositions"" (""Id"", ""Name"", ""NameAm"", ""NoOfPosition"", ""IsVacant"",
                ""DepartmentId"", ""JobGradeId"", ""DateAdd"", ""SyncedAt"", ""IsDeleted"")
            VALUES (@Id, @Name, @NameAm, @NoOfPosition, @IsVacant, @DepartmentId, @JobGradeId,
                @DateAdd, @SyncedAt, false)
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
            IsVacant = position.IsVacant ?? "Unknown",
            position.DepartmentId,
            position.JobGradeId,
            DateAdd = DateTime.UtcNow,
            SyncedAt = DateTime.UtcNow
        });

        _logger.LogInformation("Synced Position: {PositionId}", position.Id);
        await _cache.RemoveAsync($"position_{position.Id}", ct);
        await _cache.RemoveAsync("positions_all", ct);
    }

    // ==================== JOBGRADE SYNC ====================
    public async Task SyncJobGradeAsync(LocalJobGrade jobGrade, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(jobGrade.Name))
        {
            _logger.LogWarning("Skipping job grade {JobGradeId} - Name is null", jobGrade.Id);
            return;
        }

        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(ct);

        const string sql = @"
            INSERT INTO ""LocalJobGrades"" (""Id"", ""Name"", ""StartSalary"", ""MaxSalary"",
                ""DateAdd"", ""SyncedAt"", ""IsDeleted"")
            VALUES (@Id, @Name, @StartSalary, @MaxSalary,
                @DateAdd, @SyncedAt, false)
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
            SyncedAt = DateTime.UtcNow
        });

        _logger.LogInformation("Synced JobGrade: {JobGradeId}", jobGrade.Id);
        await _cache.RemoveAsync($"jobgrade_{jobGrade.Id}", ct);
        await _cache.RemoveAsync("jobgrades_all", ct);
    }

    // ==================== EMPLOYEE SYNC ====================
    public async Task SyncEmployeeAsync(LocalEmployee employee, CancellationToken ct = default)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(ct);

        const string sql = @"
            INSERT INTO ""LocalEmployees"" (""Id"", ""Code"", ""EmploymentType"", ""EmploymentNature"",
                ""WorkArrangement"", ""EmpState"", ""EmploymentDate"", ""PersonId"", ""JobGradeId"",
                ""PositionId"", ""DepartmentId"", ""AppUserId"", ""FirstName"", ""FirstNameAm"",
                ""MiddleName"", ""MiddleNameAm"", ""LastName"", ""LastNameAm"", ""Gender"",
                ""Nationality"", ""Email"", ""Phone"", ""DateAdd"", ""SyncedAt"", ""IsDeleted"")
            VALUES (@Id, @Code, @EmploymentType, @EmploymentNature, @WorkArrangement, @EmpState,
                @EmploymentDate, @PersonId, @JobGradeId, @PositionId, @DepartmentId, @AppUserId,
                @FirstName, @FirstNameAm, @MiddleName, @MiddleNameAm, @LastName, @LastNameAm,
                @Gender, @Nationality, @Email, @Phone, @DateAdd, @SyncedAt, false)
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
            SyncedAt = DateTime.UtcNow
        });

        _logger.LogInformation("Synced Employee: {EmployeeId} - {FirstName} {LastName}",
            employee.Id, employee.FirstName, employee.LastName);

        await _cache.RemoveAsync($"employee_{employee.Id}", ct);
        await _cache.RemoveAsync("employees_all", ct);
    }
public async Task SoftDeletePositionAsync(Guid id, CancellationToken ct = default)
{
    using var connection = new NpgsqlConnection(_connectionString);
    await connection.OpenAsync(ct);

    const string sql = @"
        UPDATE ""LocalPositions""
        SET ""IsDeleted"" = true, ""SyncedAt"" = @SyncedAt
        WHERE ""Id"" = @Id";

    await connection.ExecuteAsync(sql, new { Id = id, SyncedAt = DateTime.UtcNow });
    _logger.LogInformation("Soft deleted position: {PositionId}", id);

    await _cache.RemoveAsync($"position_{id}", ct);
    await _cache.RemoveAsync("positions_all", ct);
}

public async Task SoftDeleteJobGradeAsync(Guid id, CancellationToken ct = default)
{
    using var connection = new NpgsqlConnection(_connectionString);
    await connection.OpenAsync(ct);

    const string sql = @"
        UPDATE ""LocalJobGrades""
        SET ""IsDeleted"" = true, ""SyncedAt"" = @SyncedAt
        WHERE ""Id"" = @Id";

    await connection.ExecuteAsync(sql, new { Id = id, SyncedAt = DateTime.UtcNow });
    _logger.LogInformation("Soft deleted job grade: {JobGradeId}", id);

    await _cache.RemoveAsync($"jobgrade_{id}", ct);
    await _cache.RemoveAsync("jobgrades_all", ct);
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
            _logger.LogInformation("Soft deleted employee: {EmployeeId}", id);
        }
    }

    // ==================== LEAD SYNC ====================
    public async Task SyncLeadAsync(Lead lead, CancellationToken ct = default)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(ct);

        const string sql = @"
            INSERT INTO ""Leads"" (""Id"", ""FirstName"", ""LastName"", ""CompanyName"", ""Email"",
                ""Phone"", ""Mobile"", ""Address"", ""City"", ""State"", ""Country"",
                ""Status"", ""Source"", ""Priority"", ""Industry"", ""Title"", ""Description"",
                ""Budget"", ""EstimatedValue"", ""ExpectedCloseDate"", ""AssignedToUserId"",
                ""Score"", ""Tags"", ""IsConverted"", ""ConvertedDate"", ""CreatedAt"", ""SyncedAt"", ""IsDeleted"")
            VALUES (@Id, @FirstName, @LastName, @CompanyName, @Email,
                @Phone, @Mobile, @Address, @City, @State, @Country,
                @Status, @Source, @Priority, @Industry, @Title, @Description,
                @Budget, @EstimatedValue, @ExpectedCloseDate, @AssignedToUserId,
                @Score, @Tags, @IsConverted, @ConvertedDate, @CreatedAt, @SyncedAt, false)
            ON CONFLICT (""Id"") DO UPDATE SET
                ""FirstName"" = EXCLUDED.""FirstName"",
                ""LastName"" = EXCLUDED.""LastName"",
                ""CompanyName"" = EXCLUDED.""CompanyName"",
                ""Email"" = EXCLUDED.""Email"",
                ""Phone"" = EXCLUDED.""Phone"",
                ""Mobile"" = EXCLUDED.""Mobile"",
                ""Address"" = EXCLUDED.""Address"",
                ""City"" = EXCLUDED.""City"",
                ""State"" = EXCLUDED.""State"",
                ""Country"" = EXCLUDED.""Country"",
                ""Status"" = EXCLUDED.""Status"",
                ""Source"" = EXCLUDED.""Source"",
                ""Priority"" = EXCLUDED.""Priority"",
                ""Industry"" = EXCLUDED.""Industry"",
                ""Title"" = EXCLUDED.""Title"",
                ""Description"" = EXCLUDED.""Description"",
                ""Budget"" = EXCLUDED.""Budget"",
                ""EstimatedValue"" = EXCLUDED.""EstimatedValue"",
                ""ExpectedCloseDate"" = EXCLUDED.""ExpectedCloseDate"",
                ""AssignedToUserId"" = EXCLUDED.""AssignedToUserId"",
                ""Score"" = EXCLUDED.""Score"",
                ""Tags"" = EXCLUDED.""Tags"",
                ""IsConverted"" = EXCLUDED.""IsConverted"",
                ""ConvertedDate"" = EXCLUDED.""ConvertedDate"",
                ""SyncedAt"" = EXCLUDED.""SyncedAt"",
                ""IsDeleted"" = false";

        await connection.ExecuteAsync(sql, new
        {
            lead.Id,
            lead.FirstName,
            lead.LastName,
            lead.CompanyName,
            lead.Email,
            lead.Phone,
            lead.Mobile,
            lead.Address,
            lead.City,
            lead.State,
            lead.Country,
            lead.Status,
            lead.Source,
            lead.Priority,
            lead.Industry,
            lead.Title,
            lead.Description,
            lead.Budget,
            lead.EstimatedValue,
            lead.ExpectedCloseDate,
            lead.AssignedToUserId,
            lead.Score,
            lead.Tags,
            lead.IsConverted,
            lead.ConvertedDate,
            CreatedAt = DateTime.UtcNow,
            SyncedAt = DateTime.UtcNow
        });

        _logger.LogInformation("Synced Lead: {LeadId} - {FirstName} {LastName}",
            lead.Id, lead.FirstName, lead.LastName);

        await _cache.RemoveAsync($"lead_{lead.Id}", ct);
        await _cache.RemoveAsync("leads_all", ct);
    }

    public async Task BulkSyncLeadsAsync(List<Lead> leads, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Starting bulk sync for {Count} leads", leads.Count);
            foreach (var lead in leads)
            {
                await SyncLeadAsync(lead, ct);
            }
            _logger.LogInformation("Completed bulk sync for {Count} leads", leads.Count);
            await _cache.RemoveAsync("leads_all", ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in bulk sync for leads");
            throw;
        }
    }

    public async Task SoftDeleteLeadAsync(Guid id, CancellationToken ct = default)
    {
        var lead = await _context.Leads.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (lead != null)
        {
            lead.IsDeleted = true;
            lead.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
            await _cache.RemoveAsync($"lead_{id}", ct);
            await _cache.RemoveAsync("leads_all", ct);
            _logger.LogInformation("Soft deleted lead: {LeadId}", id);
        }
    }

    // ==================== CUSTOMER SYNC ====================
    public async Task SyncCustomerAsync(Customer customer, CancellationToken ct = default)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(ct);

        const string sql = @"
            INSERT INTO ""Customers"" (""Id"", ""Name"", ""CompanyName"", ""Email"", ""Phone"",
                ""Mobile"", ""Address"", ""City"", ""State"", ""Country"", ""Status"", ""Type"",
                ""Industry"", ""Description"", ""AnnualRevenue"", ""EmployeeCount"", ""Website"",
                ""Tags"", ""CreatedAt"", ""SyncedAt"", ""IsDeleted"")
            VALUES (@Id, @Name, @CompanyName, @Email, @Phone,
                @Mobile, @Address, @City, @State, @Country, @Status, @Type,
                @Industry, @Description, @AnnualRevenue, @EmployeeCount, @Website,
                @Tags, @CreatedAt, @SyncedAt, false)
            ON CONFLICT (""Id"") DO UPDATE SET
                ""Name"" = EXCLUDED.""Name"",
                ""CompanyName"" = EXCLUDED.""CompanyName"",
                ""Email"" = EXCLUDED.""Email"",
                ""Phone"" = EXCLUDED.""Phone"",
                ""Mobile"" = EXCLUDED.""Mobile"",
                ""Address"" = EXCLUDED.""Address"",
                ""City"" = EXCLUDED.""City"",
                ""State"" = EXCLUDED.""State"",
                ""Country"" = EXCLUDED.""Country"",
                ""Status"" = EXCLUDED.""Status"",
                ""Type"" = EXCLUDED.""Type"",
                ""Industry"" = EXCLUDED.""Industry"",
                ""Description"" = EXCLUDED.""Description"",
                ""AnnualRevenue"" = EXCLUDED.""AnnualRevenue"",
                ""EmployeeCount"" = EXCLUDED.""EmployeeCount"",
                ""Website"" = EXCLUDED.""Website"",
                ""Tags"" = EXCLUDED.""Tags"",
                ""SyncedAt"" = EXCLUDED.""SyncedAt"",
                ""IsDeleted"" = false";

        await connection.ExecuteAsync(sql, new
        {
            customer.Id,
            customer.Name,
            customer.CompanyName,
            customer.Email,
            customer.Phone,
            customer.Mobile,
            customer.Address,
            customer.City,
            customer.State,
            customer.Country,
            customer.Status,
            customer.Type,
            customer.Industry,
            customer.Description,
            customer.AnnualRevenue,
            customer.EmployeeCount,
            customer.Website,
            customer.Tags,
            CreatedAt = DateTime.UtcNow,
            SyncedAt = DateTime.UtcNow
        });

        _logger.LogInformation("Synced Customer: {CustomerId} - {Name}", customer.Id, customer.Name);
        await _cache.RemoveAsync($"customer_{customer.Id}", ct);
        await _cache.RemoveAsync("customers_all", ct);
    }

    public async Task BulkSyncCustomersAsync(List<Customer> customers, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Starting bulk sync for {Count} customers", customers.Count);
            foreach (var customer in customers)
            {
                await SyncCustomerAsync(customer, ct);
            }
            _logger.LogInformation("Completed bulk sync for {Count} customers", customers.Count);
            await _cache.RemoveAsync("customers_all", ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in bulk sync for customers");
            throw;
        }
    }

    public async Task SoftDeleteCustomerAsync(Guid id, CancellationToken ct = default)
    {
        var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (customer != null)
        {
            customer.IsDeleted = true;
            customer.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
            await _cache.RemoveAsync($"customer_{id}", ct);
            await _cache.RemoveAsync("customers_all", ct);
            _logger.LogInformation("Soft deleted customer: {CustomerId}", id);
        }
    }
}