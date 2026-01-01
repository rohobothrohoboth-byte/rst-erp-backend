using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Leave.API.Controllers;

/// <summary>
/// End point to get the list of names and Ids of HRM.Leave entities
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/leave/v{version:apiVersion}/Names")]
[ApiVersion("1.0")]
public class NameListController(IMediator med) : ControllerBase
{
    //[HttpGet("AllBenefitSetName")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //public async Task<IActionResult> AllBenefitSetName()
    //{
    //    var res = await med.Send(new BenefitSetNameAllQry());
    //    return Ok(res);
    //}
}