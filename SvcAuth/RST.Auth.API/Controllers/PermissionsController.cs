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
        public PermissionsController(AuthDbContext db) => _db = db;

        [HttpGet] public async Task<IActionResult> GetAll() => Ok(await _db.Permissions.ToListAsync());

        [HttpPost]
        public async Task<IActionResult> Create(Permission p)
        {
            if (await _db.Permissions.AnyAsync(x => x.Name == p.Name)) return Conflict("Exists");
            _db.Permissions.Add(p); await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = p.Id }, p);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
            => (await _db.Permissions.FindAsync(id)) is { } p ? Ok(p) : NotFound();

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var p = await _db.Permissions.FindAsync(id);
            if (p == null) return NotFound();
            _db.Remove(p); await _db.SaveChangesAsync(); return NoContent();
        }
    }
}
