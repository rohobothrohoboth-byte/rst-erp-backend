using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RST.Auth.API.DTOs;
using RST.Auth.API.Models;
using RST.Auth.API.Services;

namespace RST.Auth.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AuthController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Username);
            if (user == null) return Unauthorized();

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded) return Unauthorized();

            var tokens = await _tokenService.GenerateTokensAsync(user);
            return Ok(new { accessToken = tokens.AccessToken, refreshToken = tokens.RefreshToken });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(TokenRefreshDto dto)
        {
            var newAccessToken = await _tokenService.RefreshAccessTokenAsync(dto.RefreshToken);
            if (newAccessToken == null) return Unauthorized();

            return Ok(new { accessToken = newAccessToken });
        }

        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke(TokenRevokeDto dto)
        {
            var revoked = await _tokenService.RevokeRefreshTokenAsync(dto.RefreshToken);
            if (!revoked) return NotFound("Token not found or already revoked");

            return NoContent();
        }
    }

    //public record LoginDto(string Username, string Password);
    //public record TokenRefreshDto(string RefreshToken);
    //public record TokenRevokeDto(string RefreshToken);
}
