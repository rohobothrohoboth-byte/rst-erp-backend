using System.Security.Claims;
using Asp.Versioning;
using Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Svc.Auth.Models.Entities;
using Svc.Auth.Queries;

namespace Svc.Auth.Controllers;

[ApiController]
[Route("api/auth/v{version:apiVersion}/Permission")]
[ApiVersion("1.0")]
public sealed class RoleAdminController : ControllerBase
{
    private const string PermissionClaimType = "erp:permission";
    private readonly RoleManager<AppRole> _roles;
    private readonly IMediator _med;

    public RoleAdminController(RoleManager<AppRole> roles, IMediator med)
    {
        _roles = roles;
        _med = med;
    }

    [HttpPost("AddRole")]
    public async Task<IActionResult> AddRole([FromBody] RoleWriteDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new DomainException("Role name is required.");

        var normalized = dto.Name.Trim();
        if (await _roles.FindByNameAsync(normalized) != null)
            throw new DomainException($"Role [{normalized}] already exists.");

        var role = new AppRole { Name = normalized, Desc = dto.Description?.Trim() ?? string.Empty };
        var result = await _roles.CreateAsync(role);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        return Ok(ApiResponse<object>.Ok(new { Id = role.Id, Role = role.Name, Description = role.Desc }, "Role created successfully."));
    }

    [HttpPut("ModRole/{id}")]
    public async Task<IActionResult> ModRole(string id, [FromBody] RoleWriteDto dto)
    {
        var role = await _roles.FindByIdAsync(id);
        if (role == null)
            throw new DomainException($"ROLE with id [{id}] NOT FOUND.");

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new DomainException("Role name is required.");

        var duplicate = await _roles.FindByNameAsync(dto.Name.Trim());
        if (duplicate != null && duplicate.Id != role.Id)
            throw new DomainException($"Role [{dto.Name.Trim()}] already exists.");

        role.Name = dto.Name.Trim();
        role.Desc = dto.Description?.Trim() ?? string.Empty;
        var result = await _roles.UpdateAsync(role);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        return Ok(ApiResponse<object>.Ok(new { Id = role.Id, Role = role.Name, Description = role.Desc }, "Role updated successfully."));
    }

    [HttpDelete("DelRole/{id}")]
    public async Task<IActionResult> DelRole(string id)
    {
        var role = await _roles.FindByIdAsync(id);
        if (role == null)
            throw new DomainException($"ROLE with id [{id}] NOT FOUND.");
        if (string.Equals(role.Name, "admin", StringComparison.OrdinalIgnoreCase))
            throw new DomainException("The system administrator role cannot be deleted.");

        var result = await _roles.DeleteAsync(role);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        return Ok(ApiResponse<object>.Ok(null, "Role deleted successfully."));
    }

    [HttpGet("GetRolePermissions/{roleId}")]
    public async Task<IActionResult> GetRolePermissions(string roleId)
    {
        var role = await _roles.FindByIdAsync(roleId);
        if (role == null)
            throw new DomainException($"ROLE with id [{roleId}] NOT FOUND.");

        var claims = await _roles.GetClaimsAsync(role);
        var values = claims.Where(c => c.Type == PermissionClaimType).Select(c => c.Value).ToList();

        return Ok(ApiResponse<object>.Ok(new
        {
            RoleId = role.Id,
            RoleName = role.Name,
            Modules = values.Where(v => v.StartsWith("module:")).Select(v => v[7..]).ToList(),
            Menus = values.Where(v => v.StartsWith("menu:")).Select(v => v[5..]).ToList(),
            Apis = values.Where(v => v.StartsWith("api:")).Select(v => v[4..]).ToList()
        }));
    }

    [HttpPost("SaveRolePermissions")]
    public async Task<IActionResult> SaveRolePermissions([FromBody] SaveRolePermissionsDto dto)
    {
        var role = await _roles.FindByIdAsync(dto.RoleId);
        if (role == null)
            throw new DomainException($"ROLE with id [{dto.RoleId}] NOT FOUND.");

        var claims = await _roles.GetClaimsAsync(role);
        foreach (var claim in claims.Where(c => c.Type == PermissionClaimType).ToList())
            await _roles.RemoveClaimAsync(role, claim);

        var values = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var id in dto.ModuleIds ?? []) if (Guid.TryParse(id, out _)) values.Add($"module:{id}");
        foreach (var id in dto.MenuIds ?? []) if (Guid.TryParse(id, out _)) values.Add($"menu:{id}");
        foreach (var id in dto.ApiActionIds ?? []) if (Guid.TryParse(id, out _)) values.Add($"api:{id}");

        foreach (var value in values)
        {
            var result = await _roles.AddClaimAsync(role, new Claim(PermissionClaimType, value));
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));
        }

        return Ok(ApiResponse<object>.Ok(new { RoleId = role.Id, Count = values.Count }, "Role permissions saved successfully."));
    }

    public sealed record RoleWriteDto(string Name, string? Description);
    public sealed record SaveRolePermissionsDto(string RoleId, List<string>? ModuleIds, List<string>? MenuIds, List<string>? ApiActionIds);
}
