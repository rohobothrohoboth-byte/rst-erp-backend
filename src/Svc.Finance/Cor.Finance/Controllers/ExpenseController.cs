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
public class ExpenseController : BaseApiController
{
    private readonly IMediator _mediator;

    public ExpenseController(IMediator mediator, ILogger<ExpenseController> logger)
        : base(mediator, logger)
    {
        _mediator = mediator;
    }

  // Controllers/ExpenseController.cs

  [HttpGet("All")]
  [ProducesResponseType(typeof(List<ExpenseDto>), StatusCodes.Status200OK)]
  [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "fromDate", "toDate", "status", "branchId", "departmentId", "minAmount", "maxAmount" })]
  public async Task<IActionResult> GetAll(
      [FromQuery] DateTime? fromDate = null,
      [FromQuery] DateTime? toDate = null,
      [FromQuery] string? status = null,
      [FromQuery] Guid? branchId = null,
      [FromQuery] Guid? departmentId = null,
      [FromQuery] decimal? minAmount = null,
      [FromQuery] decimal? maxAmount = null)
  {
      try
      {
          var result = await Mediator.Send(new GetAllExpensesQry
          {
              FromDate = fromDate,
              ToDate = toDate,
              Status = status,
              BranchId = branchId,
              DepartmentId = departmentId,
              MinAmount = minAmount,
              MaxAmount = maxAmount
          });

          return Ok(result);
      }
      catch (Exception ex)
      {
          return HandleException(ex, "GetAllExpenses");
      }
  }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ExpenseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetExpenseByIdQry { Id = id });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetExpenseById", id);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ExpenseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AddExpenseDto dto)
    {
        try
        {
            if (dto.PeriodId == Guid.Empty)
                return HandleBadRequest("PeriodId is required");

            var result = await _mediator.Send(new AddExpenseCmd { AddDto = dto });
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, new
            {
                success = true,
                message = "Expense created successfully",
                data = result
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "CreateExpense");
        }
    }

    [HttpPost("Bulk")]
    [ProducesResponseType(typeof(List<ExpenseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BulkCreate([FromBody] List<AddExpenseDto> dtos)
    {
        try
        {
            foreach (var dto in dtos)
            {
                if (dto.PeriodId == Guid.Empty)
                    return HandleBadRequest("PeriodId is required for all expenses");
            }

            var result = await _mediator.Send(new BulkAddExpenseCmd { AddDtos = dtos });
            return Ok(new { success = true, message = "Expenses created successfully", data = result });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "BulkCreateExpenses");
        }
    }

    [HttpPut]
    [ProducesResponseType(typeof(ExpenseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] EditExpenseDto dto)
    {
        try
        {
            if (dto.PeriodId == Guid.Empty)
                return HandleBadRequest("PeriodId is required");

            var result = await _mediator.Send(new EditExpenseCmd { EditDto = dto });
            return Ok(new { success = true, message = "Expense updated successfully", data = result });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "UpdateExpense", dto.Id);
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
            var result = await _mediator.Send(new DeleteExpenseCmd { Id = id });
            if (!result)
                return HandleNotFound("Expense", id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "DeleteExpense", id);
        }
    }

    // ==================== EXPENSE CATEGORIES ====================

    [HttpGet("Categories")]
    [ProducesResponseType(typeof(List<ExpenseCategoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllCategories([FromQuery] bool? isActive)
    {
        try
        {
            var result = await _mediator.Send(new GetAllExpenseCategoriesQry { IsActive = isActive });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetAllExpenseCategories");
        }
    }

    [HttpGet("Category/{id}")]
    [ProducesResponseType(typeof(ExpenseCategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCategoryById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetExpenseCategoryByIdQry { Id = id });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetExpenseCategoryById", id);
        }
    }

    [HttpPost("Category")]
    [ProducesResponseType(typeof(ExpenseCategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCategory([FromBody] AddExpenseCategoryDto dto)
    {
        try
        {
            var result = await _mediator.Send(new AddExpenseCategoryCmd { AddDto = dto });
            return CreatedAtAction(nameof(GetCategoryById), new { id = result.Id }, new
            {
                success = true,
                message = "Expense category created successfully",
                data = result
            });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "CreateExpenseCategory");
        }
    }

    [HttpPut("Category")]
    [ProducesResponseType(typeof(ExpenseCategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCategory([FromBody] EditExpenseCategoryDto dto)
    {
        try        {
            var result = await _mediator.Send(new EditExpenseCategoryCmd { EditDto = dto });
            return Ok(new { success = true, message = "Expense category updated successfully", data = result });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "UpdateExpenseCategory", dto.Id);
        }
    }

    [HttpDelete("Category/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteExpenseCategoryCmd { Id = id });
            if (!result)
                return HandleNotFound("Expense Category", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex, "DeleteExpenseCategory", id);
        }
    }

    [HttpPost("Category/Bulk")]
    [ProducesResponseType(typeof(List<ExpenseCategoryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BulkCreateCategories([FromBody] List<AddExpenseCategoryDto> dtos)
    {
        try
        {
            var result = await _mediator.Send(new BulkAddExpenseCategoryCmd { AddDtos = dtos });
            return Ok(new { success = true, message = "Expense categories created successfully", data = result });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "BulkCreateExpenseCategories");
        }
    }

    [HttpPatch("Category/{id}/toggle-status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleCategoryStatus(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new ToggleExpenseCategoryStatusCmd { Id = id });
            if (!result)
                return HandleNotFound("Expense Category", id);
            return SuccessResponse("Category status toggled successfully");
        }
        catch (Exception ex)
        {
            return HandleException(ex, "ToggleExpenseCategoryStatus", id);
        }
    }
}