// LocalCopyController.cs - Fully Optimized with Caching

using Cor.Finance.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cor.Finance.Models.DTOs;
using Asp.Versioning;
using Shared.Helpers.Services;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace Cor.Finance.Controllers;

[ApiController]
[Route("api/finance/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class LocalCopyController : BaseApiController
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<LocalCopyController> _logger;
    private readonly IMediator _mediator;
    private const int CACHE_DURATION_MINUTES = 5;

    public LocalCopyController(
        IMediator mediator,
        ILogger<LocalCopyController> logger,
        ICacheService cacheService)
        : base(mediator, logger)
    {
        _mediator = mediator;
        _logger = logger;
        _cacheService = cacheService;
    }

    /// <summary>
    /// Get all local companies (from Core.Module sync)
    /// </summary>
    [HttpGet("Companies")]
    [ProducesResponseType(typeof(List<CompanyDto>), StatusCodes.Status200OK)]
    [ResponseCache(Duration = 300, VaryByQueryKeys = new[] { "isActive" })]
    public async Task<IActionResult> GetCompanies([FromQuery] bool? isActive)
    {
        try
        {
            var cacheKey = $"local:companies:active:{isActive ?? false}";

            // Try to get from cache
            var cached = await _cacheService.GetAsync<List<CompanyDto>>(cacheKey);
            if (cached != null)
            {
                _logger.LogDebug("📦 Cache HIT: {CacheKey}", cacheKey);
                return Ok(cached);
            }

            _logger.LogDebug("📦 Cache MISS: {CacheKey}", cacheKey);

            var result = await _mediator.Send(new GetAllLocalCompaniesQry { IsActive = isActive });

            // Cache the result
            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting local companies");
            return HandleException(ex, "GetLocalCompanies");
        }
    }

    /// <summary>
    /// Get company by ID
    /// </summary>
    [HttpGet("Company/{id}")]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(Duration = 300, VaryByQueryKeys = new[] { "id" })]
    public async Task<IActionResult> GetCompanyById(Guid id)
    {
        try
        {
            var cacheKey = $"local:company:{id}";

            var cached = await _cacheService.GetAsync<CompanyDto>(cacheKey);
            if (cached != null)
            {
                _logger.LogDebug("📦 Cache HIT: {CacheKey}", cacheKey);
                return Ok(cached);
            }

            var result = await _mediator.Send(new GetLocalCompanyByIdQry { Id = id });

            if (result == null)
                return NotFound($"Company with ID {id} not found");

            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting company by ID: {Id}", id);
            return HandleException(ex, "GetLocalCompanyById", id);
        }
    }

    /// <summary>
    /// Get all local branches (from Core.Module sync)
    /// </summary>
    [HttpGet("Branches")]
    [ProducesResponseType(typeof(List<BranchDto>), StatusCodes.Status200OK)]
    [ResponseCache(Duration = 300, VaryByQueryKeys = new[] { "isActive", "companyId" })]
    public async Task<IActionResult> GetBranches(
        [FromQuery] bool? isActive,
        [FromQuery] Guid? companyId)
    {
        try
        {
            var cacheKey = $"local:branches:active:{isActive ?? false}:company:{companyId ?? Guid.Empty}";

            var cached = await _cacheService.GetAsync<List<BranchDto>>(cacheKey);
            if (cached != null)
            {
                _logger.LogDebug("📦 Cache HIT: {CacheKey}", cacheKey);
                return Ok(cached);
            }

            _logger.LogDebug("📦 Cache MISS: {CacheKey}", cacheKey);

            var result = await _mediator.Send(new GetAllLocalBranchesQry
            {
                IsActive = isActive,
                CompanyId = companyId
            });

            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting local branches");
            return HandleException(ex, "GetLocalBranches");
        }
    }

    /// <summary>
    /// Get branch by ID
    /// </summary>
    [HttpGet("Branch/{id}")]
    [ProducesResponseType(typeof(BranchDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(Duration = 300, VaryByQueryKeys = new[] { "id" })]
    public async Task<IActionResult> GetBranchById(Guid id)
    {
        try
        {
            var cacheKey = $"local:branch:{id}";

            var cached = await _cacheService.GetAsync<BranchDto>(cacheKey);
            if (cached != null)
            {
                _logger.LogDebug("📦 Cache HIT: {CacheKey}", cacheKey);
                return Ok(cached);
            }

            var result = await _mediator.Send(new GetLocalBranchByIdQry { Id = id });

            if (result == null)
                return NotFound($"Branch with ID {id} not found");

            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting branch by ID: {Id}", id);
            return HandleException(ex, "GetLocalBranchById", id);
        }
    }

    /// <summary>
    /// Get all local departments (from Core.Module sync)
    /// </summary>
    [HttpGet("Departments")]
    [ProducesResponseType(typeof(List<DepartmentDto>), StatusCodes.Status200OK)]
    [ResponseCache(Duration = 300, VaryByQueryKeys = new[] { "isActive", "branchId" })]
    public async Task<IActionResult> GetDepartments(
        [FromQuery] bool? isActive,
        [FromQuery] Guid? branchId)
    {
        try
        {
            var cacheKey = $"local:departments:active:{isActive ?? false}:branch:{branchId ?? Guid.Empty}";

            var cached = await _cacheService.GetAsync<List<DepartmentDto>>(cacheKey);
            if (cached != null)
            {
                _logger.LogDebug("📦 Cache HIT: {CacheKey}", cacheKey);
                return Ok(cached);
            }

            var result = await _mediator.Send(new GetAllLocalDepartmentsQry
            {
                IsActive = isActive,
                BranchId = branchId
            });

            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting local departments");
            return HandleException(ex, "GetLocalDepartments");
        }
    }

    /// <summary>
    /// Get department by ID
    /// </summary>
    [HttpGet("Department/{id}")]
    [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(Duration = 300, VaryByQueryKeys = new[] { "id" })]
    public async Task<IActionResult> GetDepartmentById(Guid id)
    {
        try
        {
            var cacheKey = $"local:department:{id}";

            var cached = await _cacheService.GetAsync<DepartmentDto>(cacheKey);
            if (cached != null)
            {
                _logger.LogDebug("📦 Cache HIT: {CacheKey}", cacheKey);
                return Ok(cached);
            }

            var result = await _mediator.Send(new GetLocalDepartmentByIdQry { Id = id });

            if (result == null)
                return NotFound($"Department with ID {id} not found");

            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting department by ID: {Id}", id);
            return HandleException(ex, "GetLocalDepartmentById", id);
        }
    }

    /// <summary>
    /// Get all local employees (from HRM.Profile sync) - OPTIMIZED
    /// </summary>
    [HttpGet("Employees")]
    [ProducesResponseType(typeof(List<EmployeeDto>), StatusCodes.Status200OK)]
    [ResponseCache(Duration = 300, VaryByQueryKeys = new[] { "isActive", "departmentId", "position" })]
    public async Task<IActionResult> GetEmployees(
        [FromQuery] bool? isActive,
        [FromQuery] Guid? departmentId,
        [FromQuery] string? position)
    {
        try
        {
            // ✅ Build cache key with all parameters
            var cacheKey = $"local:employees:active:{isActive ?? false}:dept:{departmentId ?? Guid.Empty}:pos:{position ?? "all"}";

            // ✅ Try to get from cache first
            var cached = await _cacheService.GetAsync<List<EmployeeDto>>(cacheKey);
            if (cached != null)
            {
                _logger.LogDebug("📦 Cache HIT: {CacheKey}", cacheKey);
                return Ok(cached);
            }

            _logger.LogDebug("📦 Cache MISS: {CacheKey}", cacheKey);

            // ✅ Get from database
            var result = await _mediator.Send(new GetAllLocalEmployeesQry
            {
                IsActive = isActive,
                DepartmentId = departmentId,
                Position = position
            });

            // ✅ Cache the result for 5 minutes
            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting local employees");
            return HandleException(ex, "GetLocalEmployees");
        }
    }

    /// <summary>
    /// Get employee by ID
    /// </summary>
    [HttpGet("Employee/{id}")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(Duration = 300, VaryByQueryKeys = new[] { "id" })]
    public async Task<IActionResult> GetEmployeeById(Guid id)
    {
        try
        {
            var cacheKey = $"local:employee:{id}";

            var cached = await _cacheService.GetAsync<EmployeeDto>(cacheKey);
            if (cached != null)
            {
                _logger.LogDebug("📦 Cache HIT: {CacheKey}", cacheKey);
                return Ok(cached);
            }

            var result = await _mediator.Send(new GetLocalEmployeeByIdQry { Id = id });

            if (result == null)
                return NotFound($"Employee with ID {id} not found");

            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting employee by ID: {Id}", id);
            return HandleException(ex, "GetLocalEmployeeById", id);
        }
    }

    /// <summary>
    /// Get employees by department - OPTIMIZED
    /// </summary>
    [HttpGet("ByDepartment/{departmentId}")]
    [ProducesResponseType(typeof(List<EmployeeDto>), StatusCodes.Status200OK)]
    [ResponseCache(Duration = 300, VaryByQueryKeys = new[] { "departmentId" })]
    public async Task<IActionResult> GetEmployeesByDepartment(Guid departmentId)
    {
        try
        {
            var cacheKey = $"local:employees:dept:{departmentId}";

            var cached = await _cacheService.GetAsync<List<EmployeeDto>>(cacheKey);
            if (cached != null)
            {
                _logger.LogDebug("📦 Cache HIT: {CacheKey}", cacheKey);
                return Ok(cached);
            }

            var result = await _mediator.Send(new GetAllLocalEmployeesQry
            {
                DepartmentId = departmentId
            });

            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting employees by department: {DepartmentId}", departmentId);
            return HandleException(ex, "GetEmployeesByDepartment", departmentId);
        }
    }

    /// <summary>
    /// Get employees by position - OPTIMIZED
    /// </summary>
    [HttpGet("ByPosition/{position}")]
    [ProducesResponseType(typeof(List<EmployeeDto>), StatusCodes.Status200OK)]
    [ResponseCache(Duration = 300, VaryByQueryKeys = new[] { "position" })]
    public async Task<IActionResult> GetEmployeesByPosition(string position)
    {
        try
        {
            var cacheKey = $"local:employees:pos:{position}";

            var cached = await _cacheService.GetAsync<List<EmployeeDto>>(cacheKey);
            if (cached != null)
            {
                _logger.LogDebug("📦 Cache HIT: {CacheKey}", cacheKey);
                return Ok(cached);
            }

            var result = await _mediator.Send(new GetAllLocalEmployeesQry
            {
                Position = position
            });

            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting employees by position: {Position}", position);
            return HandleException(ex, "GetEmployeesByPosition", position);
        }
    }

    /// <summary>
    /// Invalidate all local copy caches (called after sync)
    /// </summary>
    [HttpPost("InvalidateCache")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> InvalidateCache()
    {
        try
        {
            // Remove all local copy cache entries
            await _cacheService.RemoveByPatternAsync("local:*");
            _logger.LogInformation("🗑️ Local copy cache invalidated");

            return Ok(new {
                success = true,
                message = "Cache invalidated successfully",
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error invalidating cache");
            return HandleException(ex, "InvalidateCache");
        }
    }

    /// <summary>
    /// Get cache statistics (for debugging)
    /// </summary>
    [HttpGet("CacheStats")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCacheStats()
    {
        try
        {
            var stats = new
            {
                CacheDurationMinutes = CACHE_DURATION_MINUTES,
                CachePattern = "local:*",
                Endpoints = new[]
                {
                    "Companies",
                    "Branches",
                    "Departments",
                    "Employees",
                    "ByDepartment/{departmentId}",
                    "ByPosition/{position}"
                },
                Timestamp = DateTime.UtcNow
            };

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting cache stats");
            return HandleException(ex, "GetCacheStats");
        }
    }
}