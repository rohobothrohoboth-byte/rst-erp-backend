using Asp.Versioning;
using Leave.App.Commands;
using Leave.App.Helpers;
using Leave.App.Queries;
using Leave.Domain.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Leave.API.Controllers;

/// <summary>
/// LEAVE POLICY ASSIGMENT RULE end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/leave/v{version:apiVersion}/PolicyAssignmentRule")]
[ApiVersion("1.0")]
public class PolicyAssRuleController(IMediator med) : ControllerBase
{
    [HttpGet("ActivePolicyAssRule/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivePolicyAssRule(Guid id)
    {
        var response = await med.Send(new ActiveAssignmentRulesQry { Id = id });
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("AllPolicyAssRule/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AllPolicyAssRule(Guid id)
    {
        var response = await med.Send(new AssignmentRuleByPolicyIdQry { Id = id });
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetPolicyAssRule/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPolicyAssRule(Guid id)
    {
        var response = await med.Send(new PolicyAssignmentRuleByIdQry { Id = id });
        if (response == null) { throw new DomainException($"LEAVE POLICY ASSIGMENT RULE with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("StatPolicyAssRule")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangeStat([FromBody] StatChangeDto statDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new PolicyAssignmentRuleStatCmd { StatDto = statDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "LEAVE POLICY ASSIGMENT RULE status successfully changed."));
    }

    [HttpPost("AddPolicyAssRule")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] PolicyAssignmentRuleAddDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new PolicyAssignmentRuleAddCmd { AddDto = addDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "New LEAVE POLICY ASSIGMENT RULE successfully created."));
    }

    [HttpPut("ModPolicyAssRule/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] PolicyAssignmentRuleModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new PolicyAssignmentRuleModCmd { ModDto = modDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Selected LEAVE POLICY ASSIGMENT RULE successfully updated."));
    }

    [HttpDelete("DelPolicyAssRule/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new PolicyAssignmentRuleDelCmd { Id = id };
        await med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, $"LEAVE POLICY ASSIGMENT RULE with Id {id} successfully deleted."));
    }
}