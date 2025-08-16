using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RST.Auth.API.Services;

namespace RST.Auth.API.Controllers
{
    [ApiController]
    [Route("api/keys")]
    [Authorize(Policy = "keys.rotate")]
    public class KeysController : ControllerBase
    {
        private readonly IKeyStore _store;
        public KeysController(IKeyStore store) => _store = store;

        [HttpPost("rotate")]
        public IActionResult Rotate()
        {
            var (_, kid) = _store.Rotate();
            return Ok(new { activeKid = kid });
        }
    }
}
