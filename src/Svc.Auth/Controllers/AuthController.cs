using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Svc.Auth.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Register a new user. Returns access + refresh tokens and basic user info.
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (dto == null) return BadRequest("Body required.");

        try
        {
            var cmd = new RegisterUserCommand(dto);
            var result = await _mediator.Send(cmd);
            return Ok(result);
        }
        catch (Domain.DomainException dex)
        {
            // Domain exception mapping to proper status code
            return StatusCode(dex.StatusCode, new { error = dex.Message });
        }
        catch (Exception ex)
        {
            // Generic errors - in production use ProblemDetails / logging
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Login with username/email and password. Returns access + refresh tokens.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (dto == null) return BadRequest("Body required.");

        try
        {
            var cmd = new LoginCommand(dto);
            var result = await _mediator.Send(cmd);
            return Ok(result);
        }
        catch (Domain.DomainException dex)
        {
            return StatusCode(dex.StatusCode, new { error = dex.Message });
        }
        catch (Exception ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Refresh an access token using a refresh token. Returns rotated refresh token (recommended).
    /// </summary>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshDto dto)
    {
        if (dto == null) return BadRequest("Body required.");

        try
        {
            var cmd = new RefreshTokenCommand(dto);
            var result = await _mediator.Send(cmd);
            return Ok(result);
        }
        catch (Domain.DomainException dex)
        {
            return StatusCode(dex.StatusCode, new { error = dex.Message });
        }
        catch (Exception ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Revoke a refresh token (user must be authenticated).
    /// </summary>
    [Authorize]
    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke([FromBody] RefreshDto dto)
    {
        if (dto == null) return BadRequest("Body required.");

        try
        {
            var cmd = new RevokeRefreshTokenCommand(dto);
            await _mediator.Send(cmd);
            return Ok(new { revoked = true });
        }
        catch (Domain.DomainException dex)
        {
            return StatusCode(dex.StatusCode, new { error = dex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}