using System.Security.Claims;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Svc.Auth.Commands;
using Svc.Auth.Helpers;
using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Controllers;

[ApiController]
[Route("api/auth/v{version:apiVersion}/")]
[ApiVersion("1.0")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var response = await mediator.Send(new LoginCmd { Login = dto });
        return Ok(ApiResponse<object>.Ok(response));
    }

    [Authorize]
    [HttpPost("RefreshToken")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
    {
        var empId = User.FindFirstValue("userId");
        if (empId is { Length: <= 0 })
        {
            throw new UnauthorizedException("AUTHORIZATION REQUIRED to gain access.");
        }
        var result = await mediator.Send(new RefreshTokenCmd { Input = dto, UserId = empId! });
        return Ok(result);
    }


}