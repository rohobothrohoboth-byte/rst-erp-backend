using Asp.Versioning;
using Cor.Module.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cor.Module.Controllers;

/// <summary>
/// End point to get the list of names and Ids of Core.Module entities
/// </summary>

//[Authorize]
[ApiController]
[Route("api/core/module/v{version:apiVersion}/Names")]
[ApiVersion("1.0")]
public class NameListController(IMediator med) : ControllerBase
{
    /// <summary>
    /// End point to get list of branches with company, Response will be (BranchName => CompanyName) or (BranchNameAm => CompanyNameAm) including Id of Branch.
    /// </summary>
    [HttpGet("BranchCompList")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> BranchCompList()
    {
        var res = await med.Send(new BranchCompListQry());
        return Ok(res);
    }
    
    [HttpGet("AllDeptName")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllDeptName()
    {
        var res = await med.Send(new DeptAllNameQry());
        return Ok(res);
    }

    [HttpGet("GetDeptName/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDeptName(Guid id)
    {
        var res = await med.Send(new DeptNameByIdQry { Id = id });
        if (res == null) { return NotFound(new { Error = $"DEPARTMENT with Id {id} NOT FOUND" }); }
        return Ok(res);
    }
    
    [HttpGet("AllCompName")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllCompName()
    {
        var res = await med.Send(new CompAllNameQry());
        return Ok(res);
    }

    [HttpGet("GetCompName/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCompName(Guid id)
    {
        var res = await med.Send(new CompNameByIdQry { Id = id });
        if (res == null) { return NotFound(new { Error = $"COMPANY with Id {id} NOT FOUND" }); }
        return Ok(res);
    }

    [HttpGet("AllFiscYearName")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllFiscYearName()
    {
        var res = await med.Send(new FiscalYearAllNameQry());
        return Ok(res);
    }

    [HttpGet("GetFiscYearName/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFiscYearName(Guid id)
    {
        var res = await med.Send(new FiscalYearNameByIdQry { Id = id });
        if (res == null) { return NotFound(new { Error = $"FISCAL YEAR with Id {id} NOT FOUND" }); }
        return Ok(res);
    }

    [HttpGet("AllPeriodName")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllPeriodName()
    {
        var depts = await med.Send(new PeriodAllNameQry());
        return Ok(depts);
    }

    [HttpGet("GetPeriodName/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPeriodName(Guid id)
    {
        var dept = await med.Send(new PeriodNameByIdQry { Id = id });
        if (dept == null)
        {
            return NotFound(new { Error = $"PERIOD with Id {id} NOT FOUND" });
        }
        return Ok(dept);
    }
}
