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
        private readonly RoleManager<IdentityRole> _rm;
        private readonly AuthDbContext _db;
        public RolesController(RoleManager<IdentityRole> rm, AuthDbContext db) { _rm = rm; _db = db; }

        [HttpGet] public async Task<IActionResult> All() => Ok(await _rm.Roles.ToListAsync());

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] string roleName)
        {
            if (await _rm.RoleExistsAsync(roleName)) return Conflict("Exists");
            var res = await _rm.CreateAsync(new IdentityRole(roleName));
            return res.Succeeded ? Ok() : BadRequest(res.Errors);
        }

        [HttpPost("{roleId}/permissions/{permissionId:guid}")]
        public async Task<IActionResult> Assign(string roleId, Guid permissionId)
        {
            var role = await _rm.FindByIdAsync(roleId); if (role == null) return NotFound("role");
            var perm = await _db.Permissions.FindAsync(permissionId); if (perm == null) return NotFound("permission");
            if (await _db.RolePermissions.AnyAsync(rp => rp.RoleId == role.Id && rp.PermissionId == permissionId))
                return Conflict("Already");
            _db.RolePermissions.Add(new RolePermission { RoleId = role.Id, PermissionId = permissionId });
            await _db.SaveChangesAsync(); return Ok();
        }

        [HttpDelete("{roleId}/permissions/{permissionId:guid}")]
        public async Task<IActionResult> Remove(string roleId, Guid permissionId)
        {
            var rp = await _db.RolePermissions.FindAsync(roleId, permissionId);
            if (rp == null) return NotFound();
            _db.RolePermissions.Remove(rp); await _db.SaveChangesAsync(); return NoContent();
        }
    }
}
