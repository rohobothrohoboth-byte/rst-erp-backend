using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cor.PlanDev.Commands;
using Cor.PlanDev.Queries;
using Cor.PlanDev.Models.DTOs;

namespace Cor.PlanDev.Controllers;

[ApiController]
[Route("api/plandev/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class BudgetController : BaseApiController
{
    private readonly IMediator _mediator;

    public BudgetController(IMediator mediator, ILogger<BudgetController> logger)
        : base(logger)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all budgets for a project
    /// </summary>
    [HttpGet("by-project/{projectId}")]
    [ProducesResponseType(typeof(List<BudgetDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByProject(Guid projectId)
    {
        try
        {
            var query = new GetBudgetsByProjectQuery { ProjectId = projectId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetByProject));
        }
    }

    /// <summary>
    /// Get budget by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BudgetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var query = new GetBudgetByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            if (result == null)
                return NotFound(new { message = $"Budget with ID '{id}' not found" });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetById));
        }
    }

    /// <summary>
    /// Get budget summary for a project
    /// </summary>
    [HttpGet("summary/{projectId}")]
    [ProducesResponseType(typeof(BudgetSummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummary(Guid projectId)
    {
        try
        {
            var query = new GetBudgetSummaryQuery { ProjectId = projectId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetSummary));
        }
    }

    /// <summary>
    /// Get budgets by category
    /// </summary>
    [HttpGet("by-category/{projectId}/{category}")]
    [ProducesResponseType(typeof(List<BudgetDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCategory(Guid projectId, string category)
    {
        try
        {
            var query = new GetBudgetByCategoryQuery
            {
                ProjectId = projectId,
                Category = category
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetByCategory));
        }
    }

    /// <summary>
    /// Create a new budget
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(BudgetDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateBudgetDto createDto)
    {
        try
        {
            var command = new CreateBudgetCommand { CreateDto = createDto };
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Create));
        }
    }

    /// <summary>
    /// Update an existing budget
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(BudgetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] UpdateBudgetDto updateDto)
    {
        try
        {
            var command = new UpdateBudgetCommand { UpdateDto = updateDto };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Update));
        }
    }

    /// <summary>
    /// Delete a budget
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var command = new DeleteBudgetCommand { Id = id };
            var result = await _mediator.Send(command);
            if (!result)
                return NotFound(new { message = $"Budget with ID '{id}' not found" });
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Delete));
        }
    }

    /// <summary>
    /// Update budget status
    /// </summary>
    [HttpPatch("status")]
    [ProducesResponseType(typeof(BudgetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateBudgetStatusCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(UpdateStatus));
        }
    }

    /// <summary>
    /// Bulk create budgets
    /// </summary>
    [HttpPost("bulk")]
    [ProducesResponseType(typeof(List<BudgetDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BulkCreate([FromBody] BulkCreateBudgetsCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetByProject), new { projectId = command.Budgets.FirstOrDefault()?.ProjectId }, result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(BulkCreate));
        }
    }
}