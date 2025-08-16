using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RST.Auth.API.Models;

namespace RST.Auth.API.Controllers
{
    [ApiController]
    [Route("api/users/{userId}/roles")]
    [Authorize(Policy = "roles.manage")]
    public class UserRolesController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _um;
        public UserRolesController(UserManager<ApplicationUser> um) => _um = um;

        [HttpGet]
        public async Task<IActionResult> Get(string userId)
        {
            var u = await _um.FindByIdAsync(userId); if (u == null) return NotFound();
            return Ok(await _um.GetRolesAsync(u));
        }

        [HttpPost("{roleName}")]
        public async Task<IActionResult> Add(string userId, string roleName)
        {
            var u = await _um.FindByIdAsync(userId); if (u == null) return NotFound();
            if (await _um.IsInRoleAsync(u, roleName)) return Conflict("Already");
            var res = await _um.AddToRoleAsync(u, roleName);
            return res.Succeeded ? Ok() : BadRequest(res.Errors);
        }

        [HttpDelete("{roleName}")]
        public async Task<IActionResult> Remove(string userId, string roleName)
        {
            var u = await _um.FindByIdAsync(userId); if (u == null) return NotFound();
            if (!await _um.IsInRoleAsync(u, roleName)) return Conflict("NotInRole");
            var res = await _um.RemoveFromRoleAsync(u, roleName);
            return res.Succeeded ? NoContent() : BadRequest(res.Errors);
        }
    }
}
