using Asp.Versioning;
using Common;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Svc.Auth.Commands;
using Svc.Auth.Models.Dtos;
using System.Security.Claims;

namespace Svc.Auth.Controllers;

/// <summary>
/// USER management end points
/// </summary>

[Authorize]
[ApiController]
[Route("api/auth/v{version:apiVersion}/User")]
[ApiVersion("1.0")]
public class UserController(IMediator med) : ControllerBase
{
    [HttpDelete("DelUserAcct/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        var res = await med.Send(new UserDelCmd { Id = id });
        if (!res.IsSuccess) { throw new DomainException("UNABLE to DELETE selected User's Account."); }
        return Ok(ApiResponse<object>.Ok(res, "Selected USER'S ACCOUNT successfully DELETED."));
    }

    [HttpPut("ChangePwd")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePwd([FromBody] PwdChgDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var userId = User.FindFirstValue(AuthCons.UserId);
        if (userId == null || userId.Length <= 0) { throw new UnauthorizedException("AUTHORIZATION REQUIRED to gain access."); }

        dto.Id = userId;
        var res = await med.Send(new PwdChangeCmd { Dto = dto });
        if (!res.IsSuccess) { throw new DomainException("UNABLE to CHANGE User's Password."); }
        return Ok(ApiResponse<string>.Ok(null!, "User's Password successfully CHANGED."));
    }


}