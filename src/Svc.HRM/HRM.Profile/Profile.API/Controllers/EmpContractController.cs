using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Profile.App.Commands;
using Profile.App.Queries;
using Profile.Domain.DTOs;

namespace Profile.API.Controllers;

[ApiController]
[Route("api/hrm/profile/v{version:apiVersion}/EmpContract")]
[ApiVersion("1.0")]
public class EmpContractController(IMediator med) : ControllerBase
{
    [HttpGet("All")]
    public async Task<IActionResult> All() =>
        Ok(ApiResponse<object>.Ok(await med.Send(new EmpContractAllQry())));

    [HttpGet("Get/{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var response = await med.Send(new EmpContractByIdQry { Id = id })
            ?? throw new DomainException($"Contract [{id}] NOT FOUND.");
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("ByEmployee/{employeeId:guid}")]
    public async Task<IActionResult> ByEmployee(Guid employeeId) =>
        Ok(ApiResponse<object>.Ok(await med.Send(new EmpContractByEmployeeQry { EmployeeId = employeeId })));

    [HttpPost("Add")]
    public async Task<IActionResult> Add([FromBody] EmpContractAddDto dto)
    {
        EnsureValid();
        var response = await med.Send(new EmpContractAddCmd { Dto = dto });
        return Ok(ApiResponse<object>.Ok(response, "Contract created."));
    }

    [HttpPut("Mod/{id:guid}")]
    public async Task<IActionResult> Mod(Guid id, [FromBody] EmpContractModDto dto)
    {
        EnsureValid();
        if (dto.Id != id) throw new ValException("ID mismatch between route and body.");
        var response = await med.Send(new EmpContractModCmd { Dto = dto });
        return Ok(ApiResponse<object>.Ok(response, "Contract updated."));
    }

    [HttpPost("Activate/{id:guid}")]
    public async Task<IActionResult> Activate(Guid id) =>
        Ok(ApiResponse<object>.Ok(await med.Send(new EmpContractActivateCmd { Id = id }), "Contract activated."));

    [HttpPost("Terminate")]
    public async Task<IActionResult> Terminate([FromBody] EmpContractTerminateDto dto)
    {
        EnsureValid();
        var response = await med.Send(new EmpContractTerminateCmd { Dto = dto });
        return Ok(ApiResponse<object>.Ok(response, "Contract terminated."));
    }

    [HttpPost("Renew")]
    public async Task<IActionResult> Renew([FromBody] EmpContractRenewDto dto)
    {
        EnsureValid();
        var response = await med.Send(new EmpContractRenewCmd { Dto = dto });
        return Ok(ApiResponse<object>.Ok(response, "Contract renewed."));
    }

    [HttpDelete("Del/{id:guid}")]
    public async Task<IActionResult> Del(Guid id)
    {
        await med.Send(new EmpContractDelCmd { Id = id });
        return Ok(ApiResponse<string>.Ok(null!, $"Contract {id} deleted."));
    }

    private void EnsureValid()
    {
        if (ModelState.IsValid) return;
        throw new ValException(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList());
    }
}
