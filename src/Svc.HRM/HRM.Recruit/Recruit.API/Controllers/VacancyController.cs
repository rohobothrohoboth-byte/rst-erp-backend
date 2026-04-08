using Asp.Versioning;
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
    [HttpGet("PublishedVacancy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> PublishedVacancy()
    {
        var response = await med.Send(new VacancyListQry());
        return Ok(ApiResponse<object>.Ok(response));
    }




}