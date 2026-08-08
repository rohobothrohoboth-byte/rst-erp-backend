using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Cor.Procurement.Commands;
using Cor.Procurement.Queries;
using Cor.Procurement.Models.DTOs;

namespace Cor.Procurement.Controllers;

[ApiController]
[Route("api/procurement/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class GoodsReceiptNoteController : BaseApiController
{
    private readonly IMediator _mediator;

    public GoodsReceiptNoteController(IMediator mediator, ILogger<GoodsReceiptNoteController> logger)
        : base(logger)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<GoodsReceiptNoteDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? status = null,
        [FromQuery] Guid? purchaseOrderId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        try
        {
            var query = new GetAllGoodsReceiptNotesQuery
            {
                Status = status,
                PurchaseOrderId = purchaseOrderId,
                FromDate = fromDate,
                ToDate = toDate
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetAll));
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GoodsReceiptNoteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var query = new GetGoodsReceiptNoteByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            if (result == null)
                return NotFound(new { message = $"GRN with ID '{id}' not found" });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetById));
        }
    }

    [HttpGet("by-po/{purchaseOrderId}")]
    [ProducesResponseType(typeof(List<GoodsReceiptNoteDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByPurchaseOrder(Guid purchaseOrderId)
    {
        try
        {
            var query = new GetGoodsReceiptNotesByPurchaseOrderQuery { PurchaseOrderId = purchaseOrderId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetByPurchaseOrder));
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(GoodsReceiptNoteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateGoodsReceiptNoteDto createDto)
    {
        try
        {
            var command = new CreateGoodsReceiptNoteCommand { CreateDto = createDto };
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Create));
        }
    }

    [HttpPut]
    [ProducesResponseType(typeof(GoodsReceiptNoteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] UpdateGoodsReceiptNoteDto updateDto)
    {
        try
        {
            var command = new UpdateGoodsReceiptNoteCommand { UpdateDto = updateDto };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Update));
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var command = new DeleteGoodsReceiptNoteCommand { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Delete));
        }
    }

    [HttpPatch("{id}/complete")]
    [ProducesResponseType(typeof(GoodsReceiptNoteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Complete(Guid id)
    {
        try
        {
            var command = new CompleteGoodsReceiptNoteCommand { Id = id };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Complete));
        }
    }
}