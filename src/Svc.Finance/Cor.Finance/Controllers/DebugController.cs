// Controllers/DebugController.cs
// Controllers/AccountController.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Cor.Finance.Commands;
using Shared.Helpers.Services;

namespace Cor.Finance.Controllers;
[ApiController]
[Route("api/finance/v1.0/Debug")]
[Authorize]
public class DebugController : ControllerBase
{
    [HttpGet("claims")]
    public IActionResult GetClaims()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();

        return Ok(new
        {
            IsAuthenticated = User.Identity?.IsAuthenticated,
            IdentityName = User.Identity?.Name,
            Claims = claims
        });
    }
}