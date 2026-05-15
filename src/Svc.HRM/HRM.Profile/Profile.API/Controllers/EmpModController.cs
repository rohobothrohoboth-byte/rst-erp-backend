using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Profile.App.Commands;
using Profile.App.Queries;
using Profile.Domain.DTOs;

namespace Profile.API.Controllers;

/// <summary>
/// Employee Information Update Management end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/profile/v{version:apiVersion}/EmpMod")]
[ApiVersion("1.0")]
public class EmpModController(IMediator med) : ControllerBase
{
    [HttpGet("EmpModBasic/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EmpModBasic(Guid id)
    {
        var response = await med.Send(new EmpModBasicQry { Id = id });
        return response == null ? Ok(ApiResponse<object>.Fail("EMPLOYEE'S Basic Info NOT FOUND.", null, 404)) : Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("EmpModBio/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EmpModBio(Guid id)
    {
        var response = await med.Send(new EmpModBioQry { Id = id });
        return response == null ? Ok(ApiResponse<object>.Fail("EMPLOYEE'S Biographical Info NOT FOUND.", null, 404)) : Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("EmpModGuar/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EmpModGuar(Guid id)
    {
        var response = await med.Send(new EmpModGuarQry { Id = id });
        return response == null ? Ok(ApiResponse<object>.Fail("EMPLOYEE'S Guarantor Info NOT FOUND.", null, 404)) : Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("EmpCertAll/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EmpCertAll(Guid id)
    {
        var response = await med.Send(new EmpCertAllQry { Id = id });
        return response == null ? Ok(ApiResponse<object>.Fail("EMPLOYEE'S Certificates NOT FOUND.", null, 404)) : Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("EmpCertById/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EmpCertById(Guid id)
    {
        var res = await med.Send(new EmpCertByIdQry { Id = id });
        if (res == null)
        {
            return Ok(ApiResponse<object>.Fail("Certificate NOT FOUND.", null, 404));
        }

        return File(res.Data, res.ContentType, res.FileName, enableRangeProcessing: true);
    }

    // Mods
    [HttpPut("EmpBasicMod/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> EmpBasicMod(Guid id, [FromForm] EmpModBasicDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var response = await med.Send(new EmpBasicModCmd { ModDto = modDto });
        return Ok(ApiResponse<object>.Ok(response, "Selected EMPLOYEE'S Basic Info successfully updated."));
    }

    [HttpPut("EmpBioMod/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> EmpBioMod(Guid id, [FromBody] EmpModBioDto modDto)
    {
        if (!ModelState.IsValid || modDto.EmployeeId != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var response = await med.Send(new EmpBioModCmd { ModDto = modDto });
        return Ok(ApiResponse<object>.Ok(response, "Selected EMPLOYEE'S Biographical Info successfully updated."));
    }

    [HttpPut("EmpGuarMod/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> EmpGuarMod(Guid id, [FromForm] EmpModGuarDto modDto)
    {
        if (!ModelState.IsValid || modDto.EmployeeId != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var response = await med.Send(new EmpGuarModCmd { ModDto = modDto });
        return Ok(ApiResponse<object>.Ok(response, "Selected EMPLOYEE'S Guarantor Info successfully updated."));
    }

    [HttpPut("EmpStamp/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> EmpStamp(Guid id, [FromForm] ModFileDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var response = await med.Send(new EmpStampModCmd { Dto = modDto });
        return Ok(ApiResponse<object>.Ok(response, "Selected EMPLOYEE'S Stamp File successfully Stored."));
    }

    [HttpPut("EmpSign/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> EmpSign(Guid id, [FromForm] ModFileDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var response = await med.Send(new EmpSignModCmd { Dto = modDto });
        return Ok(ApiResponse<object>.Ok(response, "Selected EMPLOYEE'S Signature File successfully Stored."));
    }


}