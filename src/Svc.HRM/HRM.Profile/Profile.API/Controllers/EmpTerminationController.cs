using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Profile.App.Commands;
using Profile.App.Queries;
using Profile.Domain.DTOs;

namespace Profile.API.Controllers;

[ApiController]
[Route("api/hrm/profile/v{version:apiVersion}/EmpTermination")]
[ApiVersion("1.0")]
public class EmpTerminationController(IMediator med) : ControllerBase
{
    [HttpGet("All")]
    public async Task<IActionResult> All() =>
        Ok(ApiResponse<object>.Ok(await med.Send(new EmpTerminationAllQry())));

    [HttpGet("Get/{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var response = await med.Send(new EmpTerminationByIdQry { Id = id })
            ?? throw new DomainException($"Termination [{id}] NOT FOUND.");
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("ByEmployee/{employeeId:guid}")]
    public async Task<IActionResult> ByEmployee(Guid employeeId) =>
        Ok(ApiResponse<object>.Ok(await med.Send(new EmpTerminationByEmployeeQry { EmployeeId = employeeId })));

    [HttpPost("Add")]
    public async Task<IActionResult> Add([FromBody] EmpTerminationAddDto dto)
    {
        EnsureValid();
        return Ok(ApiResponse<object>.Ok(await med.Send(new EmpTerminationAddCmd { Dto = dto }), "Termination request created."));
    }

    [HttpPut("Mod")]
    public async Task<IActionResult> Mod([FromBody] EmpTerminationModDto dto)
    {
        EnsureValid();
        return Ok(ApiResponse<object>.Ok(await med.Send(new EmpTerminationModCmd { Dto = dto }), "Termination updated."));
    }

    [HttpPost("Approve")]
    public async Task<IActionResult> Approve([FromBody] EmpTerminationDecisionDto dto)
    {
        EnsureValid();
        return Ok(ApiResponse<object>.Ok(await med.Send(new EmpTerminationApproveCmd { Dto = dto }), "Termination approved."));
    }

    [HttpPost("Reject")]
    public async Task<IActionResult> Reject([FromBody] EmpTerminationDecisionDto dto)
    {
        EnsureValid();
        return Ok(ApiResponse<object>.Ok(await med.Send(new EmpTerminationRejectCmd { Dto = dto }), "Termination rejected."));
    }

    [HttpPost("Apply")]
    public async Task<IActionResult> Apply([FromBody] EmpTerminationDecisionDto dto)
    {
        EnsureValid();
        return Ok(ApiResponse<object>.Ok(await med.Send(new EmpTerminationApplyCmd { Dto = dto }),
            "Termination applied. Employee terminated; settlement hooks executed."));
    }

    [HttpDelete("Del/{id:guid}")]
    public async Task<IActionResult> Del(Guid id)
    {
        await med.Send(new EmpTerminationDelCmd { Id = id });
        return Ok(ApiResponse<string>.Ok(null!, $"Termination {id} deleted."));
    }

    [HttpGet("Offboarding/{terminationId:guid}")]
    public async Task<IActionResult> Offboarding(Guid terminationId) =>
        Ok(ApiResponse<object>.Ok(await med.Send(new EmpOffboardingByTerminationQry { TerminationId = terminationId })));

    [HttpPost("Offboarding/Add")]
    public async Task<IActionResult> AddTask([FromBody] EmpOffboardingTaskAddDto dto)
    {
        EnsureValid();
        return Ok(ApiResponse<object>.Ok(await med.Send(new EmpOffboardingTaskAddCmd { Dto = dto }), "Offboarding task added."));
    }

    [HttpPut("Offboarding/Update")]
    public async Task<IActionResult> UpdateTask([FromBody] EmpOffboardingTaskUpdateDto dto)
    {
        EnsureValid();
        return Ok(ApiResponse<object>.Ok(await med.Send(new EmpOffboardingTaskUpdateCmd { Dto = dto }), "Offboarding task updated."));
    }

    [HttpDelete("Offboarding/Del/{id:guid}")]
    public async Task<IActionResult> DelTask(Guid id)
    {
        await med.Send(new EmpOffboardingTaskDelCmd { Id = id });
        return Ok(ApiResponse<string>.Ok(null!, $"Offboarding task {id} deleted."));
    }

    private void EnsureValid()
    {
        if (ModelState.IsValid) return;
        throw new ValException(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList());
    }
}
