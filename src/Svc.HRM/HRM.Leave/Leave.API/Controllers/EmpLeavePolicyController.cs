// Leave.API/Controllers/EmpLeavePolicyController.cs
using Asp.Versioning;
using Helpers;
using Leave.App.Commands;
using Leave.App.Queries;
using Leave.Domain.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Leave.API.Controllers;

[Authorize]
[ApiController]
[Route("api/hrm/leave/v{version:apiVersion}/EmpLeavePolicy")]
[ApiVersion("1.0")]
public class EmpLeavePolicyController : ControllerBase
{
    private readonly IMediator _med;

    public EmpLeavePolicyController(IMediator med)
    {
        _med = med;
    }

    /// <summary>
    /// Get all employee leave policy assignments
    /// </summary>
    [HttpGet("All")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var response = await _med.Send(new EmpLeavePolicyAllQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    /// <summary>
    /// Get leave policy assignments for a specific employee
    /// </summary>
    [HttpGet("ByEmployee/{employeeId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByEmployee(Guid employeeId)
    {
        var response = await _med.Send(new EmpLeavePolicyByEmployeeQry { EmployeeId = employeeId });
        return Ok(ApiResponse<object>.Ok(response));
    }

    /// <summary>
    /// Get leave policy assignment by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _med.Send(new EmpLeavePolicyByIdQry { Id = id });
        return Ok(ApiResponse<object>.Ok(response));
    }

    /// <summary>
    /// Assign leave policy to an employee
    /// </summary>
    [HttpPost("Assign")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Assign([FromBody] EmpLeavePolicyAddDto addDto)
    {
        var command = new EmpLeavePolicyAddCmd { AddDto = addDto };
        var response = await _med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Leave policy assigned successfully."));
    }

    /// <summary>
    /// Bulk assign leave policy to multiple employees
    /// </summary>
    [HttpPost("BulkAssign")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> BulkAssign([FromBody] BatchAssignDto batchDto)
    {
        var command = new EmpLeavePolicyBulkAddCmd { BatchDto = batchDto };
        var response = await _med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, $"{response.Count} employees assigned successfully."));
    }

    /// <summary>
    /// Update leave policy assignment
    /// </summary>
    [HttpPut("Update/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] EmpLeavePolicyModDto modDto)
    {
        if (modDto.Id != id)
            throw new DomainException("ID mismatch");

        var command = new EmpLeavePolicyModCmd { ModDto = modDto };
        var response = await _med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Leave policy assignment updated successfully."));
    }

    /// <summary>
    /// Soft delete leave policy assignment
    /// </summary>
    [HttpDelete("Delete/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new EmpLeavePolicyDelCmd { Id = id };
        await _med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, "Leave policy assignment removed successfully."));
    }

    /// <summary>
    /// Assign leave type to all employees in a department
    /// </summary>
    [HttpPost("AssignByDepartment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AssignByDepartment([FromBody] DepartmentAssignDto assignDto)
    {
        var command = new EmpLeavePolicyByDeptCmd { AssignDto = assignDto };
        var response = await _med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, $"Assigned to {response.Count} employees in department."));
    }
}