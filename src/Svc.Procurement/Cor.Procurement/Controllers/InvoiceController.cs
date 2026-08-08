using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cor.Procurement.Commands;
using Cor.Procurement.Queries;
using Cor.Procurement.Models.DTOs;

namespace Cor.Procurement.Controllers;

[ApiController]
[Route("api/procurement/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class InvoiceController : BaseApiController
{
    private readonly IMediator _mediator;

    public InvoiceController(IMediator mediator, ILogger<InvoiceController> logger)
        : base(logger)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<InvoiceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? status = null,
        [FromQuery] Guid? vendorId = null,
        [FromQuery] Guid? purchaseOrderId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        try
        {
            var query = new GetAllInvoicesQuery
            {
                Status = status,
                VendorId = vendorId,
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
[HttpGet("search")]
    [ProducesResponseType(typeof(List<InvoiceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? status = null)
    {
        try
        {
            var query = new SearchInvoicesQuery
            {
                SearchTerm = searchTerm,
                Status = status
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Search));
        }
    }
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var query = new GetInvoiceByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            if (result == null)
                return NotFound(new { message = $"Invoice with ID '{id}' not found" });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetById));
        }
    }

    [HttpGet("by-po/{purchaseOrderId}")]
    [ProducesResponseType(typeof(List<InvoiceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByPurchaseOrder(Guid purchaseOrderId)
    {
        try
        {
            var query = new GetInvoicesByPurchaseOrderQuery { PurchaseOrderId = purchaseOrderId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetByPurchaseOrder));
        }
    }

    [HttpGet("by-vendor/{vendorId}")]
    [ProducesResponseType(typeof(List<InvoiceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByVendor(Guid vendorId)
    {
        try
        {
            var query = new GetInvoicesByVendorQuery { VendorId = vendorId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetByVendor));
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceDto createDto)
    {
        try
        {
            var command = new CreateInvoiceCommand { CreateDto = createDto };
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Create));
        }
    }

    [HttpPatch("status")]
    [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateStatus([FromBody] InvoiceStatusUpdateDto statusDto)
    {
        try
        {
            var command = new UpdateInvoiceStatusCommand { StatusDto = statusDto };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(UpdateStatus));
        }
    }
}