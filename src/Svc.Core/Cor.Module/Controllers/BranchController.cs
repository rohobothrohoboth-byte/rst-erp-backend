using Asp.Versioning;
using Cor.Module.Commands;
using Cor.Module.Helpers;
using Cor.Module.Models.DTOs;
using Cor.Module.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cor.Module.Controllers;

/// <summary>
/// Branch management end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/core/module/v{version:apiVersion}/Branch")]
[ApiVersion("1.0")]
public class BranchController(IMediator med) : ControllerBase
{
    [HttpGet("AllBranch")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllBranch()
    {
        var response = await med.Send(new AllBranchesQry());
        //return Ok(response);
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetBranch/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBranch(Guid id)
    {
        var response = await med.Send(new BranchByIdQry { Id = id });
        if (response == null) { throw new DomainException($"BRANCH with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    /// <summary>
    /// End point to get list of Branches by CompanyId
    /// </summary>
    [HttpGet("BranchComp/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CompBranches(Guid id)
    {
        var response = await med.Send(new BranchByCompQry { Id = id });
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("AddBranch")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AddBranchDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValidationException(errors);
        }
        
        var command = new AddBranchCmd { AddBranchDto = addDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "New BRANCH successfully created."));
    }
    
    [HttpPut("ModBranch/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] EditBranchDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValidationException(errors);
        }

        var command = new ModBranchCmd { EditBranchDto = modDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Selected BRANCH successfully updated."));
    }

    [HttpDelete("DelBranch/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DelBranchCmd { Id = id };
        await med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, $"BRANCH with Id {id} successfully deleted."));
    }
}