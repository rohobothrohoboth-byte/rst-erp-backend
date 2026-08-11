using Asp.Versioning;
using Common;
using Cor.Inventory.Models.DTOs;
using Cor.Inventory.Services;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cor.Inventory.Controllers;

[ApiController]
[Route("api/inventory/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class ReorderController : ControllerBase
{
    private readonly IReorderService _reorderService;
    private readonly ILogger<ReorderController> _logger;

    public ReorderController(IReorderService reorderService, ILogger<ReorderController> logger)
    {
        _reorderService = reorderService;
        _logger = logger;
    }

    // ============= Levels (rules) =============

    [HttpGet("levels")]
    [PerAuth("inv.reorder.level.view")]
    [ProducesResponseType(typeof(ApiResponse<List<ReorderRuleDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLevels()
    {
        try
        {
            var rules = await _reorderService.GetRulesAsync();
            return Ok(ApiResponse<List<ReorderRuleDto>>.Ok(rules));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting reorder rules");
            return StatusCode(500, ApiResponse<List<ReorderRuleDto>>.Fail("Failed to get reorder rules", statusCode: 500));
        }
    }

    [HttpPost("levels")]
    [PerAuth("inv.reorder.level.mod")]
    [ProducesResponseType(typeof(ApiResponse<ReorderRuleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateLevel([FromBody] CreateReorderRuleDto dto)
    {
        try
        {
            var rule = await _reorderService.CreateRuleAsync(dto);
            return Ok(ApiResponse<ReorderRuleDto>.Ok(rule, "Reorder rule created"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<ReorderRuleDto>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating reorder rule");
            return StatusCode(500, ApiResponse<ReorderRuleDto>.Fail("Failed to create reorder rule", statusCode: 500));
        }
    }

    [HttpPut("levels/{id}")]
    [PerAuth("inv.reorder.level.mod")]
    [ProducesResponseType(typeof(ApiResponse<ReorderRuleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateLevel(Guid id, [FromBody] UpdateReorderRuleDto dto)
    {
        try
        {
            dto.Id = id;
            var rule = await _reorderService.UpdateRuleAsync(dto);
            return Ok(ApiResponse<ReorderRuleDto>.Ok(rule, "Reorder rule updated"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<ReorderRuleDto>.Fail(ex.Message, statusCode: 404));
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict(ApiResponse<ReorderRuleDto>.Fail("The reorder rule was modified by another user", statusCode: 409));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating reorder rule {Id}", id);
            return StatusCode(500, ApiResponse<ReorderRuleDto>.Fail("Failed to update reorder rule", statusCode: 500));
        }
    }

    // ============= Alerts =============

    [HttpGet("alerts")]
    [PerAuth("inv.reorder.level.alert")]
    [ProducesResponseType(typeof(ApiResponse<List<ReorderAlertDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAlerts()
    {
        try
        {
            var alerts = await _reorderService.GetAlertsAsync();
            return Ok(ApiResponse<List<ReorderAlertDto>>.Ok(alerts));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting reorder alerts");
            return StatusCode(500, ApiResponse<List<ReorderAlertDto>>.Fail("Failed to get reorder alerts", statusCode: 500));
        }
    }

    // ============= Requests =============

    [HttpGet("requests")]
    [PerAuth("inv.reorder.request.view")]
    [ProducesResponseType(typeof(ApiResponse<List<ReorderRequestDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRequests([FromQuery] string? status)
    {
        try
        {
            var requests = await _reorderService.GetRequestsAsync(status);
            return Ok(ApiResponse<List<ReorderRequestDto>>.Ok(requests));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting reorder requests");
            return StatusCode(500, ApiResponse<List<ReorderRequestDto>>.Fail("Failed to get reorder requests", statusCode: 500));
        }
    }

    [HttpPost("requests")]
    [PerAuth("inv.reorder.request.create")]
    [ProducesResponseType(typeof(ApiResponse<ReorderRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateRequest([FromBody] CreateReorderRequestDto dto)
    {
        try
        {
            var request = await _reorderService.CreateRequestAsync(dto);
            return Ok(ApiResponse<ReorderRequestDto>.Ok(request, "Reorder request created"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<ReorderRequestDto>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating reorder request");
            return StatusCode(500, ApiResponse<ReorderRequestDto>.Fail("Failed to create reorder request", statusCode: 500));
        }
    }

    [HttpPut("requests/{id}/approve")]
    [PerAuth("inv.reorder.request.approve")]
    [ProducesResponseType(typeof(ApiResponse<ReorderRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveRequest(Guid id, [FromBody] ReorderDecisionDto? dto)
    {
        try
        {
            var request = await _reorderService.ApproveRequestAsync(id, dto ?? new ReorderDecisionDto());
            return Ok(ApiResponse<ReorderRequestDto>.Ok(request, "Reorder request approved"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<ReorderRequestDto>.Fail(ex.Message, statusCode: 404));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<ReorderRequestDto>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving reorder request {Id}", id);
            return StatusCode(500, ApiResponse<ReorderRequestDto>.Fail("Failed to approve reorder request", statusCode: 500));
        }
    }

    [HttpPut("requests/{id}/reject")]
    [PerAuth("inv.reorder.request.reject")]
    [ProducesResponseType(typeof(ApiResponse<ReorderRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RejectRequest(Guid id, [FromBody] ReorderDecisionDto? dto)
    {
        try
        {
            var request = await _reorderService.RejectRequestAsync(id, dto ?? new ReorderDecisionDto());
            return Ok(ApiResponse<ReorderRequestDto>.Ok(request, "Reorder request rejected"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<ReorderRequestDto>.Fail(ex.Message, statusCode: 404));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<ReorderRequestDto>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting reorder request {Id}", id);
            return StatusCode(500, ApiResponse<ReorderRequestDto>.Fail("Failed to reject reorder request", statusCode: 500));
        }
    }

    [HttpPut("requests/{id}/convert")]
    [PerAuth("inv.reorder.request.convert")]
    [ProducesResponseType(typeof(ApiResponse<ReorderRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConvertRequest(Guid id, [FromBody] ReorderDecisionDto? dto)
    {
        try
        {
            var request = await _reorderService.ConvertRequestAsync(id, dto ?? new ReorderDecisionDto());
            return Ok(ApiResponse<ReorderRequestDto>.Ok(request, "Reorder request converted"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<ReorderRequestDto>.Fail(ex.Message, statusCode: 404));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<ReorderRequestDto>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting reorder request {Id}", id);
            return StatusCode(500, ApiResponse<ReorderRequestDto>.Fail("Failed to convert reorder request", statusCode: 500));
        }
    }
}
