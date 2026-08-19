using Asp.Versioning;
using Common;
using Cor.Inventory.Models.DTOs;
using Cor.Inventory.Services;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cor.Inventory.Controllers;

[ApiController]
[Route("api/inventory/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class MaterialRequestController : ControllerBase
{
    private readonly IMaterialService _materialService;
    private readonly ILogger<MaterialRequestController> _logger;

    public MaterialRequestController(IMaterialService materialService, ILogger<MaterialRequestController> logger)
    {
        _materialService = materialService;
        _logger = logger;
    }

    [HttpPost("my")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<MaterialRequestDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateMyRequest([FromBody] CreateMaterialRequestDto dto)
    {
        if (!TryGetEmployeeId(out var employeeId, out var error))
            return error!;

        try
        {
            var employeeName = User.Identity?.Name;
            var request = await _materialService.CreateRequest(employeeId, employeeName, dto);
            return CreatedAtAction(nameof(GetMyRequests), new { version = "1.0" },
                ApiResponse<MaterialRequestDto>.Ok(request, "Material request created"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating material request");
            return StatusCode(500, ApiResponse<MaterialRequestDto>.Fail("Failed to create material request", statusCode: 500));
        }
    }

    [HttpGet("my")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<List<MaterialRequestDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyRequests()
    {
        if (!TryGetEmployeeId(out var employeeId, out var error))
            return error!;

        try
        {
            var requests = await _materialService.GetMyRequests(employeeId);
            return Ok(ApiResponse<List<MaterialRequestDto>>.Ok(requests));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting my material requests");
            return StatusCode(500, ApiResponse<List<MaterialRequestDto>>.Fail("Failed to get material requests", statusCode: 500));
        }
    }

    [HttpGet]
    [PerAuth("inv.stock.outbound.view")]
    [ProducesResponseType(typeof(ApiResponse<List<MaterialRequestDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var requests = await _materialService.GetAllRequests();
            return Ok(ApiResponse<List<MaterialRequestDto>>.Ok(requests));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting material requests");
            return StatusCode(500, ApiResponse<List<MaterialRequestDto>>.Fail("Failed to get material requests", statusCode: 500));
        }
    }

    [HttpPut("{id}/approve")]
    [PerAuth("inv.stock.outbound.approve")]
    [ProducesResponseType(typeof(ApiResponse<MaterialRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Approve(Guid id, [FromBody] MaterialRequestDecisionDto dto)
    {
        try
        {
            var (userId, userName) = GetDecider();
            var request = await _materialService.ApproveRequest(id, userId, userName, dto);
            return Ok(ApiResponse<MaterialRequestDto>.Ok(request, "Material request approved"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<MaterialRequestDto>.Fail(ex.Message, statusCode: 404));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving material request {Id}", id);
            return StatusCode(500, ApiResponse<MaterialRequestDto>.Fail("Failed to approve material request", statusCode: 500));
        }
    }

    [HttpPut("{id}/reject")]
    [PerAuth("inv.stock.outbound.approve")]
    [ProducesResponseType(typeof(ApiResponse<MaterialRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reject(Guid id, [FromBody] MaterialRequestDecisionDto dto)
    {
        try
        {
            var (userId, userName) = GetDecider();
            var request = await _materialService.RejectRequest(id, userId, userName, dto);
            return Ok(ApiResponse<MaterialRequestDto>.Ok(request, "Material request rejected"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<MaterialRequestDto>.Fail(ex.Message, statusCode: 404));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting material request {Id}", id);
            return StatusCode(500, ApiResponse<MaterialRequestDto>.Fail("Failed to reject material request", statusCode: 500));
        }
    }

    [HttpPut("{id}/issue")]
    [PerAuth("inv.stock.outbound.add")]
    [ProducesResponseType(typeof(ApiResponse<MaterialAssignmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Issue(Guid id, [FromBody] MaterialRequestDecisionDto dto)
    {
        try
        {
            var (userId, userName) = GetDecider();
            var assignment = await _materialService.IssueRequest(id, userId, userName, dto);
            return Ok(ApiResponse<MaterialAssignmentDto>.Ok(assignment, "Material issued"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<MaterialAssignmentDto>.Fail(ex.Message, statusCode: 404));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error issuing material request {Id}", id);
            return StatusCode(500, ApiResponse<MaterialAssignmentDto>.Fail("Failed to issue material request", statusCode: 500));
        }
    }

    private bool TryGetEmployeeId(out Guid employeeId, out IActionResult? error)
    {
        employeeId = Guid.Empty;
        error = null;

        var raw = User.FindFirst("employeeId")?.Value;
        if (string.IsNullOrEmpty(raw))
        {
            error = Unauthorized(ApiResponse<object>.Fail("Employee identity not found in token", statusCode: 401));
            return false;
        }

        if (!Guid.TryParse(raw, out employeeId))
        {
            error = BadRequest(ApiResponse<object>.Fail("Invalid employee identity in token"));
            return false;
        }

        return true;
    }

    private (Guid? userId, string? userName) GetDecider()
    {
        Guid? userId = null;
        var raw = User.FindFirst("userId")?.Value ?? User.FindFirst("sub")?.Value;
        if (Guid.TryParse(raw, out var parsed))
            userId = parsed;
        return (userId, User.Identity?.Name);
    }
}
