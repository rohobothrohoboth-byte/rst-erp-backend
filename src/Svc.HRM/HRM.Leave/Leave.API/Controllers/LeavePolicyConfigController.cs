using Asp.Versioning;
using Leave.App.Commands;
using Leave.App.Helpers;
using Leave.App.Queries;
using Leave.Domain.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Leave.API.Controllers;

/// <summary>
/// LEAVE POLICY CONFIGURATION end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/leave/v{version:apiVersion}/LeavePolicyConfig")]
[ApiVersion("1.0")]
public class LeavePolicyConfigController(IMediator med) : ControllerBase
{
    [HttpGet("ActivePolicyConfig/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivePolicyConfig(Guid id)
    {
        var response = await med.Send(new ActivePolicyConfigQry { Id = id });
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("AllPolicyConfig/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AllPolicyConfig(Guid id)
    {
        var response = await med.Send(new PolicyConfigByPolicyIdQry { Id = id });
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetPolicyConfig/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPolicyConfig(Guid id)
    {
        var response = await med.Send(new LeavePolicyConfigByIdQry { Id = id });
        if (response == null) { throw new DomainException($"LEAVE POLICY CONFIGURATION with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("StatPolicyConfig")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangeStat([FromBody] StatChangeDto statDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new LeavePolicyConfigStatCmd { StatDto = statDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "LEAVE POLICY CONFIGURATION status successfully changed."));
    }

    [HttpPost("AddPolicyConfig")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] LeavePolicyConfigAddDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new LeavePolicyConfigAddCmd { AddDto = addDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "New LEAVE POLICY CONFIGURATION successfully created."));
    }

    [HttpPut("ModPolicyConfig/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] LeavePolicyConfigModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new LeavePolicyConfigModCmd { ModDto = modDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Selected LEAVE POLICY CONFIGURATION successfully updated."));
    }

    [HttpDelete("DelPolicyConfig/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new LeavePolicyConfigDelCmd { Id = id };
        await med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, $"LEAVE POLICY CONFIGURATION with Id {id} successfully deleted."));
    }
}