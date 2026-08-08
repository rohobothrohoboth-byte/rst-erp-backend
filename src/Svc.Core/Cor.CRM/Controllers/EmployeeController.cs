// Cor.CRM/Controllers/EmployeeController.cs

using Asp.Versioning;
using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Queries;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cor.CRM.Controllers;

/// <summary>
/// Employee Management Endpoints
/// </summary>
[Authorize]
[ApiController]
[Route("api/core/crm/v{version:apiVersion}/Employee")]
[ApiVersion("1.0")]
public class EmployeeController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogService _logger;

    public EmployeeController(IMediator mediator, ILogService logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all employees (for assignment dropdown)
    /// </summary>
    [HttpGet("AllEmployees")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllEmployees()
    {
        try
        {
            var response = await _mediator.Send(new EmployeeAllQry());
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all employees");
            throw;
        }
    }

    /// <summary>
    /// Get employees available for assignment (with AppUser)
    /// </summary>
    [HttpGet("ForAssignment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmployeesForAssignment()
    {
        try
        {
            var response = await _mediator.Send(new EmployeeForAssignmentQry());
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting employees for assignment");
            throw;
        }
    }

    /// <summary>
    /// Get a single employee by ID
    /// </summary>
    [HttpGet("GetEmployee/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmployee(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new EmployeeByIdQry { Id = id });
            if (response == null)
            {
                throw new DomainException($"Employee with id [{id}] NOT FOUND.");
            }
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting employee: {EmployeeId}", id);
            throw;
        }
    }
   /// <summary>
   /// Get employee by AppUser ID
   /// </summary>
   [HttpGet("ByAppUser/{appUserId:guid}")]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType(StatusCodes.Status404NotFound)]
   public async Task<IActionResult> GetEmployeeByAppUser(Guid appUserId)
   {
       try
       {
           var response = await _mediator.Send(new EmployeeByAppUserQry { AppUserId = appUserId });
           // ✅ If null, return 404 instead of throwing
           if (response == null)
           {
               return Ok(ApiResponse<object>.Ok(null, $"Employee with AppUserId [{appUserId}] NOT FOUND."));
           }
           return Ok(ApiResponse<object>.Ok(response));
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, "Error getting employee by AppUser: {AppUserId}", appUserId);
           throw;
       }
   }
}