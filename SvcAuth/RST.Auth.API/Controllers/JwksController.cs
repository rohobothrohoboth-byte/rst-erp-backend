using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RST.Auth.API.Services;

namespace RST.Auth.API.Controllers
{
    [ApiController]
    public class JwksController : ControllerBase
    {
        private readonly IKeyStore _keys;
        public JwksController(IKeyStore keys) => _keys = keys;

        [HttpGet("/.well-known/jwks.json")]
        [AllowAnonymous]
        public IActionResult Get()
        {
            var keys = _keys.GetAllPublicKeys().Select(k =>
            {
                var jwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(k.Key);
                jwk.Kid = k.Kid; jwk.Use = "sig"; jwk.Alg = "RS256"; return jwk;
            });
            return Ok(new { keys });
        }
    }
}
