using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Profile.App.Helpers;
using Profile.App.Queries;

namespace Profile.API.Controllers;

/// <summary>
/// End point to get the list of names and Ids of HRM.Profile entities
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/profile/v{version:apiVersion}/Names")]
[ApiVersion("1.0")]
public class NameListController(IMediator med) : ControllerBase
{
    [HttpGet("AllAddressName")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllAddressName()
    {
        var res = await med.Send(new AddressNameAllQry());
        return Ok(ApiResponse<object>.Ok(res));
    }

    [HttpGet("GetAddressName/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAddressName(Guid id)
    {
        var res = await med.Send(new AddressNameByIdQry { Id = id });
        if (res == null) { throw new DomainException($"ADDRESS with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(res));
    }




}
