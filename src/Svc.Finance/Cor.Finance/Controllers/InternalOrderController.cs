// Controllers/InternalOrderController.cs
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
public class InternalOrderController : BaseApiController
{
    private readonly IMediator _mediator;

    public InternalOrderController(IMediator mediator, ILogger<InternalOrderController> logger)
        : base(mediator, logger)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<InternalOrderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] string? status, [FromQuery] Guid? costCenterId)
    {
        try
        {
            var result = await _mediator.Send(new GetAllInternalOrdersQry { Status = status, CostCenterId = costCenterId });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetAllInternalOrders");
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(InternalOrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetInternalOrderByIdQry { Id = id });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetInternalOrderById", id);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(InternalOrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AddInternalOrderDto dto)
    {
        try
        {
            var result = await _mediator.Send(new AddInternalOrderCmd { AddDto = dto });
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "CreateInternalOrder");
        }
    }

    [HttpPut]
    [ProducesResponseType(typeof(InternalOrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] EditInternalOrderDto dto)
    {
        try
        {
            var result = await _mediator.Send(new EditInternalOrderCmd { EditDto = dto });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "UpdateInternalOrder", dto.Id);
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteInternalOrderCmd { Id = id });
            if (!result)
                return NotFound(new { message = $"Internal order with ID '{id}' not found" });
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex, "DeleteInternalOrder", id);
        }
    }
}