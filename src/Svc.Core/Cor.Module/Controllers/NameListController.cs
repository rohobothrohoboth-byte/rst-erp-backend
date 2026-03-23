using Asp.Versioning;
using Common;
using Cor.Module.Queries;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cor.Module.Controllers;

/// <summary>
/// End point to get the list of names and Ids of Core.Module entities
/// </summary>

[Authorize]
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
        var res = await med.Send(new BraCompListQry());
        return Ok(res);
    }

    /// <summary>
    /// End point to get list of branches with company by Id, Response will be (BranchName => CompanyName) or (BranchNameAm => CompanyNameAm) including Id of Branch.
    /// </summary>
    [HttpGet("GetBranchCompList/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBranchCompList(Guid id)
    {
        var res = await med.Send(new BraCompByIdQry { Id = id });
        if (res == null) { throw new DomainException($"BRANCH with id [{id}] NOT FOUND."); }
        return Ok(res);
    }

    [HttpGet("AllBranchName")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllBranchName()
    {
        var res = await med.Send(new BraAllNameQry());
        return Ok(res);
    }

    [HttpGet("GetBranchName/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBranchName(Guid id)
    {
        var res = await med.Send(new BraNameByIdQry { Id = id });
        if (res == null) { throw new DomainException($"BRANCH with id [{id}] NOT FOUND."); }
        return Ok(res);
    }

    /// <summary>
    /// End point to get list of Departments by BranchId
    /// </summary>
    [HttpGet("BranchDept/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> BranchDept(Guid id)
    {
        var res = await med.Send(new DeptByBraQry { Id = id });
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
        if (res == null) { throw new DomainException($"DEPARTMENT with id [{id}] NOT FOUND."); }
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
        if (res == null) { throw new DomainException($"COMPANY with id [{id}] NOT FOUND."); }
        return Ok(res);
    }

    [HttpGet("AllFiscYearName")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllFiscYearName()
    {
        var res = await med.Send(new FiscYearAllNameQry());
        return Ok(res);
    }

    [HttpGet("GetFiscYearName/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFiscYearName(Guid id)
    {
        var res = await med.Send(new FiscYearNameByIdQry { Id = id });
        if (res == null) { throw new DomainException($"FISCAL YEAR with id [{id}] NOT FOUND."); }
        return Ok(res);
    }

    [HttpGet("ActiveFiscYear")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ActiveFiscYear()
    {
        var res = await med.Send(new FiscYearActiveQry());
        return Ok(res);
    }

    [HttpGet("AllPeriodName")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllPeriodName()
    {
        var res = await med.Send(new PeriodAllNameQry());
        return Ok(res);
    }

    [HttpGet("GetPeriodName/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPeriodName(Guid id)
    {
        var res = await med.Send(new PeriodNameByIdQry { Id = id });
        if (res == null) { throw new DomainException($"PERIOD with id [{id}] NOT FOUND."); }
        return Ok(res);
    }
}
