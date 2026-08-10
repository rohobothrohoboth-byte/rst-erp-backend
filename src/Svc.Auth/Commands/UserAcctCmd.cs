using Dapper;
using Helpers;
using Common;
using MediatR;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;
using Svc.Auth.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;  // ADD THIS for UserManager
using Microsoft.Extensions.Caching.Memory;

namespace Svc.Auth.Commands;

public class PwdChangeCmd : IRequest<OpResult?> { public PwdChgDto Dto { get; set; } = default!; }
public class UserDelCmd : IRequest<OpResult?> { public string Id { get; set; } = default!; }

public class UpdateUserPermissionsCmd : IRequest<bool>
{
    public string UserId { get; set; } = string.Empty;
    public List<Guid>? ModuleIds { get; set; }
    public List<Guid>? MenuIds { get; set; }
    public List<Guid>? ApiActionIds { get; set; }
}
// Add these classes:

public class UpdateUserBranchCmd : IRequest<OpResult>
{
    public string UserId { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
}

public class UpdateUserDepartmentCmd : IRequest<OpResult>
{
    public string UserId { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }
}

public class UpdateUserPositionCmd : IRequest<OpResult>
{
    public string UserId { get; set; } = string.Empty;
    public Guid PositionId { get; set; }
}

// Handlers:
public class UpdateUserBranchHandler : IRequestHandler<UpdateUserBranchCmd, OpResult>
{
    private readonly UserManager<AppUser> _userManager;

    public UpdateUserBranchHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<OpResult> Handle(UpdateUserBranchCmd request, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
            return OpResult.Fail("User not found");

        user.BranchId = request.BranchId;
        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded ? OpResult.Ok() : OpResult.Fail("Failed to update branch");
    }
}

// Similar handlers for UpdateUserDepartmentCmd and UpdateUserPositionCmd
public class PwdChangeHandler(IUnitOfWork _uow, IUserAcctService _iAccountService) : IRequestHandler<PwdChangeCmd, OpResult?>
{
    public async Task<OpResult?> Handle(PwdChangeCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var res = await _iAccountService.ChnagePassword(request.Dto, ct);
            if (!res.IsSuccess) { throw new DomainException("UNABLE to CHANGE User's Password."); }
            await _uow.Commit(ct);
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class UserDelHandler(IUnitOfWork _uow, IUserAcctService _iAccountService) : IRequestHandler<UserDelCmd, OpResult?>
{
    public async Task<OpResult?> Handle(UserDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var res = await _iAccountService.DeleteAccount(request.Id, ct);
            if (!res.IsSuccess) { throw new DomainException("UNABLE to DELETE selected USER'S account."); }
            await _uow.Commit(ct);
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class UpdateUserPermissionsHandler : IRequestHandler<UpdateUserPermissionsCmd, bool>
{
    private readonly IDapperHelper _dapper;
    private readonly IUnitOfWork _uow;
    private readonly IMemoryCache _cache;

    public UpdateUserPermissionsHandler(IDapperHelper dapper, IUnitOfWork uow, IMemoryCache cache)
    {
        _dapper = dapper;
        _uow = uow;
        _cache = cache;
    }

    public async Task<bool> Handle(UpdateUserPermissionsCmd request, CancellationToken cancellationToken)
    {
        // Start transaction
        await _uow.Begin(cancellationToken);

        try
        {
            Console.WriteLine($"=== UPDATE PERMISSIONS ===");
            Console.WriteLine($"UserId: {request.UserId}");
            Console.WriteLine($"ModuleIds: {(request.ModuleIds == null ? "NULL" : $"[{string.Join(", ", request.ModuleIds)}]")}");
            Console.WriteLine($"MenuIds: {(request.MenuIds == null ? "NULL" : $"[{string.Join(", ", request.MenuIds)}]")}");
            Console.WriteLine($"ApiActionIds: {(request.ApiActionIds == null ? "NULL" : $"[{string.Join(", ", request.ApiActionIds)}]")}");

            // ? Get the transaction from UoW
            var transaction = _uow.Transaction; // Use the property directly

            // ============================================================
            // 1. Handle Module Permissions
            // ============================================================
            if (request.ModuleIds != null)
            {
                Console.WriteLine("Processing ModuleIds...");

                // Delete ALL existing modules
                const string deleteModulesSql = @"
                    DELETE FROM ""UserPerModule""
                    WHERE ""UserId"" = @UserId";

                await _dapper.ExecuteAsync(deleteModulesSql, new { UserId = request.UserId }, cancellationToken);

                // Insert new modules (if any)
                if (request.ModuleIds.Any())
                {
                    const string insertModuleSql = @"
                        INSERT INTO ""UserPerModule"" (""Id"", ""UserId"", ""PerModuleId"", ""DateAdd"", ""IsDeleted"")
                        VALUES (@Id, @UserId, @ModuleId, @DateAdd, false)";

                    foreach (var moduleId in request.ModuleIds)
                    {
                        await _dapper.ExecuteAsync(insertModuleSql, new
                        {
                            Id = Guid.NewGuid(),
                            UserId = request.UserId,
                            ModuleId = moduleId,
                            DateAdd = DateTime.UtcNow
                        }, cancellationToken);
                    }
                }
                else
                {
                    Console.WriteLine("ModuleIds is empty - All modules removed");
                }
            }
            else
            {
                Console.WriteLine("ModuleIds is NULL - Preserving existing modules");
            }

            // ============================================================
            // 2. Handle Menu Permissions
            // ============================================================
            if (request.MenuIds != null)
            {
                Console.WriteLine("Processing MenuIds...");

                const string deleteMenusSql = @"
                    DELETE FROM ""UserPerMenu""
                    WHERE ""UserId"" = @UserId";

                await _dapper.ExecuteAsync(deleteMenusSql, new { UserId = request.UserId }, cancellationToken);

                if (request.MenuIds.Any())
                {
                    const string insertMenuSql = @"
                        INSERT INTO ""UserPerMenu"" (""Id"", ""UserId"", ""PerMenuId"", ""DateAdd"", ""IsDeleted"")
                        VALUES (@Id, @UserId, @MenuId, @DateAdd, false)";

                    foreach (var menuId in request.MenuIds)
                    {
                        await _dapper.ExecuteAsync(insertMenuSql, new
                        {
                            Id = Guid.NewGuid(),
                            UserId = request.UserId,
                            MenuId = menuId,
                            DateAdd = DateTime.UtcNow
                        }, cancellationToken);
                    }
                }
                else
                {
                    Console.WriteLine("MenuIds is empty - All menus removed");
                }
            }
            else
            {
                Console.WriteLine("MenuIds is NULL - Preserving existing menus");
            }

            // ============================================================
            // 3. Handle API Permissions
            // ============================================================
            if (request.ApiActionIds != null)
            {
                Console.WriteLine("Processing ApiActionIds...");

                const string deleteApiSql = @"
                    DELETE FROM ""UserPerApi""
                    WHERE ""UserId"" = @UserId";

                await _dapper.ExecuteAsync(deleteApiSql, new { UserId = request.UserId }, cancellationToken);

                if (request.ApiActionIds.Any())
                {
                    const string insertApiSql = @"
                        INSERT INTO ""UserPerApi"" (""Id"", ""UserId"", ""PerApiId"", ""DateAdd"", ""IsDeleted"")
                        VALUES (@Id, @UserId, @ApiId, @DateAdd, false)";

                    foreach (var apiId in request.ApiActionIds)
                    {
                        await _dapper.ExecuteAsync(insertApiSql, new
                        {
                            Id = Guid.NewGuid(),
                            UserId = request.UserId,
                            ApiId = apiId,
                            DateAdd = DateTime.UtcNow
                        }, cancellationToken);
                    }
                }
                else
                {
                    Console.WriteLine("ApiActionIds is empty - All APIs removed");
                }
            }
            else
            {
                Console.WriteLine("ApiActionIds is NULL - Preserving existing APIs");
            }

            // ? COMMIT: All operations succeeded
            await _uow.Commit(cancellationToken);

            // Invalidate the cached sidebar structure so the change is reflected
            // immediately (GetUserMenuStructureHandler caches by AppUser.Id).
            _cache.Remove($"menu_structure_{request.UserId}");

            Console.WriteLine("=== PERMISSIONS UPDATED SUCCESSFULLY ===");
            return true;
        }
        catch (Exception ex)
        {
            // ? ROLLBACK: Any error ? revert ALL changes
            await _uow.Rollback(cancellationToken);

            Console.WriteLine($"ERROR: Failed to save user permissions: {ex.Message}");
            throw new DomainException($"Failed to save user permissions: {ex.Message}");
        }
    }
}
// ==================== NEW COMMANDS ====================

// Reactivate Account Command
public class ReactivateAccountCmd : IRequest<OpResult?>
{
    public string UserId { get; set; } = string.Empty;
}

public class ReactivateAccountHandler : IRequestHandler<ReactivateAccountCmd, OpResult?>
{
    private readonly UserManager<AppUser> _userManager;

    public ReactivateAccountHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<OpResult?> Handle(ReactivateAccountCmd request, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
            return OpResult.Fail("User not found");

        user.IsActive = true;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            return OpResult.Fail(result.Errors.Select(e => e.Description));

        return OpResult.Ok();
    }
}

/// Reset Password Command - UPDATED without token generation
 public class ResetPasswordCmd : IRequest<OpResult?>
 {
     public string UserId { get; set; } = string.Empty;
     public string NewPassword { get; set; } = string.Empty;
 }

 public class ResetPasswordHandler : IRequestHandler<ResetPasswordCmd, OpResult?>
 {
     private readonly UserManager<AppUser> _userManager;
     private readonly IPasswordHasher<AppUser> _passwordHasher;

     public ResetPasswordHandler(UserManager<AppUser> userManager, IPasswordHasher<AppUser> passwordHasher)
     {
         _userManager = userManager;
         _passwordHasher = passwordHasher;
     }

     public async Task<OpResult?> Handle(ResetPasswordCmd request, CancellationToken ct)
     {
         try
         {
             var user = await _userManager.FindByIdAsync(request.UserId);
             if (user == null)
                 return OpResult.Fail("User not found");

             // Directly update the password hash (bypass token generation)
             user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);

             // Update security stamp to invalidate existing tokens
             user.SecurityStamp = Guid.NewGuid().ToString();

             var result = await _userManager.UpdateAsync(user);

             if (!result.Succeeded)
                 return OpResult.Fail(result.Errors.Select(e => e.Description));

             return OpResult.Ok();
         }
         catch (Exception ex)
         {
             return OpResult.Fail($"Error resetting password: {ex.Message}");
         }
     }
 }

// Hard Delete Account Command (optional - for permanent deletion)
public class HardDeleteAccountCmd : IRequest<OpResult?>
{
    public string UserId { get; set; } = string.Empty;
}

public class HardDeleteAccountHandler : IRequestHandler<HardDeleteAccountCmd, OpResult?>
{
    private readonly UserManager<AppUser> _userManager;

    public HardDeleteAccountHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<OpResult?> Handle(HardDeleteAccountCmd request, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
            return OpResult.Fail("User not found");

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
            return OpResult.Fail(result.Errors.Select(e => e.Description));

        return OpResult.Ok();
    }
}
