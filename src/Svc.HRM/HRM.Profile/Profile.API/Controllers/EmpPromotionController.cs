using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Profile.App.Commands;
using Profile.App.Queries;
using Profile.Domain.DTOs;

namespace Profile.API.Controllers;

[ApiController]
[Route("api/hrm/profile/v{version:apiVersion}/EmpPromotion")]
[ApiVersion("1.0")]
public class EmpPromotionController(IMediator med) : ControllerBase
{
    [HttpGet("All")]
    public async Task<IActionResult> All() =>
        Ok(ApiResponse<object>.Ok(await med.Send(new EmpPromotionAllQry())));

    [HttpGet("Get/{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var response = await med.Send(new EmpPromotionByIdQry { Id = id })
            ?? throw new DomainException($"Promotion [{id}] NOT FOUND.");
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("ByEmployee/{employeeId:guid}")]
    public async Task<IActionResult> ByEmployee(Guid employeeId) =>
        Ok(ApiResponse<object>.Ok(await med.Send(new EmpPromotionByEmployeeQry { EmployeeId = employeeId })));

    [HttpPost("Add")]
    public async Task<IActionResult> Add([FromBody] EmpPromotionAddDto dto)
    {
        EnsureValid();
        return Ok(ApiResponse<object>.Ok(await med.Send(new EmpPromotionAddCmd { Dto = dto }), "Promotion request created."));
    }

    [HttpPost("Approve")]
    public async Task<IActionResult> Approve([FromBody] EmpPromotionDecisionDto dto)
    {
        EnsureValid();
        return Ok(ApiResponse<object>.Ok(await med.Send(new EmpPromotionApproveCmd { Dto = dto }), "Promotion approved."));
    }

    [HttpPost("Reject")]
    public async Task<IActionResult> Reject([FromBody] EmpPromotionDecisionDto dto)
    {
        EnsureValid();
        return Ok(ApiResponse<object>.Ok(await med.Send(new EmpPromotionRejectCmd { Dto = dto }), "Promotion rejected."));
    }

    [HttpPost("Apply")]
    public async Task<IActionResult> Apply([FromBody] EmpPromotionDecisionDto dto)
    {
        EnsureValid();
        return Ok(ApiResponse<object>.Ok(await med.Send(new EmpPromotionApplyCmd { Dto = dto }), "Promotion applied to employee record."));
    }

    [HttpDelete("Del/{id:guid}")]
    public async Task<IActionResult> Del(Guid id)
    {
        await med.Send(new EmpPromotionDelCmd { Id = id });
        return Ok(ApiResponse<string>.Ok(null!, $"Promotion {id} deleted."));
    }

    private void EnsureValid()
    {
        if (ModelState.IsValid) return;
        throw new ValException(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList());
    }
}
