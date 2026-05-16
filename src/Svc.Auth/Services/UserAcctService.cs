using Helpers;
using Microsoft.AspNetCore.Identity;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;

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
        var errors = new List<string>();
        var user = await _uManager.FindByIdAsync(id);
        if (user == null) { return OpResult.Fail("User not found"); }

        var roles = await _uManager.GetRolesAsync(user);
        if (roles.Any())
        {
            var res = await _uManager.RemoveFromRolesAsync(user, roles);
            if (!res.Succeeded) { errors.AddRange(res.Errors.Select(e => $"Role: {e.Description}")); }
        }

        var claims = await _uManager.GetClaimsAsync(user);
        if (claims.Any())
        {
            var res = await _uManager.RemoveClaimsAsync(user, claims);
            if (!res.Succeeded) { errors.AddRange(res.Errors.Select(e => $"Claim: {e.Description}")); }
        }

        var logins = await _uManager.GetLoginsAsync(user);
        foreach (var login in logins)
        {
            var res = await _uManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
            if (!res.Succeeded) { errors.AddRange(res.Errors.Select(e => $"Login: {e.Description}")); }
        }

        var providers = new[] { "Default", "Email", "Phone" };
        var tokens = new[] { "RefreshToken", "AccessToken" };

        foreach (var provider in providers)
        {
            foreach (var token in tokens)
            {
                await _uManager.RemoveAuthenticationTokenAsync(user, provider, token);
            }
        }

        var deleteResult = await _uManager.DeleteAsync(user);
        if (!deleteResult.Succeeded) { errors.AddRange(deleteResult.Errors.Select(e => $"Delete: {e.Description}")); }

        return errors.Any() ? OpResult.Fail(errors) : OpResult.Ok();
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