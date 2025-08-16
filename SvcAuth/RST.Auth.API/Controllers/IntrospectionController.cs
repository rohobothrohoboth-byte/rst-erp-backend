using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RST.Auth.API.Data;

namespace RST.Auth.API.Controllers
{
    [ApiController]
    [Route("api/introspect")]
    [Authorize(Policy = "introspect.access")]
    public class IntrospectionController : ControllerBase
    {
        private readonly AuthDbContext _db;
        public IntrospectionController(AuthDbContext db) => _db = db;

        [HttpGet("access/{jti}")]
        [AllowAnonymous] // allow consumer service to check without a token (or keep protected and use client creds)
        public async Task<IActionResult> IsActive(string jti)
        {
            var revoked = await _db.RevokedAccessTokens.AnyAsync(x => x.Jti == jti);
            return Ok(new { jti, active = !revoked });
        }
    }
}
