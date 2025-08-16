using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RST.Auth.API.Data;
using RST.Auth.API.DTOs;
using RST.Auth.API.Models;
using RST.Auth.API.Services;

namespace RST.Auth.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _um;
        private readonly SignInManager<ApplicationUser> _sm;
        private readonly ITokenService _tokens;
        private readonly AuthDbContext _db;

        public AuthController(UserManager<ApplicationUser> um, SignInManager<ApplicationUser> sm, ITokenService tokens, AuthDbContext db)
        { _um = um; _sm = sm; _tokens = tokens; _db = db; }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _um.FindByNameAsync(dto.Username);
            if (user == null) return Unauthorized();

            var ok = await _sm.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!ok.Succeeded) return Unauthorized();

            var (access, refresh, jti) = await _tokens.GenerateTokensAsync(user);
            return Ok(new { accessToken = access, refreshToken = refresh, jti, userId = user.Id, user = user.UserName });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(TokenDto dto)
        {
            var newAccess = await _tokens.RefreshAccessTokenAsync(dto.RefreshToken);
            if (newAccess is null) return Unauthorized();
            return Ok(new { accessToken = newAccess });
        }

        [HttpPost("revoke-refresh")]
        public async Task<IActionResult> RevokeRefresh(TokenDto dto)
        {
            var ok = await _tokens.RevokeRefreshTokenAsync(dto.RefreshToken);
            return ok ? NoContent() : NotFound();
        }

        [HttpPost("revoke-access")]
        public async Task<IActionResult> RevokeAccess(RevokeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Jti)) return BadRequest();
            if (!await _db.RevokedAccessTokens.AnyAsync(x => x.Jti == dto.Jti))
                _db.RevokedAccessTokens.Add(new RevokedAccessToken { Jti = dto.Jti, UserId = "system" });
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
