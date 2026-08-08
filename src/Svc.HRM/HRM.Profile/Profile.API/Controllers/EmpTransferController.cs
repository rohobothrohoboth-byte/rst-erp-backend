using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Profile.App.Commands;
using Profile.App.Queries;
using Profile.Domain.DTOs;

namespace Profile.API.Controllers;

[ApiController]
[Route("api/hrm/profile/v{version:apiVersion}/EmpTransfer")]
[ApiVersion("1.0")]
public class EmpTransferController(IMediator med) : ControllerBase
{
    [HttpGet("All")]
    public async Task<IActionResult> All() =>
        Ok(ApiResponse<object>.Ok(await med.Send(new EmpTransferAllQry())));

    [HttpGet("Get/{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var response = await med.Send(new EmpTransferByIdQry { Id = id })
            ?? throw new DomainException($"Transfer [{id}] NOT FOUND.");
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("ByEmployee/{employeeId:guid}")]
    public async Task<IActionResult> ByEmployee(Guid employeeId) =>
        Ok(ApiResponse<object>.Ok(await med.Send(new EmpTransferByEmployeeQry { EmployeeId = employeeId })));

    [HttpPost("Add")]
    public async Task<IActionResult> Add([FromBody] EmpTransferAddDto dto)
    {
        EnsureValid();
        return Ok(ApiResponse<object>.Ok(await med.Send(new EmpTransferAddCmd { Dto = dto }), "Transfer request created."));
    }

    [HttpPost("Approve")]
    public async Task<IActionResult> Approve([FromBody] EmpTransferDecisionDto dto)
    {
        EnsureValid();
        return Ok(ApiResponse<object>.Ok(await med.Send(new EmpTransferApproveCmd { Dto = dto }), "Transfer approved."));
    }

    [HttpPost("Reject")]
    public async Task<IActionResult> Reject([FromBody] EmpTransferDecisionDto dto)
    {
        EnsureValid();
        return Ok(ApiResponse<object>.Ok(await med.Send(new EmpTransferRejectCmd { Dto = dto }), "Transfer rejected."));
    }

    [HttpPost("Apply")]
    public async Task<IActionResult> Apply([FromBody] EmpTransferDecisionDto dto)
    {
        EnsureValid();
        return Ok(ApiResponse<object>.Ok(await med.Send(new EmpTransferApplyCmd { Dto = dto }), "Transfer applied to employee record."));
    }

    [HttpDelete("Del/{id:guid}")]
    public async Task<IActionResult> Del(Guid id)
    {
        await med.Send(new EmpTransferDelCmd { Id = id });
        return Ok(ApiResponse<string>.Ok(null!, $"Transfer {id} deleted."));
    }

    private void EnsureValid()
    {
        if (ModelState.IsValid) return;
        throw new ValException(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList());
    }
}
