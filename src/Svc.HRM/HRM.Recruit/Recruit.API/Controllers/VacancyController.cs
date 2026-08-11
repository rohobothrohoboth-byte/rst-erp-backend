using Asp.Versioning;
using Common;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Recruit.App.Queries;

namespace Recruit.API.Controllers;

/// <summary>
/// VACANCY management end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/recruit/v{version:apiVersion}/Vacancy")]
[ApiVersion("1.0")]
public class VacancyController(IMediator med) : ControllerBase
{
    [PerAuth("hr.recruit.requisition.view")]
    [HttpGet("PublishedVacancy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> PublishedVacancy()
    {
        var response = await med.Send(new VacancyListQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.recruit.requisition.view")]
    [HttpGet("InternalVacancy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> InternalVacancy()
    {
        var response = await med.Send(new InternalVacancyListQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.recruit.requisition.view")]
    [HttpGet("ExternalVacancy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ExternalVacancy()
    {
        var response = await med.Send(new ExternalVacancyListQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.recruit.requisition.view")]
    [HttpGet("GetVacancy/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVacancy(Guid id)
    {
        var response = await med.Send(new VacancyDetailQry { Id = id });
        if (response == null)
        {
            throw new DomainException($"VACANCY with id [{id}] NOT FOUND.");
        }
        return Ok(ApiResponse<object>.Ok(response));
    }
}