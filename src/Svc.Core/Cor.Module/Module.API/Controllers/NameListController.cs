using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Module.App.Queries;

namespace Module.API.Controllers;

/// <summary>
/// End point to get the list of names & Ids of Core.Module entities
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
        var branches = await med.Send(new BranchCompListQry());
        return Ok(branches);
    }
    
    [HttpGet("AllDeptName")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllDeptName()
    {
        var depts = await med.Send(new AllDeptNameQry());
        return Ok(depts);
    }

    [HttpGet("GetDeptName/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDeptName(Guid id)
    {
        var dept = await med.Send(new DeptNameByIdQry { Id = id });
        if (dept == null)
        {
            return NotFound(new { Error = $"DEPARTMENT with Id {id} not found" });
        }
        return Ok(dept);
    }



}
