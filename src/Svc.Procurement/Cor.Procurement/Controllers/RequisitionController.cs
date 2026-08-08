// Controllers/RequisitionController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cor.Procurement.Commands;
using Cor.Procurement.Queries;
using Cor.Procurement.Models.DTOs;
using Asp.Versioning;
using Microsoft.Extensions.Logging;

namespace Cor.Procurement.Controllers;

[ApiController]
[Route("api/procurement/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class RequisitionController : BaseApiController
{
    private readonly IMediator _mediator;

    public RequisitionController(IMediator mediator, ILogger<RequisitionController> logger)
        : base(logger)
    {
        _mediator = mediator;
    }

    // ============================================================
    // GET ENDPOINTS
    // ============================================================

    /// <summary>
    /// Get all requisitions with filtering
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<RequisitionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? status = null,
        [FromQuery] string? priority = null,
        [FromQuery] Guid? departmentId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? searchTerm = null)
    {
        try
        {
            var query = new GetAllRequisitionsQuery
            {
                Status = status,
                Priority = priority,
                DepartmentId = departmentId,
                FromDate = fromDate,
                ToDate = toDate,
                SearchTerm = searchTerm
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetAll));
        }
    }

    /// <summary>
    /// Get all requisitions with pagination
    /// </summary>
    [HttpGet("All")]
    [ProducesResponseType(typeof(PagedResult<RequisitionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null,
        [FromQuery] string? priority = null,
        [FromQuery] Guid? departmentId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = "SubmittedDate",
        [FromQuery] string? sortOrder = "DESC")
    {
        try
        {
            var query = new GetPagedRequisitionsQuery
            {
                Page = page,
                PageSize = pageSize,
                Status = status,
                Priority = priority,
                DepartmentId = departmentId,
                FromDate = fromDate,
                ToDate = toDate,
                SearchTerm = searchTerm,
                SortBy = sortBy,
                SortOrder = sortOrder
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetPaged));
        }
    }

    /// <summary>
    /// Get requisition by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RequisitionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var query = new GetRequisitionByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            if (result == null)
                return NotFound(new { message = $"Requisition with ID '{id}' not found" });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetById));
        }
    }

    /// <summary>
    /// Get requisitions by status
    /// </summary>
    [HttpGet("ByStatus/{status}")]
    [ProducesResponseType(typeof(List<RequisitionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByStatus(string status)
    {
        try
        {
            var query = new GetRequisitionsByStatusQuery { Status = status };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetByStatus));
        }
    }

    /// <summary>
    /// Get requisitions pending approval
    /// </summary>
    [HttpGet("PendingApproval")]
    [ProducesResponseType(typeof(List<RequisitionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPendingApproval()
    {
        try
        {
            // Use a default approver ID or get from token
            var approverId = GetCurrentUserId();
            var query = new GetRequisitionsForApprovalQuery { ApproverId = approverId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetPendingApproval));
        }
    }

    /// <summary>
    /// Get requisition summary
    /// </summary>
    [HttpGet("Summary")]
    [ProducesResponseType(typeof(RequisitionSummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummary(
        [FromQuery] Guid? departmentId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        try
        {
            var query = new GetRequisitionSummaryQuery
            {
                DepartmentId = departmentId,
                FromDate = fromDate,
                ToDate = toDate
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetSummary));
        }
    }

    // ============================================================
    // POST ENDPOINTS
    // ============================================================

    /// <summary>
    /// Create a new requisition
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(RequisitionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateRequisitionDto createDto)
    {
        try
        {
            if (createDto == null)
                return BadRequest(new { message = "Requisition data is required" });

            if (createDto.Lines == null || !createDto.Lines.Any())
                return BadRequest(new { message = "At least one line item is required" });

            var command = new CreateRequisitionCommand { CreateDto = createDto };
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Create));
        }
    }

    // ============================================================
    // PUT ENDPOINTS
    // ============================================================

    /// <summary>
    /// Update a requisition
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(RequisitionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] UpdateRequisitionDto updateDto)
    {
        try
        {
            if (updateDto == null)
                return BadRequest(new { message = "Update data is required" });

            if (updateDto.Lines == null || !updateDto.Lines.Any())
                return BadRequest(new { message = "At least one line item is required" });

            var command = new UpdateRequisitionCommand { UpdateDto = updateDto };
            var result = await _mediator.Send(command);
            if (result == null)
                return NotFound(new { message = $"Requisition with ID '{updateDto.Id}' not found" });
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Update));
        }
    }

    // ============================================================
    // PATCH ENDPOINTS
    // ============================================================

    /// <summary>
    /// Submit a requisition for approval
    /// </summary>
    [HttpPatch("{id}/submit")]
    [ProducesResponseType(typeof(RequisitionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Submit(Guid id)
    {
        try
        {
            var command = new SubmitRequisitionCommand { Id = id };
            var result = await _mediator.Send(command);
            if (result == null)
                return NotFound(new { message = $"Requisition with ID '{id}' not found" });
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Submit));
        }
    }

    // ============================================================
    // POST ENDPOINTS (Approval)
    // ============================================================

    /// <summary>
    /// Approve a requisition
    /// </summary>
    [HttpPost("{id}/approve")]
    [ProducesResponseType(typeof(RequisitionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Approve(Guid id, [FromBody] RequisitionApprovalActionDto? actionDto = null)
    {
        try
        {
            var dto = actionDto ?? new RequisitionApprovalActionDto();
            dto.RequisitionId = id;
            dto.Action = "Approve";

            var command = new ApproveRequisitionCommand { ActionDto = dto };
            var result = await _mediator.Send(command);
            if (result == null)
                return NotFound(new { message = $"Requisition with ID '{id}' not found" });
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Approve));
        }
    }

    /// <summary>
    /// Reject a requisition
    /// </summary>
    [HttpPost("{id}/reject")]
    [ProducesResponseType(typeof(RequisitionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RequisitionApprovalActionDto actionDto)
    {
        try
        {
            if (actionDto == null || string.IsNullOrEmpty(actionDto.RejectionReason))
                return BadRequest(new { message = "Rejection reason is required" });

            actionDto.RequisitionId = id;
            actionDto.Action = "Reject";

            var command = new ApproveRequisitionCommand { ActionDto = actionDto };
            var result = await _mediator.Send(command);
            if (result == null)
                return NotFound(new { message = $"Requisition with ID '{id}' not found" });
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Reject));
        }
    }

    /// <summary>
    /// Create purchase order from requisition
    /// </summary>
    [HttpPost("{id}/create-po")]
    [ProducesResponseType(typeof(PurchaseOrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreatePurchaseOrder(Guid id)
    {
        try
        {
            var command = new CreatePurchaseOrderFromRequisitionCommand { RequisitionId = id };
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(PurchaseOrderController.GetById), "PurchaseOrder", new { id = result.Id }, result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(CreatePurchaseOrder));
        }
    }

    // ============================================================
    // DELETE ENDPOINTS
    // ============================================================

    /// <summary>
    /// Delete a requisition
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var command = new DeleteRequisitionCommand { Id = id };
            var result = await _mediator.Send(command);
            if (!result)
                return NotFound(new { message = $"Requisition with ID '{id}' not found" });
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Delete));
        }
    }

    // ============================================================
    // HELPER METHODS
    // ============================================================

    private Guid GetCurrentUserId()
    {
        // Get user ID from claims
        var userIdClaim = User?.FindFirst("userId")?.Value ??
                         User?.FindFirst("sub")?.Value;

        if (Guid.TryParse(userIdClaim, out var userId))
            return userId;

        return Guid.Empty;
    }
}