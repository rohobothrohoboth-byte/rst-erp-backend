using Asp.Versioning;
using MediatR;
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
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var response = await mediator.Send(new LoginCmd { Login = dto });
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("RefreshToken")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
    {
        var result = await mediator.Send(new RefreshTokenCmd { Input = dto });
        return Ok(result);
    }

    
}