using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Svc.Auth.Commands;
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
        var result = await mediator.Send(new LoginCmd { Login = dto });
        return Ok(result);
    }

    //[HttpPost("refresh")]
    //public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
    //{
    //    var result = await mediator.Send(new RefreshTokenCommand { RefreshToken = dto.RefreshToken });
    //    return Ok(result);
    //}

    //[HttpPost("revoke")]
    //[Authorize]  // Requires valid token
    //public async Task<IActionResult> Revoke([FromBody] RevokeTokenDto dto)
    //{
    //    await mediator.Send(new RevokeTokenCommand { Token = dto.Token });
    //    return Ok();
    //}
}