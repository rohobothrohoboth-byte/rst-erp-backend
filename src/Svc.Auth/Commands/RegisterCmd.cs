using Common;
using Dapper;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;
using Svc.Auth.Persistence;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Svc.Auth.Commands;

public class RegStep1Cmd : IRequest<RegRes?>
{
    public RegStep1 Reg { get; set; } = default!;
}

public class RegStep2Cmd : IRequest<RegRes?>
{
    public RegStep2 Reg { get; set; } = default!;
}

public class RegStep3Cmd : IRequest<RegRes?>
{
    public RegStep3 Reg { get; set; } = default!;
}

public class RegRes
{
    public Guid? UserId { get; set; }
    public bool IsSuccess { get; set; }
    public string[]? Errors { get; set; }
    public string? Message { get; set; }

    public static RegRes Success(Guid userId, string? message = null) => new RegRes
    {
        IsSuccess = true,
        UserId = userId,
        Message = message ?? "User registered successfully"
    };

    public static RegRes Fail(string error) => new RegRes
    {
        IsSuccess = false,
        Errors = new[] { error }
    };

    public static RegRes Fail(IEnumerable<string> errors) => new RegRes
    {
        IsSuccess = false,
        Errors = errors.ToArray()
    };
}

// ==================== STEP 1 HANDLER ====================
public class RegStep1Handler : IRequestHandler<RegStep1Cmd, RegRes?>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AuthDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RegStep1Handler> _logger;

    public RegStep1Handler(
        UserManager<AppUser> userManager,
        AuthDbContext dbContext,
        IConfiguration configuration,
        ILogger<RegStep1Handler> logger)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<RegRes?> Handle(RegStep1Cmd request, CancellationToken ct)
    {
        _logger.LogInformation("Starting Step1 registration for user: {UserName}", request.Reg.UserName);

        // Validate
        if (string.IsNullOrEmpty(request.Reg.UserName))
            throw new DomainException("UserName is required");

        if (string.IsNullOrEmpty(request.Reg.Email))
            throw new DomainException("Email is required");

        if (string.IsNullOrEmpty(request.Reg.Password))
            throw new DomainException("Password is required");

        if (string.IsNullOrEmpty(request.Reg.ConfirmPassword))
            throw new DomainException("ConfirmPassword is required");

        if (request.Reg.Password != request.Reg.ConfirmPassword)
            throw new DomainException("Password and ConfirmPassword do not match");

        if (string.IsNullOrEmpty(request.Reg.EmployeeId))
            throw new DomainException("EmployeeId is required");

        if (!Guid.TryParse(request.Reg.EmployeeId, out var employeeGuid))
            throw new DomainException($"Invalid EmployeeId format: '{request.Reg.EmployeeId}'");

        if (string.IsNullOrEmpty(request.Reg.RoleId))
            throw new DomainException("RoleId is required");

        if (!Guid.TryParse(request.Reg.RoleId, out var roleGuid))
            throw new DomainException($"Invalid RoleId format: '{request.Reg.RoleId}'");

        // Get employee data FIRST (before any transaction)
        var connectionString = _configuration.GetConnectionString("authMgrCon");
        if (string.IsNullOrEmpty(connectionString))
            throw new DomainException("authMgrCon connection string not configured");

        Guid? branchId = null;
        Guid? departmentId = null;
        Guid? positionId = null;
        string? employeeEmail = null;

        using (var connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync(ct);

            const string empSql = @"
                SELECT
                    e.""Id"" as EmployeeId,
                    e.""PositionId"",
                    e.""DepartmentId"",
                    e.""JobGradeId"",
                    e.""FirstName"",
                    e.""LastName"",
                    e.""Email""
                FROM ""Employees"" e
                WHERE e.""Id"" = @EmployeeId
                AND e.""IsDeleted"" = false";

            var employee = await connection.QueryFirstOrDefaultAsync<dynamic>(
                empSql,
                new { EmployeeId = employeeGuid });

            if (employee == null)
                throw new DomainException($"Employee with ID '{request.Reg.EmployeeId}' not found");

            employeeEmail = employee.Email;

            if (employee.DepartmentId != null)
            {
                try
                {
                    var deptId = Convert.ToGuid(employee.DepartmentId);
                    const string deptSql = @"
                        SELECT d.""Id"" as DepartmentId, d.""BranchId""
                        FROM ""Departments"" d
                        WHERE d.""Id"" = @DepartmentId AND d.""IsDeleted"" = false";

                    var deptInfo = await connection.QueryFirstOrDefaultAsync<dynamic>(deptSql, new { DepartmentId = deptId });
                    if (deptInfo != null)
                    {
                        departmentId = Convert.ToGuid(deptInfo.DepartmentId);
                        branchId = Convert.ToGuid(deptInfo.BranchId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not fetch department info");
                }
            }

            if (employee.PositionId != null)
            {
                try
                {
                    var posId = Convert.ToGuid(employee.PositionId);
                    const string posSql = @"
                        SELECT p.""Id"" as PositionId
                        FROM ""Positions"" p
                        WHERE p.""Id"" = @PositionId AND p.""IsDeleted"" = false";

                    var posInfo = await connection.QueryFirstOrDefaultAsync<dynamic>(posSql, new { PositionId = posId });
                    if (posInfo != null)
                    {
                        positionId = Convert.ToGuid(posInfo.PositionId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not fetch position info");
                }
            }
        }

        // Check if user exists (BEFORE transaction)
        var existingUser = await _userManager.FindByNameAsync(request.Reg.UserName);
        if (existingUser != null)
            throw new DomainException($"User '{request.Reg.UserName}' already exists");

        // Create user
        var user = new AppUser
        {
            UserName = request.Reg.UserName,
            Email = request.Reg.Email ?? employeeEmail,
            EmployeeId = employeeGuid,
            IsActive = true,
            BranchId = branchId,
            DepartmentId = departmentId,
            PositionId = positionId
        };

        var result = await _userManager.CreateAsync(user, request.Reg.Password);
        if (!result.Succeeded)
            throw new DomainException(string.Join(", ", result.Errors.Select(e => e.Description)));

        // ✅ Use execution strategy
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                if (request.Reg.PerModules != null && request.Reg.PerModules.Any())
                {
                    foreach (var moduleId in request.Reg.PerModules)
                    {
                        var userModule = new UserPerModule
                        {
                            Id = Guid.CreateVersion7(),
                            UserId = user.Id,
                            PerModuleId = moduleId,
                            DateAdd = DateTime.UtcNow,
                            IsDeleted = false
                        };
                        await _dbContext.UserPerModule.AddAsync(userModule, ct);
                    }
                    await _dbContext.SaveChangesAsync(ct);
                }

                if (!string.IsNullOrEmpty(request.Reg.RoleId))
                {
                    var userRole = new IdentityUserRole<string>
                    {
                        UserId = user.Id,
                        RoleId = request.Reg.RoleId
                    };
                    await _dbContext.UserRoles.AddAsync(userRole, ct);
                    await _dbContext.SaveChangesAsync(ct);
                }

                await transaction.CommitAsync(ct);

                _logger.LogInformation("User created successfully: {UserName}", request.Reg.UserName);

                return RegRes.Success(Guid.Parse(user.Id), "User registered successfully");
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        });
    }
}

// ==================== STEP 2 HANDLER ====================
public class RegStep2Handler : IRequestHandler<RegStep2Cmd, RegRes?>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AuthDbContext _dbContext;
    private readonly ILogger<RegStep2Handler> _logger;
    private readonly IMemoryCache _cache;

    public RegStep2Handler(
        UserManager<AppUser> userManager,
        AuthDbContext dbContext,
        ILogger<RegStep2Handler> logger,
        IMemoryCache cache)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _logger = logger;
        _cache = cache;
    }

    public async Task<RegRes?> Handle(RegStep2Cmd request, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(request.Reg.UserId))
            throw new DomainException("UserId is required");

        if (!Guid.TryParse(request.Reg.UserId, out var userId))
            throw new DomainException($"Invalid UserId format: '{request.Reg.UserId}'");

        var selPer = request.Reg.PerMenus;
        if (selPer == null || selPer.Count <= 0)
            throw new DomainException("NO MENU Permission selected");

        // ✅ Check user BEFORE transaction
        var user = await _userManager.FindByIdAsync(request.Reg.UserId);
        if (user == null)
            throw new DomainException($"User with ID '{request.Reg.UserId}' not found");

        // ✅ Use execution strategy
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                // ✅ Use AsTracking() for updates
                var existingMenus = await _dbContext.UserPerMenu
                    .AsTracking()
                    .Where(p => p.UserId == request.Reg.UserId && !p.IsDeleted)
                    .ToListAsync(ct);

                // Soft delete existing
                foreach (var existing in existingMenus)
                {
                    existing.IsDeleted = true;
                }
                await _dbContext.SaveChangesAsync(ct);

                // Add new menu permissions
                foreach (var per in selPer)
                {
                    var userMod = new UserPerMenu
                    {
                        Id = Guid.CreateVersion7(),
                        UserId = request.Reg.UserId,
                        PerMenuId = per,
                        DateAdd = DateTime.UtcNow,
                        IsDeleted = false
                    };
                    await _dbContext.UserPerMenu.AddAsync(userMod, ct);
                }

                await _dbContext.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);

                // Invalidate the cached sidebar structure so a freshly-created user
                // sees their assigned menus immediately (no edit-and-save required).
                _cache.Remove($"menu_structure_{request.Reg.UserId}");

                _logger.LogInformation("Menu permissions saved for user: {UserId}", request.Reg.UserId);

                return RegRes.Success(userId, "Menu permissions saved successfully");
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        });
    }
}

// ==================== STEP 3 HANDLER ====================
public class RegStep3Handler : IRequestHandler<RegStep3Cmd, RegRes?>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AuthDbContext _dbContext;
    private readonly ILogger<RegStep3Handler> _logger;
    private readonly IMemoryCache _cache;

    public RegStep3Handler(
        UserManager<AppUser> userManager,
        AuthDbContext dbContext,
        ILogger<RegStep3Handler> logger,
        IMemoryCache cache)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _logger = logger;
        _cache = cache;
    }

    public async Task<RegRes?> Handle(RegStep3Cmd request, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(request.Reg.UserId))
            throw new DomainException("UserId is required");

        if (!Guid.TryParse(request.Reg.UserId, out var userId))
            throw new DomainException($"Invalid UserId format: '{request.Reg.UserId}'");

        var selPer = request.Reg.PerAccess;
        if (selPer == null || selPer.Count <= 0)
            throw new DomainException("NO ACCESS Permission selected");

        // ✅ Check user BEFORE transaction
        var user = await _userManager.FindByIdAsync(request.Reg.UserId);
        if (user == null)
            throw new DomainException($"User with ID '{request.Reg.UserId}' not found");

        // ✅ Use execution strategy
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                // ✅ Use AsTracking() for updates
                var existingApis = await _dbContext.UserPerApi
                    .AsTracking()
                    .Where(p => p.UserId == request.Reg.UserId && !p.IsDeleted)
                    .ToListAsync(ct);

                // Soft delete existing
                foreach (var existing in existingApis)
                {
                    existing.IsDeleted = true;
                }
                await _dbContext.SaveChangesAsync(ct);

                // Add new API permissions
                foreach (var per in selPer)
                {
                    var userMod = new UserPerApi
                    {
                        Id = Guid.CreateVersion7(),
                        UserId = request.Reg.UserId,
                        PerApiId = per,
                        DateAdd = DateTime.UtcNow,
                        IsDeleted = false
                    };
                    await _dbContext.UserPerApi.AddAsync(userMod, ct);
                }

                await _dbContext.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);

                // Invalidate cached sidebar structure so new permissions apply at once.
                _cache.Remove($"menu_structure_{request.Reg.UserId}");

                _logger.LogInformation("API permissions saved for user: {UserId}", request.Reg.UserId);

                return RegRes.Success(userId, "API permissions saved successfully");
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        });
    }
}

// ==================== HELPER EXTENSION ====================
public static class Convert
{
    public static Guid ToGuid(object value)
    {
        if (value == null || value == DBNull.Value)
            return Guid.Empty;

        if (value is Guid guid)
            return guid;

        if (value is string str && Guid.TryParse(str, out var result))
            return result;

        try
        {
            return Guid.Parse(value.ToString()!);
        }
        catch
        {
            return Guid.Empty;
        }
    }
}