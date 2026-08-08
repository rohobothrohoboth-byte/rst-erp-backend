// Controllers/BudgetCategoryController.cs
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
public class BudgetCategoryController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ILogger<BudgetCategoryController> _logger;

    public BudgetCategoryController(IMediator mediator, ILogger<BudgetCategoryController> logger)
        : base(mediator, logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all budget categories
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<BudgetCategoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool? isActive)
    {
        try
        {
            var result = await _mediator.Send(new GetAllBudgetCategoriesQry
            {
                IsActive = isActive
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetAllBudgetCategories");
        }
    }

    /// <summary>
    /// Get budget category by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BudgetCategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetBudgetCategoryByIdQry { Id = id });
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetBudgetCategoryById", id);
        }
    }

    /// <summary>
    /// Create a new budget category
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(BudgetCategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AddBudgetCategoryDto dto)
    {
        try
        {
            var result = await _mediator.Send(new AddBudgetCategoryCmd { AddDto = dto });
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "CreateBudgetCategory");
        }
    }

    /// <summary>
    /// Update a budget category
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(BudgetCategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] EditBudgetCategoryDto dto)
    {
        try
        {
            var result = await _mediator.Send(new EditBudgetCategoryCmd { EditDto = dto });
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
            return HandleException(ex, "UpdateBudgetCategory", dto.Id);
        }
    }

    /// <summary>
    /// Delete a budget category
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteBudgetCategoryCmd { Id = id });
            if (!result)
                return NotFound(new { message = $"Budget category with ID '{id}' not found" });
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex, "DeleteBudgetCategory", id);
        }
    }
}