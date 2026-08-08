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
public class InspectionController : BaseApiController
{
    private readonly IMediator _mediator;

    public InspectionController(IMediator mediator, ILogger<InspectionController> logger)
        : base(logger)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<InspectionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? status = null,
        [FromQuery] string? inspectorId = null,
        [FromQuery] Guid? grnId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        try
        {
            var query = new GetAllInspectionsQuery
            {
                Status = status,
                InspectorId = inspectorId,
                GrnId = grnId,
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
    [ProducesResponseType(typeof(InspectionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var query = new GetInspectionByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            if (result == null)
                return NotFound(new { message = $"Inspection with ID '{id}' not found" });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetById));
        }
    }

    [HttpGet("by-grn/{grnId}")]
    [ProducesResponseType(typeof(List<InspectionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByGrn(Guid grnId)
    {
        try
        {
            var query = new GetInspectionsByGrnQuery { GrnId = grnId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetByGrn));
        }
    }

    [HttpPost("complete")]
    [ProducesResponseType(typeof(InspectionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Complete([FromBody] CompleteInspectionDto dto)
    {
        try
        {
            var command = new CompleteInspectionCommand { CompleteDto = dto };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Complete));
        }
    }
}