using Asp.Versioning;
using Cor.HRMM.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cor.HRMM.Controllers;

/// <summary>
/// End point to get the list of names & Ids of Core.HRMM entities
/// </summary>

//[Authorize]
[ApiController]
[Route("api/core/hrmm/v{version:apiVersion}/Names")]
[ApiVersion("1.0")]
public class NameListController(IMediator med) : ControllerBase
{
    [HttpGet("AllAddressName")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllAddressName()
    {
        var res = await med.Send(new AddressNameAllQry());
        return Ok(res);
    }

    [HttpGet("GetAddressName/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAddressName(Guid id)
    {
        var res = await med.Send(new AddressNameByIdQry { Id = id });
        if (res == null) { return NotFound(new { Error = $"ADDRESS with Id {id} not found" }); }
        return Ok(res);
    }

    [HttpGet("AllJobGradeName")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllJobGradeName()
    {
        var res = await med.Send(new JobGradeNameAllQry());
        return Ok(res);
    }

    [HttpGet("GetJobGradeName/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetJobGradeName(Guid id)
    {
        var res = await med.Send(new JobGradeNameByIdQry { Id = id });
        if (res == null) { return NotFound(new { Error = $"JOB GRADE with Id {id} not found" }); }
        return Ok(res);
    }

    [HttpGet("AllPositionName")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllPositionName()
    {
        var res = await med.Send(new PositionNameAllQry());
        return Ok(res);
    }

    [HttpGet("GetPositionName/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPositionName(Guid id)
    {
        var res = await med.Send(new PositionNameByIdQry { Id = id });
        if (res == null) { return NotFound(new { Error = $"POSITION with Id {id} not found" }); }
        return Ok(res);
    }



}
