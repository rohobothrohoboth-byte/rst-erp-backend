// Controllers/BudgetCodeController.cs
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
public class BudgetCodeController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ILogger<BudgetCodeController> _logger;

    public BudgetCodeController(IMediator mediator, ILogger<BudgetCodeController> logger)
        : base(mediator, logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all budget codes
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<BudgetCodeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool? isActive,
        [FromQuery] string? budgetType,
        [FromQuery] string? fiscalYear)
    {
        try
        {
            var result = await _mediator.Send(new GetAllBudgetCodesQry
            {
                IsActive = isActive,
                BudgetType = budgetType,
                FiscalYear = fiscalYear
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetAllBudgetCodes");
        }
    }

    /// <summary>
    /// Get budget code by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BudgetCodeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetBudgetCodeByIdQry { Id = id });
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetBudgetCodeById", id);
        }
    }

    /// <summary>
    /// Create a new budget code
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(BudgetCodeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AddBudgetCodeDto dto)
    {
        try
        {
            var result = await _mediator.Send(new AddBudgetCodeCmd { AddDto = dto });
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "CreateBudgetCode");
        }
    }

    /// <summary>
    /// Update a budget code
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(BudgetCodeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] EditBudgetCodeDto dto)
    {
        try
        {
            var result = await _mediator.Send(new EditBudgetCodeCmd { EditDto = dto });
            return Ok(result);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "UpdateBudgetCode", dto.Id);
        }
    }

    /// <summary>
    /// Delete a budget code
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteBudgetCodeCmd { Id = id });
            if (!result)
                return NotFound(new { message = $"Budget code with ID '{id}' not found" });
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex, "DeleteBudgetCode", id);
        }
    }
}