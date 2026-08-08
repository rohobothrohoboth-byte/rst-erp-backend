using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Svc.Auth.Commands;
using Svc.Auth.Models.Dtos;
using System.Security.Claims;
using Svc.Auth.Services;
namespace Svc.Auth.Controllers;

/// <summary>
/// AUTHORIZATION management end points
/// </summary>
[ApiController]
[Route("api/auth/v{version:apiVersion}/")]
[ApiVersion("1.0")]
public class AuthController : ControllerBase
{
    private readonly IMediator _med;



     private readonly ISetupService _setupService;

        public AuthController(IMediator med, ISetupService setupService)
        {
            _med = med;
            _setupService = setupService;
        }

    [AllowAnonymous]
    [HttpPost("Login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var response = await _med.Send(new LoginCmd { Login = dto });
        return Ok(ApiResponse<object>.Ok(response));
    }


    [Authorize]
    [HttpPost("RefreshToken")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
    {
        var empId = User.FindFirstValue("userId");
        if (string.IsNullOrEmpty(empId))
            throw new UnauthorizedException("AUTHORIZATION REQUIRED to gain access.");

        var result = await _med.Send(new RefreshTokenCmd { Input = dto, UserId = empId });
        return Ok(ApiResponse<object>.Ok(result));
    }

    [Authorize]
    [HttpPost("Logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userId))
        {
            await _med.Send(new LogoutCmd { UserId = userId });
        }
        return Ok(ApiResponse<object>.Ok(null, "Logged out successfully."));
    }

     [HttpGet("status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSetupStatus()
        {
            var status = await _setupService.GetSetupStatusAsync();
            return Ok(ApiResponse<object>.Ok(status));
        }
}