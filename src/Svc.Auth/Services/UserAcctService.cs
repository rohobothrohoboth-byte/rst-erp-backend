using Helpers;
using Common;
using Microsoft.AspNetCore.Identity;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;
using Microsoft.EntityFrameworkCore;
namespace Svc.Auth.Services;

public interface IUserAcctService
{
    Task<string> GetUserRoleDesc(string email, CancellationToken ct);
    Task<AppUser?> GetByEmail(string email, CancellationToken ct);
    Task<OpResult> UpdateUser(string userId, string newEmail, CancellationToken ct);
    Task<OpResult> ActivateUser(string userId, bool isActive, CancellationToken ct);
    Task<OpResult> DeleteAccount(string id, CancellationToken ct);
    Task<OpResult> AssignRole(string userId, string roleId, CancellationToken ct);
    Task<OpResult> RemoveRole(string userId, string roleName, CancellationToken ct);
    Task<OpResult> SoftDeleteAccount(string email, CancellationToken ct);
    Task<OpResult> ChnagePassword(PwdChgDto Dto, CancellationToken ct);
    Task<OpResult> ResetPassword(string userId, string newPassword, CancellationToken ct);



}

public class UserAcctService(UserManager<AppUser> _uManager, RoleManager<AppRole> _rManager) : IUserAcctService
{
    //private readonly UserManager<AppUser> _uManager = uManager;
    //private readonly RoleManager<AppRole> _rManager = rManager;

    public async Task<AppUser?> GetByEmail(string email, CancellationToken ct)
    {
        return await _uManager.FindByEmailAsync(email);
    }

    public async Task<string> GetUserRoleDesc(string email, CancellationToken ct)
    {
        var user = await _uManager.FindByEmailAsync(email);
        if (user == null) { return "Not Available"; }

        var roles = await _uManager.GetRolesAsync(user);
        if (!roles.Any()) { return "Not Available"; }

        var role = await _rManager.FindByNameAsync(roles.First());
        return role?.Desc ?? "Not Available";
    }

    public async Task<OpResult> UpdateUser(string userId, string newEmail, CancellationToken ct)
    {
        var user = await _uManager.FindByIdAsync(userId);
        if (user == null) { return OpResult.Fail("User not found"); }

        var existing = await _uManager.FindByEmailAsync(newEmail);
        if (existing != null && existing.Id != userId) { return OpResult.Fail("Email already in use"); }

        var token = await _uManager.GenerateChangeEmailTokenAsync(user, newEmail);
        var result = await _uManager.ChangeEmailAsync(user, newEmail, token);

        if (!result.Succeeded) { return OpResult.Fail(result.Errors.Select(e => e.Description)); }

        user.UserName = newEmail;
        user.EmailConfirmed = true;
        user.NormalizedEmail = _uManager.NormalizeEmail(newEmail);
        user.NormalizedUserName = _uManager.NormalizeName(newEmail);
        await _uManager.UpdateAsync(user);
        await _uManager.UpdateSecurityStampAsync(user);

        return OpResult.Ok();
    }

    public async Task<OpResult> ActivateUser(string userId, bool isActive, CancellationToken ct)
    {
        var user = await _uManager.FindByIdAsync(userId);
        if (user == null) { return OpResult.Fail("User not found"); }

        user.IsActive = isActive;
        var result = await _uManager.UpdateAsync(user);
        return result.Succeeded ? OpResult.Ok() : OpResult.Fail(result.Errors.Select(e => e.Description));
    }
    public async Task<OpResult> DeleteAccount(string id, CancellationToken ct)
    {
        Console.WriteLine($"=== Deleting account for user: {id} ===");

        // Try to find by EmployeeId first (since that's what you're passing)
        var user = await _uManager.Users
            .FirstOrDefaultAsync(u => u.EmployeeId == Guid.Parse(id), ct);

        if (user == null)
        {
            // If not found by EmployeeId, try by Id
            user = await _uManager.FindByIdAsync(id);
        }

        if (user == null)
        {
            Console.WriteLine($"User not found with ID: {id}");
            return OpResult.Fail($"User not found with ID: {id}");
        }

        Console.WriteLine($"User found: {user.Email}, IsActive: {user.IsActive}");

        try
        {
            // Just deactivate the user
            user.IsActive = false;
            var result = await _uManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                Console.WriteLine("User deactivated successfully");
                return OpResult.Ok();
            }

            Console.WriteLine($"Update failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            return OpResult.Fail(result.Errors.Select(e => e.Description));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return OpResult.Fail($"Error: {ex.Message}");
        }
    }
    public async Task<OpResult> AssignRole(string userId, string roleId, CancellationToken ct)
    {
        var user = await _uManager.FindByIdAsync(userId);
        if (user == null) { return OpResult.Fail("User not found"); }

        var role = await _rManager.FindByIdAsync(roleId);
        if (role == null) { return OpResult.Fail("Role not found"); }

        var result = await _uManager.AddToRoleAsync(user, role.Name!);

        return result.Succeeded ? OpResult.Ok() : OpResult.Fail(result.Errors.Select(e => e.Description));
    }

    public async Task<OpResult> RemoveRole(string userId, string roleName, CancellationToken ct)
    {
        var user = await _uManager.FindByIdAsync(userId);
        if (user == null) { return OpResult.Fail("User not found"); }

        var result = await _uManager.RemoveFromRoleAsync(user, roleName);
        return result.Succeeded ? OpResult.Ok() : OpResult.Fail(result.Errors.Select(e => e.Description));
    }

    public async Task<OpResult> SoftDeleteAccount(string email, CancellationToken ct)
    {
        var user = await _uManager.FindByEmailAsync(email);
        if (user == null) { return OpResult.Fail("User not found"); }

        user.IsActive = false;
        var result = await _uManager.UpdateAsync(user);
        return result.Succeeded ? OpResult.Ok() : OpResult.Fail(result.Errors.Select(e => e.Description));
    }

    public async Task<OpResult> ChnagePassword(PwdChgDto dto, CancellationToken ct)
    {
        var user = await _uManager.FindByIdAsync(dto.Id);
        if (user == null) { return OpResult.Fail("User not found"); }

        var result = await _uManager.ChangePasswordAsync(user, dto.OldPwd, dto.NewPwd);
        return result.Succeeded ? OpResult.Ok() : OpResult.Fail(result.Errors.Select(e => e.Description));
    }

    public async Task<OpResult> ResetPassword(string userId, string newPassword, CancellationToken ct)
    {
        var user = await _uManager.FindByIdAsync(userId);
        if (user == null) { return OpResult.Fail("User not found"); }

        var token = await _uManager.GeneratePasswordResetTokenAsync(user);
        var result = await _uManager.ResetPasswordAsync(user, token, newPassword);

        return result.Succeeded ? OpResult.Ok() : OpResult.Fail(result.Errors.Select(e => e.Description));
    }
}