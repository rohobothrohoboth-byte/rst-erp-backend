// Controllers/ProfitCenterController.cs
using Cor.Finance.Commands;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace Cor.Finance.Controllers;

[ApiController]
[Route("api/finance/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class ProfitCenterController : BaseApiController
{
    private readonly IMediator _mediator;

    public ProfitCenterController(IMediator mediator, ILogger<ProfitCenterController> logger)
        : base(mediator, logger)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ProfitCenterDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] bool? isActive)
    {
        try
        {
            var result = await _mediator.Send(new GetAllProfitCentersQry { IsActive = isActive });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetAllProfitCenters");
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProfitCenterDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetProfitCenterByIdQry { Id = id });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetProfitCenterById", id);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProfitCenterDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AddProfitCenterDto dto)
    {
        try
        {
            var result = await _mediator.Send(new AddProfitCenterCmd { AddDto = dto });
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "CreateProfitCenter");
        }
    }

    [HttpPut]
    [ProducesResponseType(typeof(ProfitCenterDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] EditProfitCenterDto dto)
    {
        try
        {
            var result = await _mediator.Send(new EditProfitCenterCmd { EditDto = dto });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "UpdateProfitCenter", dto.Id);
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteProfitCenterCmd { Id = id });
            if (!result)
                return NotFound(new { message = $"Profit center with ID '{id}' not found" });
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex, "DeleteProfitCenter", id);
        }
    }
}