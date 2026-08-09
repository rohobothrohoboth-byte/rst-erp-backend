using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities.Local;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;

namespace Cor.Finance.Queries;

public class GetAllLocalCompaniesQry : IRequest<List<CompanyDto>>
{
    public bool? IsActive { get; set; }
}

public class GetAllLocalBranchesQry : IRequest<List<BranchDto>>
{
    public bool? IsActive { get; set; }
    public Guid? CompanyId { get; set; }
}

public class GetAllLocalDepartmentsQry : IRequest<List<DepartmentDto>>
{
    public bool? IsActive { get; set; }
    public Guid? BranchId { get; set; }
}

public class GetAllLocalEmployeesQry : IRequest<List<EmployeeDto>>
{
    public bool? IsActive { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? Position { get; set; }
}

public class GetLocalCompanyByIdQry : IRequest<CompanyDto?>
{
    public Guid Id { get; set; }
}

public class GetLocalBranchByIdQry : IRequest<BranchDto?>
{
    public Guid Id { get; set; }
}

public class GetLocalDepartmentByIdQry : IRequest<DepartmentDto?>
{
    public Guid Id { get; set; }
}

public class GetLocalEmployeeByIdQry : IRequest<EmployeeDto?>
{
    public Guid Id { get; set; }
}

// ============================================================
// OPTIMIZED HANDLERS WITH CACHING & AsNoTracking
// ============================================================

public class GetAllLocalCompaniesHandler : IRequestHandler<GetAllLocalCompaniesQry, List<CompanyDto>>
{
    private readonly FinanceDbContext _context;
    private readonly ICacheService _cache;
    private readonly ILogger<GetAllLocalCompaniesHandler> _logger;
    private const string CACHE_KEY_PREFIX = "local:companies:";
    private const int CACHE_DURATION_MINUTES = 5;

    public GetAllLocalCompaniesHandler(
        FinanceDbContext context,
        ICacheService cache,
        ILogger<GetAllLocalCompaniesHandler> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<CompanyDto>> Handle(GetAllLocalCompaniesQry request, CancellationToken ct)
    {
        var cacheKey = $"{CACHE_KEY_PREFIX}active:{request.IsActive ?? false}";

        // Try cache first
        var cached = await _cache.GetAsync<List<CompanyDto>>(cacheKey);
        if (cached != null)
        {
            _logger.LogDebug("Cache HIT: {CacheKey}", cacheKey);
            return cached;
        }

        _logger.LogDebug("Cache MISS: {CacheKey}", cacheKey);

        var companies = await _context.LocalCompanies
            .AsNoTracking()  // ✅ Read-only optimization
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Name)
            .Select(x => new CompanyDto
            {
                Id = x.Id,
                Name = x.Name,
                NameAm = x.NameAm,
                TaxId = x.TaxId,
                Phone = x.Phone,
                Email = x.Email,
                Address = x.Address,
                LogoUrl = null
            })
            .ToListAsync(ct);

        // Cache the result
        await _cache.SetAsync(cacheKey, companies, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));

        return companies;
    }
}

public class GetAllLocalBranchesHandler : IRequestHandler<GetAllLocalBranchesQry, List<BranchDto>>
{
    private readonly FinanceDbContext _context;
    private readonly ICacheService _cache;
    private readonly ILogger<GetAllLocalBranchesHandler> _logger;
    private const string CACHE_KEY_PREFIX = "local:branches:";
    private const int CACHE_DURATION_MINUTES = 5;

    public GetAllLocalBranchesHandler(
        FinanceDbContext context,
        ICacheService cache,
        ILogger<GetAllLocalBranchesHandler> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<BranchDto>> Handle(GetAllLocalBranchesQry request, CancellationToken ct)
    {
        var cacheKey = $"{CACHE_KEY_PREFIX}active:{request.IsActive ?? false}:company:{request.CompanyId ?? Guid.Empty}";

        var cached = await _cache.GetAsync<List<BranchDto>>(cacheKey);
        if (cached != null)
        {
            _logger.LogDebug("Cache HIT: {CacheKey}", cacheKey);
            return cached;
        }

        _logger.LogDebug("Cache MISS: {CacheKey}", cacheKey);

        var query = _context.LocalBranches
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        if (request.CompanyId.HasValue)
            query = query.Where(x => x.CompId == request.CompanyId.Value);

        var branches = await query
            .OrderBy(x => x.Name)
            .Select(x => new BranchDto
            {
                Id = x.Id,
                Name = x.Name,
                NameAm = x.NameAm,
                Code = x.Code,
                Location = x.Location ?? string.Empty,
                OpenDate = DateTime.UtcNow,
                BranchType = string.Empty,
                BranchStat = string.Empty,
                CompId = x.CompId ?? Guid.Empty
            })
            .ToListAsync(ct);

        await _cache.SetAsync(cacheKey, branches, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));

        return branches;
    }
}

public class GetAllLocalDepartmentsHandler : IRequestHandler<GetAllLocalDepartmentsQry, List<DepartmentDto>>
{
    private readonly FinanceDbContext _context;
    private readonly ICacheService _cache;
    private readonly ILogger<GetAllLocalDepartmentsHandler> _logger;
    private const string CACHE_KEY_PREFIX = "local:departments:";
    private const int CACHE_DURATION_MINUTES = 5;

    public GetAllLocalDepartmentsHandler(
        FinanceDbContext context,
        ICacheService cache,
        ILogger<GetAllLocalDepartmentsHandler> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<DepartmentDto>> Handle(GetAllLocalDepartmentsQry request, CancellationToken ct)
    {
        var cacheKey = $"{CACHE_KEY_PREFIX}active:{request.IsActive ?? false}:branch:{request.BranchId ?? Guid.Empty}";

        var cached = await _cache.GetAsync<List<DepartmentDto>>(cacheKey);
        if (cached != null)
        {
            _logger.LogDebug("Cache HIT: {CacheKey}", cacheKey);
            return cached;
        }

        _logger.LogDebug("Cache MISS: {CacheKey}", cacheKey);

        var query = _context.LocalDepartments
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        if (request.BranchId.HasValue)
            query = query.Where(x => x.BranchId == request.BranchId.Value);

        var departments = await query
            .OrderBy(x => x.Name)
            .Select(x => new DepartmentDto
            {
                Id = x.Id,
                Name = x.Name,
                NameAm = x.NameAm,
                DeptStat = string.Empty,
                BranchId = x.BranchId ?? Guid.Empty
            })
            .ToListAsync(ct);

        await _cache.SetAsync(cacheKey, departments, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));

        return departments;
    }
}

public class GetAllLocalEmployeesHandler : IRequestHandler<GetAllLocalEmployeesQry, List<EmployeeDto>>
{
    private readonly FinanceDbContext _context;
    private readonly ICacheService _cache;
    private readonly ILogger<GetAllLocalEmployeesHandler> _logger;
    private const string CACHE_KEY_PREFIX = "local:employees:";
    private const int CACHE_DURATION_MINUTES = 5;

    public GetAllLocalEmployeesHandler(
        FinanceDbContext context,
        ICacheService cache,
        ILogger<GetAllLocalEmployeesHandler> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<EmployeeDto>> Handle(GetAllLocalEmployeesQry request, CancellationToken ct)
    {
        var cacheKey = $"{CACHE_KEY_PREFIX}active:{request.IsActive ?? false}:dept:{request.DepartmentId ?? Guid.Empty}:pos:{request.Position ?? "all"}";

        var cached = await _cache.GetAsync<List<EmployeeDto>>(cacheKey);
        if (cached != null)
        {
            _logger.LogDebug("Cache HIT: {CacheKey}", cacheKey);
            return cached;
        }

        _logger.LogDebug("Cache MISS: {CacheKey}", cacheKey);

        var query = _context.LocalEmployees
            .AsNoTracking()
            .Include(x => x.Position)
            .Include(x => x.Department)
            .Include(x => x.JobGrade)
            .Where(x => !x.IsDeleted);

        if (request.DepartmentId.HasValue)
            query = query.Where(x => x.DepartmentId == request.DepartmentId.Value);

        if (!string.IsNullOrEmpty(request.Position))
            query = query.Where(x => x.Position != null && x.Position.Name == request.Position);

        var employees = await query
            .OrderBy(x => x.FirstName)
            .Select(x => new EmployeeDto
            {
                Id = x.Id,
                Code = x.Code,
                FirstName = x.FirstName,
                FirstNameAm = x.FirstNameAm,
                MiddleName = x.MiddleName,
                MiddleNameAm = x.MiddleNameAm,
                LastName = x.LastName,
                LastNameAm = x.LastNameAm,
                Gender = x.Gender,
                Nationality = x.Nationality,
                Email = x.Email,
                Phone = x.Phone,
                DepartmentId = x.DepartmentId,
                JobGradeId = x.JobGradeId,
                PositionId = x.PositionId,
                BranchId = null,
                PersonId = x.PersonId,
                AppUserId = x.AppUserId,
                Position = x.Position != null ? x.Position.Name : null,
                Department = x.Department != null ? x.Department.Name : null,
                JobGrade = x.JobGrade != null ? x.JobGrade.Name : null,
                Branch = null,
                IsActive = x.AppUserId.HasValue,
                EmpState = x.EmpState,
                EmploymentType = x.EmploymentType,
                EmploymentNature = x.EmploymentNature,
                WorkArrangement = x.WorkArrangement,
                EmploymentDate = x.EmploymentDate
            })
            .ToListAsync(ct);

        await _cache.SetAsync(cacheKey, employees, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));

        return employees;
    }
}

// ============================================================
// SINGLE ENTITY HANDLERS (With Caching)
// ============================================================

public class GetLocalCompanyByIdHandler : IRequestHandler<GetLocalCompanyByIdQry, CompanyDto?>
{
    private readonly FinanceDbContext _context;
    private readonly ICacheService _cache;
    private readonly ILogger<GetLocalCompanyByIdHandler> _logger;
    private const int CACHE_DURATION_MINUTES = 10;

    public GetLocalCompanyByIdHandler(
        FinanceDbContext context,
        ICacheService cache,
        ILogger<GetLocalCompanyByIdHandler> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<CompanyDto?> Handle(GetLocalCompanyByIdQry request, CancellationToken ct)
    {
        var cacheKey = $"local:company:{request.Id}";

        var cached = await _cache.GetAsync<CompanyDto>(cacheKey);
        if (cached != null)
        {
            _logger.LogDebug("Cache HIT: {CacheKey}", cacheKey);
            return cached;
        }

        var company = await _context.LocalCompanies
            .AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
            .Select(x => new CompanyDto
            {
                Id = x.Id,
                Name = x.Name,
                NameAm = x.NameAm,
                TaxId = x.TaxId,
                Phone = x.Phone,
                Email = x.Email,
                Address = x.Address,
                LogoUrl = null
            })
            .FirstOrDefaultAsync(ct);

        if (company != null)
        {
            await _cache.SetAsync(cacheKey, company, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));
        }

        return company;
    }
}

public class GetLocalBranchByIdHandler : IRequestHandler<GetLocalBranchByIdQry, BranchDto?>
{
    private readonly FinanceDbContext _context;
    private readonly ICacheService _cache;
    private readonly ILogger<GetLocalBranchByIdHandler> _logger;
    private const int CACHE_DURATION_MINUTES = 10;

    public GetLocalBranchByIdHandler(
        FinanceDbContext context,
        ICacheService cache,
        ILogger<GetLocalBranchByIdHandler> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<BranchDto?> Handle(GetLocalBranchByIdQry request, CancellationToken ct)
    {
        var cacheKey = $"local:branch:{request.Id}";

        var cached = await _cache.GetAsync<BranchDto>(cacheKey);
        if (cached != null)
        {
            _logger.LogDebug("Cache HIT: {CacheKey}", cacheKey);
            return cached;
        }

        var branch = await _context.LocalBranches
            .AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
            .Select(x => new BranchDto
            {
                Id = x.Id,
                Name = x.Name,
                NameAm = x.NameAm,
                Code = x.Code,
                Location = x.Location ?? string.Empty,
                OpenDate = DateTime.UtcNow,
                BranchType = string.Empty,
                BranchStat = string.Empty,
                CompId = x.CompId ?? Guid.Empty
            })
            .FirstOrDefaultAsync(ct);

        if (branch != null)
        {
            await _cache.SetAsync(cacheKey, branch, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));
        }

        return branch;
    }
}

public class GetLocalDepartmentByIdHandler : IRequestHandler<GetLocalDepartmentByIdQry, DepartmentDto?>
{
    private readonly FinanceDbContext _context;
    private readonly ICacheService _cache;
    private readonly ILogger<GetLocalDepartmentByIdHandler> _logger;
    private const int CACHE_DURATION_MINUTES = 10;

    public GetLocalDepartmentByIdHandler(
        FinanceDbContext context,
        ICacheService cache,
        ILogger<GetLocalDepartmentByIdHandler> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<DepartmentDto?> Handle(GetLocalDepartmentByIdQry request, CancellationToken ct)
    {
        var cacheKey = $"local:department:{request.Id}";

        var cached = await _cache.GetAsync<DepartmentDto>(cacheKey);
        if (cached != null)
        {
            _logger.LogDebug("Cache HIT: {CacheKey}", cacheKey);
            return cached;
        }

        var department = await _context.LocalDepartments
            .AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
            .Select(x => new DepartmentDto
            {
                Id = x.Id,
                Name = x.Name,
                NameAm = x.NameAm,
                DeptStat = string.Empty,
                BranchId = x.BranchId ?? Guid.Empty
            })
            .FirstOrDefaultAsync(ct);

        if (department != null)
        {
            await _cache.SetAsync(cacheKey, department, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));
        }

        return department;
    }
}

public class GetLocalEmployeeByIdHandler : IRequestHandler<GetLocalEmployeeByIdQry, EmployeeDto?>
{
    private readonly FinanceDbContext _context;
    private readonly ICacheService _cache;
    private readonly ILogger<GetLocalEmployeeByIdHandler> _logger;
    private const int CACHE_DURATION_MINUTES = 10;

    public GetLocalEmployeeByIdHandler(
        FinanceDbContext context,
        ICacheService cache,
        ILogger<GetLocalEmployeeByIdHandler> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<EmployeeDto?> Handle(GetLocalEmployeeByIdQry request, CancellationToken ct)
    {
        var cacheKey = $"local:employee:{request.Id}";

        var cached = await _cache.GetAsync<EmployeeDto>(cacheKey);
        if (cached != null)
        {
            _logger.LogDebug("Cache HIT: {CacheKey}", cacheKey);
            return cached;
        }

        var employee = await _context.LocalEmployees
            .AsNoTracking()
            .Include(x => x.Position)
            .Include(x => x.Department)
            .Include(x => x.JobGrade)
            .Where(x => x.Id == request.Id && !x.IsDeleted)
            .Select(x => new EmployeeDto
            {
                Id = x.Id,
                Code = x.Code,
                FirstName = x.FirstName,
                FirstNameAm = x.FirstNameAm,
                MiddleName = x.MiddleName,
                MiddleNameAm = x.MiddleNameAm,
                LastName = x.LastName,
                LastNameAm = x.LastNameAm,
                Gender = x.Gender,
                Nationality = x.Nationality,
                Email = x.Email,
                Phone = x.Phone,
                DepartmentId = x.DepartmentId,
                JobGradeId = x.JobGradeId,
                PositionId = x.PositionId,
                BranchId = null,
                PersonId = x.PersonId,
                AppUserId = x.AppUserId,
                Position = x.Position != null ? x.Position.Name : null,
                Department = x.Department != null ? x.Department.Name : null,
                JobGrade = x.JobGrade != null ? x.JobGrade.Name : null,
                Branch = null,
                IsActive = x.AppUserId.HasValue,
                EmpState = x.EmpState,
                EmploymentType = x.EmploymentType,
                EmploymentNature = x.EmploymentNature,
                WorkArrangement = x.WorkArrangement,
                EmploymentDate = x.EmploymentDate
            })
            .FirstOrDefaultAsync(ct);

        if (employee != null)
        {
            await _cache.SetAsync(cacheKey, employee, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));
        }

        return employee;
    }
}