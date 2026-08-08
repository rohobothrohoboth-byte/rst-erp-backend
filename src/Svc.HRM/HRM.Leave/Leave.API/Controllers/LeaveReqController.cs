using Asp.Versioning;
using Helpers;
using Leave.App.Commands;
using Leave.App.Queries;
using Leave.Domain.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Dapper;
using Leave.App.Interfaces;

namespace Leave.API.Controllers;

/// <summary>
/// LEAVE REQUEST MANAGEMENT - Employee leave requests
/// </summary>
[Authorize]
[ApiController]
[Route("api/hrm/leave/v{version:apiVersion}/Request")]
[ApiVersion("1.0")]
public class LeaveRequestController : ControllerBase
{
    private readonly IMediator _med;
    private readonly IDapperHelper _dapper;

    public LeaveRequestController(IMediator med, IDapperHelper dapper)
    {
        _med = med;
        _dapper = dapper;
    }

    private IEnumerable<string> GetModelStateErrors()
    {
        return ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage);
    }
[HttpPost("AddNewReq")]
[ProducesResponseType(StatusCodes.Status200OK)]
public async Task<IActionResult> AddNewReq([FromBody] LeaveRequestAddDto addDto)
{
    if (!ModelState.IsValid) throw new ValException(GetModelStateErrors());

    var empId = User.FindFirstValue("employeeId");
    if (string.IsNullOrEmpty(empId)) throw new UnauthorizedException("Authorization required.");

    // Parse dates as UTC explicitly
    if (addDto.StartDate.Kind != DateTimeKind.Utc)
        addDto.StartDate = DateTime.SpecifyKind(addDto.StartDate, DateTimeKind.Utc);
    if (addDto.EndDate.Kind != DateTimeKind.Utc)
        addDto.EndDate = DateTime.SpecifyKind(addDto.EndDate, DateTimeKind.Utc);

    var command = new LeaveRequestAddCmd
    {
        AddDto = addDto,
        EmpId = Guid.Parse(empId)
    };
    var response = await _med.Send(command);
    return Ok(ApiResponse<object>.Ok(response, "Leave request created successfully."));
}
    [HttpGet("MyLeaveReq")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> MyLeaveReq()
    {
        var empId = User.FindFirstValue("employeeId");
        if (string.IsNullOrEmpty(empId)) throw new UnauthorizedException("Authorization required.");

        var response = await _med.Send(new LeaveRequestMyQry { EmployeeId = Guid.Parse(empId) });
        return Ok(ApiResponse<object>.Ok(response));
    }
[HttpGet("AllRequests")]
[ProducesResponseType(StatusCodes.Status200OK)]
public async Task<IActionResult> AllRequests()
{
    var response = await _med.Send(new LeaveRequestAllQry());
    return Ok(ApiResponse<object>.Ok(response));
}
    [HttpGet("GetLeaveReq/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLeaveReq(Guid id)
    {
        var response = await _med.Send(new LeaveRequestByIdQry { Id = id });
        if (response == null) throw new DomainException($"Leave request with id [{id}] not found.");
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPut("UpdateLeaveReq/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateLeaveReq(Guid id, [FromBody] LeaveRequestModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id) throw new ValException(GetModelStateErrors());
        var command = new LeaveRequestModCmd { ModDto = modDto };
        var response = await _med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Leave request updated successfully."));
    }

    [HttpDelete("DeleteLeaveReq/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteLeaveReq(Guid id)
    {
        var command = new LeaveRequestDelCmd { Id = id };
        await _med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, "Leave request deleted successfully."));
    }

    // In LeaveRequestController.cs
    [HttpPut("Approve/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ApproveRequest(Guid id, [FromBody] ApprovalCommentDto? commentDto = null)
    {
        var approvedById = User.FindFirstValue("employeeId");
        if (string.IsNullOrEmpty(approvedById)) throw new UnauthorizedException("Authorization required.");

        var command = new LeaveRequestApproveCmd
        {
            Id = id,
            Comments = commentDto?.Comments ?? string.Empty,
            RowVersion = commentDto?.RowVersion ?? string.Empty,
            ApprovedById = Guid.Parse(approvedById)  // Add this
        };
        var response = await _med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Request approved successfully."));
    }

    [HttpPut("Reject/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RejectRequest(Guid id, [FromBody] ApprovalCommentDto? commentDto = null)
    {
        var command = new LeaveRequestRejectCmd
        {
            Id = id,
            Comments = commentDto?.Comments ?? string.Empty,
            RowVersion = commentDto?.RowVersion ?? string.Empty
        };
        var response = await _med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Request rejected successfully."));
    }

    [HttpPut("Cancel/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CancelRequest(Guid id, [FromBody] CancelRequestDto? cancelDto = null)
    {
        var command = new LeaveRequestCancelCmd
        {
            Id = id,
            RowVersion = cancelDto?.RowVersion ?? string.Empty
        };
        var response = await _med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Request cancelled successfully."));
    }

    // SINGLE debug method - check policy assignment for employee
    [HttpGet("DebugPolicy/{employeeId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DebugPolicy(Guid employeeId)
    {
        try
        {
            // Check policy assignment for this employee - use ::uuid cast for the policy ID
            const string policySql = @"
                SELECT
                    ep.""Id"",
                    ep.""EmployeeId"",
                    ep.""LeaveTypeId"",
                    ep.""LeavePolicyId"",
                    ep.""EffectiveFrom"",
                    ep.""EffectiveTo"",
                    ep.""AssignedEntitlement"",
                    lp.""Status"" as PolicyStatus,
                    lt.""Name"" as LeaveTypeName,
                    lt.""IsActive"" as LeaveTypeActive
                FROM ""EmpLeavePolicy"" ep
                INNER JOIN ""LeavePolicy"" lp ON ep.""LeavePolicyId"" = lp.""Id"" AND lp.""IsDeleted"" = false
                INNER JOIN ""LeaveType"" lt ON ep.""LeaveTypeId"" = lt.""Id"" AND lt.""IsDeleted"" = false
                WHERE ep.""EmployeeId"" = @EmployeeId::uuid
                AND ep.""IsDeleted"" = false
                AND (ep.""EffectiveTo"" IS NULL OR ep.""EffectiveTo"" >= CURRENT_DATE)";

            var policies = await _dapper.QueryAsync<dynamic>(policySql, new { EmployeeId = employeeId });

            // Check if policy config exists - cast the policy ID to UUID
            const string configSql = @"
                SELECT
                    lpc.""Id"",
                    lpc.""IsActive"",
                    lpc.""AnnualEntitlement""
                FROM ""LeavePolicyConfig"" lpc
                WHERE lpc.""LeavePolicyId"" = @PolicyId::uuid
                AND lpc.""IsDeleted"" = false";


var policyId = policies.FirstOrDefault()?.LeavePolicyId;

var configs = await _dapper.QueryAsync<dynamic>(configSql, new { PolicyId = policyId });
            return Ok(new
            {
                EmployeeId = employeeId,
                PoliciesFound = policies.Count(),
                Policies = policies,
                PolicyConfigs = configs,
                CurrentDate = DateTime.UtcNow,
                Message = policies.Any() ? "Policy found" : "No active policy found for this employee"
            });
        }
        catch (Exception ex)
        {
            return Ok(new { error = ex.Message, stackTrace = ex.StackTrace });
        }
    }
}
