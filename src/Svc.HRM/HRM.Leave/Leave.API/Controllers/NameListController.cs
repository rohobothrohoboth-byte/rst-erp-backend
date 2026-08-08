using Asp.Versioning;
using Helpers;
using Leave.App.Queries;
using Leave.Domain.DTOs;  // ? ADD THIS for NameList
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Leave.API.Controllers;

/// <summary>
/// NAME LISTS for dropdowns and selections in the Leave module
/// </summary>
[Authorize]
[ApiController]
[Route("api/hrm/leave/v{version:apiVersion}/Names")]
[ApiVersion("1.0")]
public class NameListController : ControllerBase
{
    private readonly IMediator _med;

    public NameListController(IMediator med)
    {
        _med = med;
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("Test")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Test()
    {
        return Ok(new
        {
            message = "Leave API is working!",
            timestamp = DateTime.UtcNow,
            apiVersion = "v1"
        });
    }

    // ==================== LEAVE TYPE NAMES ====================

    /// <summary>
    /// Get all leave type names for dropdowns
    /// </summary>
    [HttpGet("LeaveTypes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLeaveTypeNames()
    {
        var response = await _med.Send(new LeaveTypeNameAllQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    /// <summary>
    /// Get leave type name by ID
    /// </summary>
    [HttpGet("LeaveType/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLeaveTypeNameById(Guid id)
    {
        var response = await _med.Send(new LeaveTypeNameByIdQry { Id = id });
        if (response == null)
            throw new DomainException($"Leave type with id [{id}] not found.");
        return Ok(ApiResponse<object>.Ok(response));
    }

    // ==================== LEAVE POLICY NAMES ====================

    /// <summary>
    /// Get all leave policy names for dropdowns
    /// </summary>
    [HttpGet("LeavePolicies")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLeavePolicyNames()
    {
        var response = await _med.Send(new LeavePolicyAllQry());
        var nameList = response?.Select(p => new NameList { Id = p.Id, Name = p.Name }).ToList();
        return Ok(ApiResponse<object>.Ok(nameList ?? new List<NameList>()));
    }

    /// <summary>
    /// Get leave policy name by ID
    /// </summary>
    [HttpGet("LeavePolicy/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLeavePolicyNameById(Guid id)
    {
        var response = await _med.Send(new LeavePolicyByIdQry { Id = id });
        if (response == null)
            throw new DomainException($"Leave policy with id [{id}] not found.");
        return Ok(ApiResponse<object>.Ok(new NameList { Id = response.Id, Name = response.Name }));
    }
}