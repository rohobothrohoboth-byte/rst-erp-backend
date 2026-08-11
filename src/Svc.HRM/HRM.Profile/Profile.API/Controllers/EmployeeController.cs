using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Profile.App.Commands;
using Profile.App.Queries;
using Profile.Domain.DTOs;
using Profile.App.Interfaces;
using Profile.Domain.Entities;
using Common;
using Dapper;
using EthiopianCalendar;
using Profile.App.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Profile.API.Controllers;

/// <summary>
/// Employees Management endpoints
/// </summary>
[ApiController]
[Route("api/hrm/profile/v{version:apiVersion}/Employee")]
[ApiVersion("1.0")]
public class EmployeeController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IDapperHelper _dapper;
    private readonly ICachedReferenceService _referenceService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<EmployeeController> _logger;

    // Cache keys
    private const string FILTER_CACHE_KEY = "employee_filter_options";
    private const string STATS_CACHE_KEY = "employee_stats";
    private static readonly TimeSpan FilterCacheDuration = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan StatsCacheDuration = TimeSpan.FromMinutes(5);

    public EmployeeController(
        IMediator mediator,
        IDapperHelper dapper,
        ICachedReferenceService referenceService,
        IMemoryCache cache,
        ILogger<EmployeeController> logger)
    {
        _mediator = mediator;
        _dapper = dapper;
        _referenceService = referenceService;
        _cache = cache;
        _logger = logger;
    }

    // ============================================================
    // GET: All Employees (Non-Paginated)
    // ============================================================
   [PerAuth("hr.emp.view")]
   [HttpGet("AllEmployee")]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
   public async Task<IActionResult> AllEmployee()
   {
       const string cacheKey = "all_employees";

       if (_cache.TryGetValue(cacheKey, out List<EmployeeListDto>? cached))
       {
           return Ok(ApiResponse<object>.Ok(cached!));
       }

       var response = await _mediator.Send(new EmployeeAllQry());
       _cache.Set(cacheKey, response, TimeSpan.FromMinutes(5));

       return Ok(ApiResponse<object>.Ok(response));
   }

    // ============================================================
    // GET: Employee by ID
    // ============================================================
    [PerAuth("hr.emp.view")]
    [HttpGet("GetEmployee/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmployee(Guid id)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var response = await _mediator.Send(new EmployeeByIdQry { Id = id });

        if (response == null)
        {
            throw new DomainException($"EMPLOYEE with id [{id}] NOT FOUND.");
        }

        _logger.LogDebug("GetEmployee completed in {Elapsed}ms", stopwatch.ElapsedMilliseconds);
        return Ok(ApiResponse<object>.Ok(response));
    }

    // ============================================================
    // GET: Employee Code by ID
    // ============================================================
    [PerAuth("hr.emp.view")]
    [HttpGet("GetEmployeeCode/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmployeeCode(Guid id)
    {
        var response = await _mediator.Send(new EmpCodeByIdQry { Id = id });
        if (response == null)
        {
            throw new DomainException($"EMPLOYEE with id [{id}] NOT FOUND.");
        }
        return Ok(ApiResponse<object>.Ok(new { Code = response }));
    }

    // ============================================================
    // GET: Employee Print Details
    // ============================================================
    [PerAuth("hr.emp.print")]
    [HttpGet("Print/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmployeePrint(Guid id)
    {
        var response = await _mediator.Send(new EmpAddPrintQry { Id = id });
        if (response == null)
        {
            throw new DomainException($"EMPLOYEE with id [{id}] NOT FOUND.");
        }
        return Ok(ApiResponse<object>.Ok(response));
    }

    // ============================================================
    // GET: Step 2 (Basic Info)
    // ============================================================
    [PerAuth("hr.emp.view")]
    [HttpGet("Step2/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStep2(Guid id)
    {
        var response = await _mediator.Send(new Step2Qry { Id = id });
        if (response == null)
        {
            throw new DomainException($"EMPLOYEE with id [{id}] NOT FOUND.");
        }
        return Ok(ApiResponse<object>.Ok(response));
    }

   // ============================================================
   // GET: Paginated Employees (With Caching - FIXED)
   // ============================================================
   [PerAuth("hr.emp.view")]
   [HttpGet("paginated")]
   [ProducesResponseType(StatusCodes.Status200OK)]
   public async Task<IActionResult> GetPaginatedEmployees(
       [FromQuery] int pageNumber = 1,
       [FromQuery] int pageSize = 10,
       [FromQuery] string? sortBy = "DateAdd",
       [FromQuery] string? sortOrder = "desc",
       [FromQuery] string? searchTerm = null,
       [FromQuery] string? department = null,
       [FromQuery] string? branch = null,
       [FromQuery] string? empState = null,
       [FromQuery] string? empNature = null,
       [FromQuery] string? gender = null)
   {
       var stopwatch = System.Diagnostics.Stopwatch.StartNew();

       // ✅ LOG the received filter
       _logger.LogInformation($"📊 Received empState filter: '{empState}'");

       // ✅ Build cache key based on query parameters
       var cacheKey = $"employees_paginated_{pageNumber}_{pageSize}_{sortBy}_{sortOrder}_{searchTerm ?? "null"}_{department ?? "null"}_{branch ?? "null"}_{empState ?? "null"}_{empNature ?? "null"}_{gender ?? "null"}";

       // ✅ ONLY use cache if there are NO filters (all employees)
       // This prevents stale data for filtered views
       var shouldCache = pageNumber == 1 && pageSize <= 20 &&
                         string.IsNullOrEmpty(searchTerm) &&
                         string.IsNullOrEmpty(department) &&
                         string.IsNullOrEmpty(branch) &&
                         string.IsNullOrEmpty(empState) &&      // ← Only cache when NO status filter
                         string.IsNullOrEmpty(empNature) &&
                         string.IsNullOrEmpty(gender);

       if (shouldCache)
       {
           if (_cache.TryGetValue(cacheKey, out PaginatedResult<EmployeeListDto>? cached))
           {
               _logger.LogDebug("✅ Paginated cache HIT in {Elapsed}ms", stopwatch.ElapsedMilliseconds);
               return Ok(ApiResponse<PaginatedResult<EmployeeListDto>>.Ok(cached!));
           }
       }
       else
       {
           _logger.LogDebug($"⏳ Cache SKIPPED for filtered request (empState: '{empState}')");
       }

       _logger.LogDebug("⏳ Paginated cache MISS, executing query...");

       var query = new EmployeePaginatedQry
       {
           PageNumber = pageNumber,
           PageSize = pageSize,
           SortBy = sortBy,
           SortOrder = sortOrder,
           SearchTerm = searchTerm,
           Department = department,
           Branch = branch,
           EmpState = empState,
           EmpNature = empNature,
           Gender = gender
       };

       var result = await _mediator.Send(query);

       // ✅ Cache ONLY the unfiltered "All Employees" view
       if (shouldCache)
       {
           _cache.Set(cacheKey, result, TimeSpan.FromMinutes(2));
           _logger.LogDebug("📦 Paginated result cached for 2 minutes");
       }

       _logger.LogInformation("✅ Paginated query completed in {Elapsed}ms with {Count} items (filter: {Filter})",
           stopwatch.ElapsedMilliseconds, result.Items?.Count ?? 0, empState ?? "All");

       return Ok(ApiResponse<PaginatedResult<EmployeeListDto>>.Ok(result));
   }

    // ============================================================
    // GET: Filter Options (With Caching)
    // ============================================================
    [PerAuth("hr.emp.view")]
    [HttpGet("filter-options")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFilterOptions()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // ✅ Check cache first
        if (_cache.TryGetValue(FILTER_CACHE_KEY, out object? cached))
        {
            _logger.LogDebug("✅ Filter options cache HIT in {Elapsed}ms", stopwatch.ElapsedMilliseconds);
            return Ok(cached);
        }

        _logger.LogDebug("⏳ Filter options cache MISS, fetching...");

        try
        {
            // ✅ Use cached reference service
            var (deptDict, posDict, _) = await _referenceService.GetReferenceDataAsync(CancellationToken.None);

            var departments = deptDict.Select(d => new
            {
                id = d.Key,
                name = d.Value.Name,
                nameAm = d.Value.NameAm
            }).ToList();

            var positions = posDict.Select(p => new
            {
                id = p.Key,
                name = p.Value.Name
            }).ToList();

            // Get cached enum values
            var empStates = GetCachedEnumValues<EmpState>();
            var empNatures = GetCachedEnumValues<EmpNature>();
            var genders = GetCachedEnumValues<Gender>();

            var result = new
            {
                Departments = departments,
                Positions = positions,
                EmpStates = empStates,
                EmpNatures = empNatures,
                Genders = genders
            };

            // ✅ Cache the result
            _cache.Set(FILTER_CACHE_KEY, result, FilterCacheDuration);
            _logger.LogDebug("✅ Filter options cached in {Elapsed}ms", stopwatch.ElapsedMilliseconds);

            return Ok(ApiResponse<object>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting filter options");

            // Fallback to cached enums only
            var fallback = new
            {
                Departments = new List<object>(),
                Positions = new List<object>(),
                EmpStates = GetCachedEnumValues<EmpState>(),
                EmpNatures = GetCachedEnumValues<EmpNature>(),
                Genders = GetCachedEnumValues<Gender>()
            };
            return Ok(ApiResponse<object>.Ok(fallback));
        }
    }

    // ============================================================
    // GET: Employee Stats (With Caching)
    // ============================================================
    [PerAuth("hr.emp.view")]
    [HttpGet("stats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmployeeStats()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // ✅ Check cache first
        if (_cache.TryGetValue(STATS_CACHE_KEY, out EmployeeStatsDto? cached))
        {
            _logger.LogDebug("✅ Stats cache HIT in {Elapsed}ms", stopwatch.ElapsedMilliseconds);
            return Ok(ApiResponse<EmployeeStatsDto>.Ok(cached!));
        }

        _logger.LogDebug("⏳ Stats cache MISS, executing query...");

        var result = await _mediator.Send(new EmployeeStatsQry());

        // ✅ Cache for 5 minutes
        _cache.Set(STATS_CACHE_KEY, result, StatsCacheDuration);
        _logger.LogDebug("✅ Stats cached in {Elapsed}ms", stopwatch.ElapsedMilliseconds);

        return Ok(ApiResponse<EmployeeStatsDto>.Ok(result));
    }

    // ============================================================
    // PATCH: Update Employee Status
    // ============================================================
    [PerAuth("hr.emp.mod")]
    [HttpPatch("UpdateEmployeeStatus/{id}")]
    [HttpPost("UpdateEmployeeStatus/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEmployeeStatus(Guid id, [FromBody] UpdateStatusRequest request)
    {
        if (string.IsNullOrEmpty(request.EmpState))
        {
            throw new ValException("EmpState is required");
        }

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        const string sql = @"
            UPDATE ""Employee""
            SET ""EmpState"" = @EmpState,
                ""DateMod"" = NOW()
            WHERE ""Id"" = @Id";

        var rowsAffected = await _dapper.ExecuteAsync(sql, new { Id = id, EmpState = request.EmpState });

        if (rowsAffected == 0)
        {
            throw new DomainException($"Employee with id {id} not found");
        }

        // ✅ Invalidate stats cache
        _cache.Remove(STATS_CACHE_KEY);
        _cache.Remove(FILTER_CACHE_KEY);

        _logger.LogInformation("✅ Employee {Id} status updated to {Status} in {Elapsed}ms",
            id, request.EmpState, stopwatch.ElapsedMilliseconds);

        return Ok(ApiResponse<object>.Ok(null, $"Employee status updated to {request.EmpState}"));
    }

    // ============================================================
    // PUT: Update Employee
    // ============================================================
    [PerAuth("hr.emp.mod")]
    [HttpPut("ModEmployee/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] EmployeeModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var command = new EmployeeModCmd { ModDto = modDto };
        var response = await _mediator.Send(command);

        // ✅ Invalidate caches
        _cache.Remove(STATS_CACHE_KEY);
        _cache.Remove(FILTER_CACHE_KEY);

        _logger.LogInformation("✅ Employee {Id} updated in {Elapsed}ms", id, stopwatch.ElapsedMilliseconds);

        return Ok(ApiResponse<object>.Ok(response, "Selected EMPLOYEE successfully updated."));
    }

    // ============================================================
    // DELETE: Employee
    // ============================================================
    [PerAuth("hr.emp.del")]
    [HttpDelete("DelEmployee/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var command = new EmployeeDelCmd { Id = id };
        await _mediator.Send(command);

        // ✅ Invalidate caches
        _cache.Remove(STATS_CACHE_KEY);
        _cache.Remove(FILTER_CACHE_KEY);

        _logger.LogInformation("✅ Employee {Id} deleted in {Elapsed}ms", id, stopwatch.ElapsedMilliseconds);

        return Ok(ApiResponse<string>.Ok(null!, $"EMPLOYEE with Id {id} successfully deleted."));
    }

    // ============================================================
    // GET: User Scope
    // ============================================================
    [PerAuth("hr.emp.view")]
    [HttpGet("user-scope/{employeeId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserScope(string employeeId)
    {
        var userScopeService = HttpContext.RequestServices.GetRequiredService<IUserScopeService>();
        var scope = await userScopeService.GetUserScopeAsync(employeeId);

        if (scope == null)
        {
            throw new DomainException($"Employee with ID [{employeeId}] NOT FOUND.");
        }

        return Ok(ApiResponse<UserScopeDto>.Ok(scope));
    }

    // ============================================================
    // GET: User Branch ID
    // ============================================================
    [PerAuth("hr.emp.view")]
    [HttpGet("user-branch/{employeeId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserBranchId(string employeeId)
    {
        var userScopeService = HttpContext.RequestServices.GetRequiredService<IUserScopeService>();
        var branchId = await userScopeService.GetUserBranchIdAsync(employeeId);

        return Ok(ApiResponse<object>.Ok(new { BranchId = branchId }));
    }

    // ============================================================
    // GET: User Department ID
    // ============================================================
    [PerAuth("hr.emp.view")]
    [HttpGet("user-department/{employeeId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserDepartmentId(string employeeId)
    {
        var userScopeService = HttpContext.RequestServices.GetRequiredService<IUserScopeService>();
        var departmentId = await userScopeService.GetUserDepartmentIdAsync(employeeId);

        return Ok(ApiResponse<object>.Ok(new { DepartmentId = departmentId }));
    }

    // ============================================================
    // GET: Users Paginated
    // ============================================================
    [PerAuth("hr.emp.view")]
    [HttpGet("users/paginated")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaginatedUsers([FromQuery] UserPaginatedQry query)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await _mediator.Send(query);

        _logger.LogDebug("✅ Users paginated completed in {Elapsed}ms", stopwatch.ElapsedMilliseconds);

        return Ok(ApiResponse<PaginatedResult<UserListDto>>.Ok(result));
    }

    // ============================================================
    // POST: Invalidate Cache (Admin only)
    // ============================================================
    [PerAuth("hr.emp.mod")]
    [HttpPost("invalidate-cache")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult InvalidateCache()
    {
        _cache.Remove(FILTER_CACHE_KEY);
        _cache.Remove(STATS_CACHE_KEY);
        _referenceService.InvalidateCache();

        _logger.LogInformation("🗑️ All caches invalidated");

        return Ok(ApiResponse<object>.Ok(null, "Cache invalidated successfully"));
    }

    // ============================================================
    // PRIVATE HELPER: Get Cached Enum Values
    // ============================================================
   // ✅ FIXED: Use struct, Enum constraint
   private List<object> GetCachedEnumValues<T>() where T : struct, Enum
   {
       var enumCacheKey = $"enum_{typeof(T).Name}";

       if (_cache.TryGetValue(enumCacheKey, out List<object>? cached))
       {
           return cached!;
       }

       var values = Enum.GetValues(typeof(T))
           .Cast<T>()
           .Select(e => new
           {
               value = e.ToString(),
               label = MyEnumHelper.FormatEnum<T>(e.ToString())  // ✅ Now works
           })
           .Cast<object>()
           .ToList();

       _cache.Set(enumCacheKey, values, TimeSpan.FromHours(1));
       return values;
   }

    // ============================================================
    // REQUEST DTOs
    // ============================================================
    public class UpdateStatusRequest
    {
        public string EmpState { get; set; } = string.Empty;
    }
}