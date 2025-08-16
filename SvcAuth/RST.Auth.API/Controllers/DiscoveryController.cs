using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RST.Auth.API.Controllers
{
    [ApiController]
    public class DiscoveryController : ControllerBase
    {
        private readonly IConfiguration _cfg;
        public DiscoveryController(IConfiguration cfg) => _cfg = cfg;

        [HttpGet("/.well-known/openid-configuration")]
        [AllowAnonymous]
        public IActionResult Get()
        {
            var issuer = _cfg["Jwt:Issuer"] ?? $"{Request.Scheme}://{Request.Host}";
            return Ok(new
            {
                issuer,
                jwks_uri = $"{issuer}/.well-known/jwks.json",
                token_endpoint = $"{issuer}/api/auth/login",
                introspection_endpoint = $"{issuer}/api/introspect/access"
            });
        }
    }
}
