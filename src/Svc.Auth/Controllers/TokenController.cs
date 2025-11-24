using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Svc.Auth.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TokenController : ControllerBase
{
    private readonly IMediator _mediator;

    public TokenController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Generate a new access token for an authenticated user (manual refresh alternative)
    /// </summary>
    [Authorize]
    [HttpPost("generate")]
    public async Task<IActionResult> Generate()
    {
        var userId = User?.FindFirst("sub")?.Value;

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized("Invalid user context.");

        var dto = new RefreshDto
        {
            UserId = Guid.Parse(userId),
            RefreshToken = null // in this endpoint we rely entirely on the authenticated identity
        };

        var cmd = new RefreshTokenCommand(dto);

        try
        {
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
    /// Revoke all refresh tokens for the current user
    /// </summary>
    [Authorize]
    [HttpPost("revoke-all")]
    public async Task<IActionResult> RevokeAll()
    {
        var userId = User?.FindFirst("sub")?.Value;

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized("Invalid user context.");

        var dto = new RefreshDto
        {
            UserId = Guid.Parse(userId)
        };

        var cmd = new RevokeRefreshTokenCommand(dto);

        try
        {
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