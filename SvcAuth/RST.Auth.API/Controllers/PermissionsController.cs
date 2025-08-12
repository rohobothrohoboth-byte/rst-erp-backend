using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RST.Auth.API.Data;
using RST.Auth.API.Models;

namespace RST.Auth.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "permissions.manage")]
    public class PermissionsController : ControllerBase
    {
        private readonly AuthDbContext _db;

        public PermissionsController(AuthDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var permissions = await _db.Permissions.ToListAsync();
            return Ok(permissions);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Permission permission)
        {
            if (await _db.Permissions.AnyAsync(p => p.Name == permission.Name))
                return Conflict("Permission with that name already exists.");

            _db.Permissions.Add(permission);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = permission.Id }, permission);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var perm = await _db.Permissions.FindAsync(id);
            if (perm == null) return NotFound();
            return Ok(perm);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var perm = await _db.Permissions.FindAsync(id);
            if (perm == null) return NotFound();

            _db.Permissions.Remove(perm);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
