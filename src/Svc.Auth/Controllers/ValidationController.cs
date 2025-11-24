using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Svc.Auth.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ValidationController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly ILogger<ValidationController> _logger;

    public ValidationController(ITokenService tokenService, ILogger<ValidationController> logger)
    {
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _logger = logger;
    }

    /// <summary>
    /// Validate a JWT access token for external microservices
    /// </summary>
    [HttpPost("validate-token")]
    [AllowAnonymous]
    public IActionResult ValidateToken([FromBody] TokenValidationRequest dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Token))
            return BadRequest(new { valid = false, error = "Token is required." });

        try
        {
            var principal = _tokenService.ValidateAccessToken(dto.Token);

            if (principal == null)
            {
                return Unauthorized(new
                {
                    valid = false,
                    error = "Invalid token"
                });
            }

            return Ok(new TokenValidationResponse
            {
                Valid = true,
                UserId = principal.FindFirst("sub")?.Value,
                Username = principal.FindFirst("username")?.Value,
                Permissions = principal.FindAll("permission")?.Select(c => c.Value).ToList()
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Token validation failed: {msg}", ex.Message);

            return Unauthorized(new
            {
                valid = false,
                error = "Token validation failed"
            });
        }
    }
}