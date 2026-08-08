using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Cor.Procurement.Models.DTOs;
using Shared.Helpers.Services;
using Cor.Procurement.Persistence;

namespace Cor.Procurement.Services;

public class CachedLocalCopyService
{
    private readonly ProcurementDbContext _context;
    private readonly ICacheService _cache;
    private readonly ILogger<CachedLocalCopyService> _logger;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

    public CachedLocalCopyService(
        ProcurementDbContext context,
        ICacheService cache,
        ILogger<CachedLocalCopyService> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    // ============================================================
    // COMPANIES
    // ============================================================

    public async Task<List<CompanyDto>> GetCompaniesAsync(CancellationToken ct = default)
    {
        const string cacheKey = "localcopy:companies";

        try
        {
            var cached = await _cache.GetAsync<List<CompanyDto>>(cacheKey, ct);
            if (cached != null)
            {
                _logger.LogDebug("✅ Companies retrieved from cache");
                return cached;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get companies from cache, falling back to database");
        }

        var companies = await _context.LocalCompanies
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Name)
            .Select(x => new CompanyDto
            {
                Id = x.Id,
                Name = x.Name,
                NameAm = x.NameAm ?? string.Empty,
                TaxId = x.TaxId,
                Phone = x.Phone,
                Email = x.Email,
                Address = x.Address
                // LogoUrl removed - not in entity
            })
            .ToListAsync(ct);

        try
        {
            await _cache.SetAsync(cacheKey, companies, _cacheDuration, ct);
            _logger.LogDebug("✅ Companies cached successfully");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to cache companies");
        }

        return companies;
    }

    // ============================================================
    // BRANCHES
    // ============================================================

    public async Task<List<BranchDto>> GetBranchesAsync(CancellationToken ct = default)
    {
        const string cacheKey = "localcopy:branches";

        try
        {
            var cached = await _cache.GetAsync<List<BranchDto>>(cacheKey, ct);
            if (cached != null)
            {
                _logger.LogDebug("✅ Branches retrieved from cache");
                return cached;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get branches from cache, falling back to database");
        }

        var branches = await _context.LocalBranches
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Name)
            .Select(x => new BranchDto
            {
                Id = x.Id,
                Name = x.Name,
                NameAm = x.NameAm ?? string.Empty,
                Code = x.Code ?? string.Empty,
                Location = x.Location ?? string.Empty,
                // OpenDate removed - not in entity
                // BranchType removed - not in entity
                // BranchStat removed - not in entity
                CompId = x.CompId ?? Guid.Empty  // Handle nullable
            })
            .ToListAsync(ct);

        try
        {
            await _cache.SetAsync(cacheKey, branches, _cacheDuration, ct);
            _logger.LogDebug("✅ Branches cached successfully");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to cache branches");
        }

        return branches;
    }

    // ============================================================
    // DEPARTMENTS
    // ============================================================

    public async Task<List<DepartmentDto>> GetDepartmentsAsync(CancellationToken ct = default)
    {
        const string cacheKey = "localcopy:departments";

        try
        {
            var cached = await _cache.GetAsync<List<DepartmentDto>>(cacheKey, ct);
            if (cached != null)
            {
                _logger.LogDebug("✅ Departments retrieved from cache");
                return cached;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get departments from cache, falling back to database");
        }

        var departments = await _context.LocalDepartments
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Name)
            .Select(x => new DepartmentDto
            {
                Id = x.Id,
                Name = x.Name,
                NameAm = x.NameAm ?? string.Empty,
                // DeptStat removed - not in entity
                BranchId = x.BranchId ?? Guid.Empty,  // Handle nullable
                // Branch removed - not in entity
                // BranchAm removed - not in entity
                Branch = null,  // Not in entity
                BranchAm = null  // Not in entity
            })
            .ToListAsync(ct);

        try
        {
            await _cache.SetAsync(cacheKey, departments, _cacheDuration, ct);
            _logger.LogDebug("✅ Departments cached successfully");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to cache departments");
        }

        return departments;
    }

    // ============================================================
    // POSITIONS
    // ============================================================

    public async Task<List<PositionDto>> GetPositionsAsync(CancellationToken ct = default)
    {
        const string cacheKey = "localcopy:positions";

        try
        {
            var cached = await _cache.GetAsync<List<PositionDto>>(cacheKey, ct);
            if (cached != null)
            {
                _logger.LogDebug("✅ Positions retrieved from cache");
                return cached;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get positions from cache, falling back to database");
        }

        var positions = await _context.LocalPositions
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Name)
            .Select(x => new PositionDto
            {
                Id = x.Id,
                Name = x.Name,
                NameAm = x.NameAm ?? string.Empty,
                NoOfPosition = x.NoOfPosition,
                IsVacant = x.IsVacant ?? "No",
                DepartmentId = x.DepartmentId,
                JobGradeId = x.JobGradeId
            })
            .ToListAsync(ct);

        try
        {
            await _cache.SetAsync(cacheKey, positions, _cacheDuration, ct);
            _logger.LogDebug("✅ Positions cached successfully");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to cache positions");
        }

        return positions;
    }

    // ============================================================
    // JOB GRADES
    // ============================================================

    public async Task<List<JobGradeDto>> GetJobGradesAsync(CancellationToken ct = default)
    {
        const string cacheKey = "localcopy:jobgrades";

        try
        {
            var cached = await _cache.GetAsync<List<JobGradeDto>>(cacheKey, ct);
            if (cached != null)
            {
                _logger.LogDebug("✅ Job Grades retrieved from cache");
                return cached;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get job grades from cache, falling back to database");
        }

        var jobGrades = await _context.LocalJobGrades
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Name)
            .Select(x => new JobGradeDto
            {
                Id = x.Id,
                Name = x.Name,
                StartSalary = x.StartSalary,
                MaxSalary = x.MaxSalary
            })
            .ToListAsync(ct);

        try
        {
            await _cache.SetAsync(cacheKey, jobGrades, _cacheDuration, ct);
            _logger.LogDebug("✅ Job Grades cached successfully");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to cache job grades");
        }

        return jobGrades;
    }

    // ============================================================
    // EMPLOYEES
    // ============================================================

    public async Task<List<EmployeeDto>> GetEmployeesAsync(CancellationToken ct = default)
    {
        const string cacheKey = "localcopy:employees";

        try
        {
            var cached = await _cache.GetAsync<List<EmployeeDto>>(cacheKey, ct);
            if (cached != null)
            {
                _logger.LogDebug("✅ Employees retrieved from cache ({Count} records)", cached.Count);
                return cached;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get employees from cache, falling back to database");
        }

        var employees = await _context.LocalEmployees
            .Include(x => x.Position)
            .Include(x => x.Department)
            .Include(x => x.JobGrade)
            .Include(x => x.Branch)
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.FirstName)
            .Select(x => new EmployeeDto
            {
                Id = x.Id,
                Code = x.Code ?? string.Empty,

                // IDs
                DepartmentId = x.DepartmentId,
                JobGradeId = x.JobGradeId,
                PositionId = x.PositionId,
                BranchId = x.BranchId,
                PersonId = x.PersonId,

                // Person fields (directly from LocalEmployee)
                FirstName = x.FirstName,
                FirstNameAm = x.FirstNameAm ?? string.Empty,
                MiddleName = x.MiddleName ?? string.Empty,
                MiddleNameAm = x.MiddleNameAm ?? string.Empty,
                LastName = x.LastName,
                LastNameAm = x.LastNameAm ?? string.Empty,
                Gender = x.Gender ?? "Not Specified",
                Nationality = x.Nationality ?? "Not Specified",
                Email = x.Email,
                Phone = x.Phone,

                // Display names from navigation properties
                Position = x.Position != null ? x.Position.Name : null,
                Department = x.Department != null ? x.Department.Name : null,
                JobGrade = x.JobGrade != null ? x.JobGrade.Name : null,
                Branch = x.Branch != null ? x.Branch.Name : null,

                // Employment fields
                AppUserId = x.AppUserId,
                IsActive = x.IsActive ?? true,
                EmpState = x.EmpState ?? "Active",
                EmploymentType = x.EmploymentType ?? "FullTime",
                EmploymentNature = x.EmploymentNature ?? "Permanent",
                WorkArrangement = x.WorkArrangement ?? "OnSite",
                EmploymentDate = x.EmploymentDate
            })
            .ToListAsync(ct);

        try
        {
            await _cache.SetAsync(cacheKey, employees, _cacheDuration, ct);
            _logger.LogDebug("✅ Employees cached successfully ({Count} records)", employees.Count);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to cache employees");
        }

        return employees;
    }

    // ============================================================
    // PERSONS
    // ============================================================

    public async Task<List<PersonDto>> GetPersonsAsync(CancellationToken ct = default)
    {
        const string cacheKey = "localcopy:persons";

        try
        {
            var cached = await _cache.GetAsync<List<PersonDto>>(cacheKey, ct);
            if (cached != null)
            {
                _logger.LogDebug("✅ Persons retrieved from cache");
                return cached;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get persons from cache, falling back to database");
        }

        // If you have a LocalPersons DbSet, use it; otherwise, get from LocalEmployees
        var persons = await _context.LocalEmployees
            .Where(x => !x.IsDeleted)
            .GroupBy(x => new { x.PersonId, x.FirstName, x.FirstNameAm, x.MiddleName, x.MiddleNameAm, x.LastName, x.LastNameAm, x.Gender, x.Nationality })
            .Select(g => new PersonDto
            {
                Id = g.Key.PersonId,
                FirstName = g.Key.FirstName,
                FirstNameAm = g.Key.FirstNameAm ?? string.Empty,
                MiddleName = g.Key.MiddleName ?? string.Empty,
                MiddleNameAm = g.Key.MiddleNameAm ?? string.Empty,
                LastName = g.Key.LastName,
                LastNameAm = g.Key.LastNameAm ?? string.Empty,
                Gender = g.Key.Gender ?? "Not Specified",
                Nationality = g.Key.Nationality ?? "Not Specified"
            })
            .ToListAsync(ct);

        try
        {
            await _cache.SetAsync(cacheKey, persons, _cacheDuration, ct);
            _logger.LogDebug("✅ Persons cached successfully");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to cache persons");
        }

        return persons;
    }

    // ============================================================
    // HELPER METHODS
    // ============================================================

    public async Task ClearCacheAsync(CancellationToken ct = default)
    {
        var keys = new[]
        {
            "localcopy:companies",
            "localcopy:branches",
            "localcopy:departments",
            "localcopy:positions",
            "localcopy:jobgrades",
            "localcopy:employees",
            "localcopy:persons"
        };

        foreach (var key in keys)
        {
            try
            {
                await _cache.RemoveAsync(key, ct);
                _logger.LogDebug("✅ Cache cleared for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to clear cache for key: {Key}", key);
            }
        }
    }

    public async Task RefreshCacheAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("🔄 Refreshing local copy cache...");

        await ClearCacheAsync(ct);

        // Reload all data
        await GetCompaniesAsync(ct);
        await GetBranchesAsync(ct);
        await GetDepartmentsAsync(ct);
        await GetPositionsAsync(ct);
        await GetJobGradesAsync(ct);
        await GetEmployeesAsync(ct);
        await GetPersonsAsync(ct);

        _logger.LogInformation("✅ Local copy cache refreshed successfully");
    }
}