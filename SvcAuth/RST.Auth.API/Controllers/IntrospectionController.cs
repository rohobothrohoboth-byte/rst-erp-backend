using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RST.Auth.API.Data;
using RST.Auth.API.Models;

namespace RST.Auth.API.Controllers
{
    [ApiController]
    [Route("api/introspect")]
    [Authorize(Policy = "introspect.access")]
    public class IntrospectionController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthDbContext _db;

        public IntrospectionController(UserManager<ApplicationUser> userManager, AuthDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [HttpGet("{userId}/{permission}")]
        public async Task<IActionResult> CheckPermission(string userId, string permission)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var roleIds = _db.Roles.Where(r => roles.Contains(r.Name)).Select(r => r.Id).ToList();

            var hasPermission = await _db.RolePermissions
                .AnyAsync(rp => roleIds.Contains(rp.RoleId) && rp.Permission.Name == permission);

            return Ok(new { userId, permission, authorized = hasPermission });
        }
    }
}
