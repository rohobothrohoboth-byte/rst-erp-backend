using Cor.Finance.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Commands;

namespace Cor.Finance.Controllers;

[ApiController]
[Route("api/finance/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class PurchaseOrderController : BaseApiController
{
    private readonly IMediator _mediator;

    public PurchaseOrderController(IMediator mediator, ILogger<PurchaseOrderController> logger)
        : base(mediator, logger)
    {
        _mediator = mediator;
    }

    [HttpGet("All")]
    [ProducesResponseType(typeof(List<PurchaseOrderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? vendorId,
        [FromQuery] string? status,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] Guid? periodId)
    {
        try
        {
            var result = await _mediator.Send(new GetAllPurchaseOrdersQry
            {
                VendorId = vendorId,
                Status = status,
                FromDate = fromDate,
                ToDate = toDate,
                PeriodId = periodId
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetAllPurchaseOrders");
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PurchaseOrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetPurchaseOrderByIdQry { Id = id });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetPurchaseOrderById", id);
        }
    }

    [HttpGet("ByVendor/{vendorId}")]
    [ProducesResponseType(typeof(List<PurchaseOrderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByVendor(Guid vendorId)
    {
        try
        {
            var result = await _mediator.Send(new GetPurchaseOrdersByVendorQry { VendorId = vendorId });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetPurchaseOrdersByVendor", vendorId);
        }
    }

    [HttpGet("ByNumber/{purchaseOrderNumber}")]
    [ProducesResponseType(typeof(PurchaseOrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByNumber(string purchaseOrderNumber)
    {
        try
        {
            var result = await _mediator.Send(new GetPurchaseOrderByNumberQry { PurchaseOrderNumber = purchaseOrderNumber });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetPurchaseOrderByNumber", purchaseOrderNumber);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(PurchaseOrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AddPurchaseOrderDto dto)
    {
        try
        {
            if (dto.PeriodId == Guid.Empty)
                return HandleBadRequest("PeriodId is required");

            var result = await _mediator.Send(new AddPurchaseOrderCmd { AddDto = dto });
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, new
            {
                success = true,
                message = "Purchase order created successfully",
                data = result
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "CreatePurchaseOrder");
        }
    }

    [HttpPut]
    [ProducesResponseType(typeof(PurchaseOrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] EditPurchaseOrderDto dto)
    {
        try
        {
            if (dto.PeriodId == Guid.Empty)
                return HandleBadRequest("PeriodId is required");

            var result = await _mediator.Send(new EditPurchaseOrderCmd { EditDto = dto });
            return Ok(new { success = true, message = "Purchase order updated successfully", data = result });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "UpdatePurchaseOrder", dto.Id);
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new DeletePurchaseOrderCmd { Id = id });
            if (!result)
                return HandleNotFound("Purchase Order", id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "DeletePurchaseOrder", id);
        }
    }

    [HttpPost("{id}/receive")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Receive(Guid id, [FromBody] ReceivePurchaseOrderDto dto)
    {
        try
        {
            var result = await _mediator.Send(new ReceivePurchaseOrderCmd
            {
                Id = id,
               ReceivedDate = dto.ReceivedDate ?? DateTime.UtcNow,
                ReceivedBy = dto.ReceivedBy
            });
            if (!result)
                return HandleNotFound("Purchase Order", id);
            return SuccessResponse("Purchase order received successfully");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "ReceivePurchaseOrder", id);
        }
    }
}