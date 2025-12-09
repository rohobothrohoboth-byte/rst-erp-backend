using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Svc.Auth.Commands;
using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Controllers;

[ApiController]
[Route("api/auth/v{version:apiVersion}/Register")]
[ApiVersion("1.0")]
public class RegistrationController(IMediator mediator) : ControllerBase
{
    [HttpPost("Step1")]
    public async Task<IActionResult> Step1([FromBody] LoginDto dto)
    {
        var result = await mediator.Send(new LoginCmd { Login = dto });
        return Ok(result);
    }

    [HttpPost("Step2")]
    public async Task<IActionResult> Step2([FromBody] LoginDto dto)
    {
        var result = await mediator.Send(new LoginCmd { Login = dto });
        return Ok(result);
    }
    
    [HttpPost("Step3")]
    public async Task<IActionResult> Step3([FromBody] LoginDto dto)
    {
        var result = await mediator.Send(new LoginCmd { Login = dto });
        return Ok(result);
    }
}