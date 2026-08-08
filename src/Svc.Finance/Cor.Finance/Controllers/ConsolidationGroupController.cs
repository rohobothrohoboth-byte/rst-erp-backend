// Controllers/ConsolidationGroupController.cs
using Cor.Finance.Commands;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace Cor.Finance.Controllers;
// Controllers/ConsolidationGroupController.cs
[ApiController]
[Route("api/finance/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class ConsolidationGroupController : BaseApiController
{
    private readonly IMediator _mediator;

    public ConsolidationGroupController(IMediator mediator, ILogger<ConsolidationGroupController> logger)
        : base(mediator, logger)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ConsolidationGroupDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] string? status, [FromQuery] Guid? periodId)
    {
        try
        {
            var result = await _mediator.Send(new GetAllConsolidationGroupsQry { Status = status, PeriodId = periodId });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetAllConsolidationGroups");
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ConsolidationGroupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetConsolidationGroupByIdQry { Id = id });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetConsolidationGroupById", id);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ConsolidationGroupDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AddConsolidationGroupDto dto)
    {
        try
        {
            var result = await _mediator.Send(new AddConsolidationGroupCmd { AddDto = dto });
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "CreateConsolidationGroup");
        }
    }

    [HttpPut]
    [ProducesResponseType(typeof(ConsolidationGroupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] EditConsolidationGroupDto dto)
    {
        try
        {
            var result = await _mediator.Send(new EditConsolidationGroupCmd { EditDto = dto });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "UpdateConsolidationGroup", dto.Id);
        }
    }

    [HttpPost("{id}/run")]
    [ProducesResponseType(typeof(ConsolidationGroupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RunConsolidation(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new RunConsolidationCmd { Id = id });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "RunConsolidation", id);
        }
    }

    [HttpGet("{id}/results")]
    [ProducesResponseType(typeof(ConsolidationGroupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetResults(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetConsolidationResultsQry { Id = id });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetConsolidationResults", id);
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteConsolidationGroupCmd { Id = id });
            if (!result)
                return NotFound(new { message = $"Consolidation group with ID '{id}' not found" });
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex, "DeleteConsolidationGroup", id);
        }
    }
}