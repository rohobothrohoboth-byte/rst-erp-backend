using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Recruit.App.Commands;
using Recruit.App.Queries;
using Recruit.Domain.DTOs;

namespace Recruit.API.Controllers;

/// <summary>
/// ONBOARDING ASSIGNMENTS for hired employees
/// </summary>
[ApiController]
[Route("api/hrm/recruit/v{version:apiVersion}/OnboardingAssign")]
[ApiVersion("1.0")]
public class OnboardingAssignController(IMediator med) : ControllerBase
{
    [HttpGet("All")]
    public async Task<IActionResult> All()
    {
        var response = await med.Send(new OnboardingAssignAllQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("Get/{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var response = await med.Send(new OnboardingAssignByIdQry { Id = id });
        if (response == null)
            throw new DomainException($"ONBOARDING ASSIGN with id [{id}] NOT FOUND.");
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("ByEmployee/{employeeId:guid}")]
    public async Task<IActionResult> ByEmployee(Guid employeeId)
    {
        var response = await med.Send(new OnboardingAssignByEmployeeQry { EmployeeId = employeeId });
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("Add")]
    public async Task<IActionResult> Add([FromBody] OnboardingAssignAddDto addDto)
    {
        EnsureValid();
        var response = await med.Send(new OnboardingAssignAddCmd { AddDto = addDto });
        return Ok(ApiResponse<object>.Ok(response, "Onboarding assignment created."));
    }

    [HttpPost("BulkAdd")]
    public async Task<IActionResult> BulkAdd([FromBody] OnboardingAssignBulkAddDto addDto)
    {
        EnsureValid();
        var response = await med.Send(new OnboardingAssignBulkAddCmd { AddDto = addDto });
        return Ok(ApiResponse<object>.Ok(response, "Onboarding assignments created."));
    }

    [HttpPut("Mod/{id:guid}")]
    public async Task<IActionResult> Mod(Guid id, [FromBody] OnboardingAssignModDto modDto)
    {
        EnsureValid();
        if (modDto.Id != id)
            throw new ValException("ID mismatch between route and body.");
        var response = await med.Send(new OnboardingAssignModCmd { ModDto = modDto });
        return Ok(ApiResponse<object>.Ok(response, "Onboarding assignment updated."));
    }

    [HttpPost("Complete")]
    public async Task<IActionResult> Complete([FromBody] OnboardingAssignCompleteDto dto)
    {
        EnsureValid();
        var response = await med.Send(new OnboardingAssignCompleteCmd { Dto = dto });
        return Ok(ApiResponse<object>.Ok(response, "Onboarding assignment completed."));
    }

    [HttpDelete("Del/{id:guid}")]
    public async Task<IActionResult> Del(Guid id)
    {
        await med.Send(new OnboardingAssignDelCmd { Id = id });
        return Ok(ApiResponse<string>.Ok(null!, $"Onboarding assignment {id} deleted."));
    }

    private void EnsureValid()
    {
        if (ModelState.IsValid) return;
        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
        throw new ValException(errors);
    }
}
