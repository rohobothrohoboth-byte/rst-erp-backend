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
public class VendorEvaluationController : BaseApiController
{
    private readonly IMediator _mediator;

    public VendorEvaluationController(IMediator mediator, ILogger<VendorEvaluationController> logger)
        : base(logger)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<VendorEvaluationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? vendorId = null,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        try
        {
            var query = new GetAllVendorEvaluationsQuery
            {
                VendorId = vendorId,
                Status = status,
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
    [ProducesResponseType(typeof(VendorEvaluationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var query = new GetVendorEvaluationByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetById));
        }
    }

    [HttpGet("by-vendor/{vendorId}")]
    [ProducesResponseType(typeof(List<VendorEvaluationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByVendor(Guid vendorId)
    {
        try
        {
            var query = new GetVendorEvaluationsByVendorQuery { VendorId = vendorId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetByVendor));
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(VendorEvaluationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateVendorEvaluationDto createDto)
    {
        try
        {
            var command = new CreateVendorEvaluationCommand { CreateDto = createDto };
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Create));
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var command = new DeleteVendorEvaluationCommand { Id = id };
            var result = await _mediator.Send(command);
            if (!result)
                return NotFound(new { message = $"Evaluation with ID '{id}' not found" });
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Delete));
        }
    }
}