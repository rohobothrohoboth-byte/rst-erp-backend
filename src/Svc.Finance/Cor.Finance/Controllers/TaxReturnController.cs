// Controllers/TaxReturnController.cs
using Cor.Finance.Commands;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace Cor.Finance.Controllers;

[ApiController]
[Route("api/finance/v{version:apiVersion}/tax-returns")]
[ApiVersion("1.0")]
[Authorize]
public class TaxReturnController : BaseApiController
{
    private readonly IMediator _mediator;

    public TaxReturnController(IMediator mediator, ILogger<TaxReturnController> logger)
        : base(mediator, logger)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<TaxReturnDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? period,
        [FromQuery] string? fiscalYear,
        [FromQuery] string? status,
        [FromQuery] string? taxType)
    {
        try
        {
            var result = await _mediator.Send(new GetAllTaxReturnsQry
            {
                Period = period,
                FiscalYear = fiscalYear,
                Status = status,
                TaxType = taxType
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetAllTaxReturns");
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TaxReturnDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetTaxReturnByIdQry { Id = id });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetTaxReturnById", id);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(TaxReturnDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AddTaxReturnDto dto)
    {
        try
        {
            var result = await _mediator.Send(new AddTaxReturnCmd { AddDto = dto });
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "CreateTaxReturn");
        }
    }

    [HttpPut]
    [ProducesResponseType(typeof(TaxReturnDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] EditTaxReturnDto dto)
    {
        try
        {
            var result = await _mediator.Send(new EditTaxReturnCmd { EditDto = dto });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "UpdateTaxReturn", dto.Id);
        }
    }

    [HttpPost("{id}/file")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FileTaxReturn(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new FileTaxReturnCmd { Id = id });
            return Ok(new { success = true, message = "Tax return filed successfully", data = result });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "FileTaxReturn", id);
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteTaxReturnCmd { Id = id });
            if (!result)
                return NotFound(new { message = $"Tax return with ID '{id}' not found" });
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex, "DeleteTaxReturn", id);
        }
    }
}