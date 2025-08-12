using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RST.Auth.API.Data;
using RST.Auth.API.Models;

namespace RST.Auth.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "roles.manage")]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AuthDbContext _db;

        public RolesController(RoleManager<IdentityRole> roleManager, AuthDbContext db)
        {
            _roleManager = roleManager;
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return Ok(roles);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] IdentityRole role)
        {
            if (await _roleManager.RoleExistsAsync(role.Name))
                return Conflict("Role already exists.");

            var result = await _roleManager.CreateAsync(new IdentityRole(role.Name));
            if (result.Succeeded)
                return Ok();
            else
                return BadRequest(result.Errors);
        }

        [HttpPost("{roleId}/permissions/{permissionId:guid}")]
        public async Task<IActionResult> AssignPermission(string roleId, Guid permissionId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null) return NotFound("Role not found.");

            var permission = await _db.Permissions.FindAsync(permissionId);
            if (permission == null) return NotFound("Permission not found.");

            if (await _db.RolePermissions.AnyAsync(rp => rp.RoleId == role.Id && rp.PermissionId == permissionId))
                return Conflict("Permission already assigned.");

            _db.RolePermissions.Add(new RolePermission { RoleId = role.Id, PermissionId = permissionId });
            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{roleId}/permissions/{permissionId:guid}")]
        public async Task<IActionResult> RemovePermission(string roleId, Guid permissionId)
        {
            var rp = await _db.RolePermissions.FindAsync(roleId, permissionId);
            if (rp == null) return NotFound();

            _db.RolePermissions.Remove(rp);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
