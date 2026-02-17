using Asp.Versioning;
using Cor.HRMM.Queries;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cor.HRMM.Controllers;

/// <summary>
/// End point to get the list of names and Ids of Core.HRMM entities
/// </summary>

//[Authorize]
[ApiController]
[Route("api/core/hrmm/v{version:apiVersion}/Names")]
[ApiVersion("1.0")]
public class NameListController(IMediator med) : ControllerBase
{
    [HttpGet("AllBenefitSetName")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllBenefitSetName()
    {
        var res = await med.Send(new BenefitSetNameAllQry());
        return Ok(res);
    }

    [HttpGet("GetBenefitSetName/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBenefitSetName(Guid id)
    {
        var res = await med.Send(new BenefitSetNameByIdQry { Id = id });
        if (res == null) { throw new DomainException($"BENEFIT SETTING with id [{id}] NOT FOUND."); }
        return Ok(res);
    }

    [HttpGet("AllEducationQualName")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllEducationQualName()
    {
        var res = await med.Send(new EducationQualNameAllQry());
        return Ok(res);
    }

    [HttpGet("GetEducationQualName/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEducationQualName(Guid id)
    {
        var res = await med.Send(new EducationQualNameByIdQry { Id = id });
        if (res == null) { throw new DomainException($"EDUCATION QUALIFICATION with id [{id}] NOT FOUND."); }
        return Ok(res);
    }

    /// <summary>
    /// End point to get the list of JOB GRADE STEP names and Ids. Response will be in the form of (JOB GRADE STEP NAME => JOB GRADE)
    /// </summary>
    [HttpGet("AllJgStepName")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllJgStepName()
    {
        var res = await med.Send(new JgStepNameAllQry());
        return Ok(res);
    }

    /// <summary>
    /// End point to get JOB GRADE STEP name and Id. Response will be in the form of (JOB GRADE STEP NAME => JOB GRADE)
    /// </summary>
    [HttpGet("GetJgStepName/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetJgStepName(Guid id)
    {
        var res = await med.Send(new JgStepNameByIdQry { Id = id });
        if (res == null) { throw new DomainException($"JOB GRADE STEP with id [{id}] NOT FOUND."); }
        return Ok(res);
    }

    [HttpGet("AllJobGradeName")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllJobGradeName()
    {
        var res = await med.Send(new JobGradeNameAllQry());
        return Ok(res);
    }

    [HttpGet("GetJobGradeName/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetJobGradeName(Guid id)
    {
        var res = await med.Send(new JobGradeNameByIdQry { Id = id });
        if (res == null) { throw new DomainException($"JOB GRADE with id [{id}] NOT FOUND."); }
        return Ok(res);
    }

    /// <summary>
    /// End point to get list of Positions by DepartmentId
    /// </summary>
    [HttpGet("DeptPosition/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeptPosition(Guid id)
    {
        var res = await med.Send(new PositionByDeptQry { Id = id });
        return Ok(res);
    }

    [HttpGet("AllPositionName")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllPositionName()
    {
        var res = await med.Send(new PositionNameAllQry());
        return Ok(res);
    }

    [HttpGet("GetPositionName/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPositionName(Guid id)
    {
        var res = await med.Send(new PositionNameByIdQry { Id = id });
        if (res == null) { throw new DomainException($"POSITION with id [{id}] NOT FOUND."); }
        return Ok(res);
    }
        
    [HttpGet("GetUser/{token}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUser(string token)
    {
        var response = await med.Send(new ValUserQry { Token = token });
        return Ok(ApiResponse<object>.Ok(response));
    }

}
