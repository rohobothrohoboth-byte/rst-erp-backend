using Asp.Versioning;
using Leave.App.Commands;
using Leave.App.Helpers;
using Leave.App.Queries;
using Leave.Domain.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Leave.API.Controllers;

/// <summary>
/// LEAVE POLICY ACCRUAL end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/leave/v{version:apiVersion}/LeavePolicyAcc")]
[ApiVersion("1.0")]
public class LeavePolicyAccController(IMediator med) : ControllerBase
{
    [HttpGet("PolicyLeaveAcc/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PolicyLeaveAcc(Guid id)
    {
        //var response = await med.Send(new PolicyLeaveAccrualQry { Id = id });
        //return Ok(ApiResponse<object>.Ok(response));
        return Ok();
    }

    [HttpGet("AllLeavePolicyAcc")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllLeavePolicyAcc()
    {
        //var response = await med.Send(new LeavePolicyAccrualAllQry ());
        //return Ok(ApiResponse<object>.Ok(response));
        return Ok();
    }

    [HttpGet("GetLeavePolicyAcc/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLeavePolicyAcc(Guid id)
    {
        //var response = await med.Send(new LeavePolicyAccrualByIdQry { Id = id });
        //if (response == null) { throw new DomainException($"LEAVE POLICY ACCRUAL with id [{id}] NOT FOUND."); }
        //return Ok(ApiResponse<object>.Ok(response));
        return Ok();
    }

    [HttpPost("AddLeavePolicyAcc")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] LeavePolicyConfigAddDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValidationException(errors);
        }

        //var command = new LeavePolicyAccrualAddCmd { AddDto = addDto };
        //var response = await med.Send(command);
        //return Ok(ApiResponse<object>.Ok(response, "New LEAVE POLICY ACCRUAL successfully created."));
        return Ok();
    }

    [HttpPut("ModLeavePolicyAcc/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] LeavePolicyConfigModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValidationException(errors);
        }

        //var command = new LeavePolicyAccrualModCmd { ModDto = modDto };
        //var response = await med.Send(command);
        //return Ok(ApiResponse<object>.Ok(response, "Selected LEAVE POLICY ACCRUAL successfully updated."));
        return Ok();
    }

    [HttpDelete("DelLeavePolicyAcc/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        //var command = new LeavePolicyAccrualDelCmd { Id = id };
        //await med.Send(command);
        //return Ok(ApiResponse<string>.Ok(null!, $"LEAVE POLICY ACCRUAL with Id {id} successfully deleted."));
        return Ok();
    }
}