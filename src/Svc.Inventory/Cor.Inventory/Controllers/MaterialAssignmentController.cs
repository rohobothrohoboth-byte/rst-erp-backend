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
public class MaterialAssignmentController : ControllerBase
{
    private readonly IMaterialService _materialService;
    private readonly ILogger<MaterialAssignmentController> _logger;

    public MaterialAssignmentController(IMaterialService materialService, ILogger<MaterialAssignmentController> logger)
    {
        _materialService = materialService;
        _logger = logger;
    }

    [HttpGet("my")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<List<MaterialAssignmentDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyMaterials()
    {
        if (!TryGetEmployeeId(out var employeeId, out var error))
            return error!;

        try
        {
            var assignments = await _materialService.GetMyAssignments(employeeId);
            return Ok(ApiResponse<List<MaterialAssignmentDto>>.Ok(assignments));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting my materials");
            return StatusCode(500, ApiResponse<List<MaterialAssignmentDto>>.Fail("Failed to get materials", statusCode: 500));
        }
    }

    [HttpGet]
    [PerAuth("inv.stock.outbound.view")]
    [ProducesResponseType(typeof(ApiResponse<List<MaterialAssignmentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var assignments = await _materialService.GetAllAssignments();
            return Ok(ApiResponse<List<MaterialAssignmentDto>>.Ok(assignments));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting material assignments");
            return StatusCode(500, ApiResponse<List<MaterialAssignmentDto>>.Fail("Failed to get material assignments", statusCode: 500));
        }
    }

    [HttpPost]
    [PerAuth("inv.stock.outbound.add")]
    [ProducesResponseType(typeof(ApiResponse<MaterialAssignmentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMaterialAssignmentDto dto)
    {
        try
        {
            var assignment = await _materialService.CreateAssignment(dto);
            return CreatedAtAction(nameof(GetAll), new { version = "1.0" },
                ApiResponse<MaterialAssignmentDto>.Ok(assignment, "Material assignment created"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating material assignment");
            return StatusCode(500, ApiResponse<MaterialAssignmentDto>.Fail("Failed to create material assignment", statusCode: 500));
        }
    }

    [HttpPut("{id}/return")]
    [PerAuth("inv.stock.outbound.mod")]
    [ProducesResponseType(typeof(ApiResponse<MaterialAssignmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Return(Guid id)
    {
        try
        {
            var assignment = await _materialService.ReturnAssignment(id);
            return Ok(ApiResponse<MaterialAssignmentDto>.Ok(assignment, "Material returned"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<MaterialAssignmentDto>.Fail(ex.Message, statusCode: 404));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error returning material assignment {Id}", id);
            return StatusCode(500, ApiResponse<MaterialAssignmentDto>.Fail("Failed to return material assignment", statusCode: 500));
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
}
