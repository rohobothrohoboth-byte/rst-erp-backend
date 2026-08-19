// Leave.API/Controllers/YearEndProcessingController.cs

using Asp.Versioning;
using Common;
using Helpers;
using Leave.App.Commands;
using Leave.App.Queries;
using Leave.Domain.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Leave.App.Services;
using System.Security.Claims;
using Leave.App.Interfaces;

namespace Leave.API.Controllers;

[Authorize]
[ApiController]
[Route("api/hrm/leave/v{version:apiVersion}/YearEnd")]
[ApiVersion("1.0")]
public class YearEndProcessingController : ControllerBase
{
    private readonly IYearEndProcessingService _yearEndService;
    private readonly ILogger<YearEndProcessingController> _logger;
    private readonly IHistoryRepository _historyRepository;

    public YearEndProcessingController(
        IYearEndProcessingService yearEndService,
        ILogger<YearEndProcessingController> logger,
        IHistoryRepository historyRepository)
    {
        _yearEndService = yearEndService;
        _logger = logger;
        _historyRepository = historyRepository;
    }

    /// <summary>
    /// Get available fiscal years for year-end processing
    /// </summary>
    [HttpGet("AvailableFiscalYears")]
    [PerAuth("hr.leave.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailableFiscalYears(CancellationToken ct)
    {
        var result = await _yearEndService.GetAvailableFiscalYearsAsync(ct);
        return Ok(ApiResponse<object>.Ok(result, "Available fiscal years retrieved."));
    }

    /// <summary>
    /// Check if year-end processing can be performed for a given fiscal year
    /// </summary>
    [HttpGet("CanProcess/{fiscalYearId}")]
    [PerAuth("hr.leave.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CanProcessYearEnd(Guid fiscalYearId, CancellationToken ct)
    {
        try
        {
            var canProcess = await _yearEndService.CanProcessYearEnd(fiscalYearId, ct);

            var fiscalYears = await _yearEndService.GetAvailableFiscalYearsAsync(ct);
            var fiscalYear = fiscalYears.FirstOrDefault(fy => fy.Id == fiscalYearId);
            var fiscalYearNumber = fiscalYear?.Name ?? fiscalYearId.ToString();

            return Ok(ApiResponse<object>.Ok(new
            {
                canProcess = canProcess,
                fiscalYearId = fiscalYearId,
                fiscalYear = fiscalYearNumber,
                message = canProcess
                    ? $"Year-end processing is available for fiscal year {fiscalYearNumber}"
                    : $"Year-end processing is not available for fiscal year {fiscalYearNumber}. Either the year hasn't ended or it has already been processed."
            }, "Year-end processing availability checked."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking year-end processing availability for {FiscalYearId}", fiscalYearId);
            return StatusCode(500, ApiResponse<object>.Ok(null, $"Error checking availability: {ex.Message}"));
        }
    }

    /// <summary>
    /// Get the next available fiscal year for processing
    /// </summary>
    [HttpGet("NextAvailableYear")]
    [PerAuth("hr.leave.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNextAvailableYear(CancellationToken ct)
    {
        try
        {
            var fiscalYears = await _yearEndService.GetAvailableFiscalYearsAsync(ct);
            var currentDate = DateTime.UtcNow;

            var availableFiscalYear = fiscalYears
                .Where(fy => fy.DateEnd <= currentDate)
                .OrderByDescending(fy => fy.DateEnd)
                .FirstOrDefault();

            if (availableFiscalYear != null)
            {
                var canProcess = await _yearEndService.CanProcessYearEnd(availableFiscalYear.Id, ct);
                if (canProcess)
                {
                    return Ok(ApiResponse<object>.Ok(new
                    {
                        availableYearId = availableFiscalYear.Id,
                        availableYearName = availableFiscalYear.Name,
                        currentYear = currentDate.Year,
                        message = $"Next available fiscal year for processing is {availableFiscalYear.Name}"
                    }, "Next available year retrieved."));
                }
            }

            return Ok(ApiResponse<object>.Ok(new
            {
                availableYearId = (Guid?)null,
                availableYearName = (string?)null,
                currentYear = currentDate.Year,
                message = "No available fiscal years found for processing"
            }, "Next available year retrieved."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting next available year");
            return StatusCode(500, ApiResponse<object>.Ok(null, $"Error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Preview carryover calculations for a fiscal year
    /// </summary>
   [HttpGet("Preview/{fiscalYearId}")]
   [PerAuth("hr.leave.view")]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType(StatusCodes.Status400BadRequest)]
   public async Task<IActionResult> PreviewCarryover(Guid fiscalYearId, CancellationToken ct)
   {
       try
       {
           var canProcess = await _yearEndService.CanProcessYearEnd(fiscalYearId, ct);

           // Check if already processed by looking for carryover policies
           var fiscalYear = await _yearEndService.GetFiscalYearByIdAsync(fiscalYearId, ct);
           var isAlreadyProcessed = await _yearEndService.IsYearEndProcessedAsync(fiscalYear?.Name, ct);

           if (!canProcess && !isAlreadyProcessed)
           {
               return BadRequest(ApiResponse<object>.Ok(null,
                   $"Cannot preview for this fiscal year as it hasn't ended yet."));
           }

           if (isAlreadyProcessed)
           {
               // Return a different response for already processed years
               return Ok(ApiResponse<object>.Ok(new
               {
                   fiscalYearId = fiscalYearId,
                   fiscalYearName = fiscalYear?.Name ?? "Unknown",
                   previewDate = DateTime.UtcNow,
                   isAlreadyProcessed = true,
                   message = "Year-end processing has already been completed for this fiscal year.",
                   summary = new
                   {
                       totalEmployees = 0,
                       employeesWithCarryover = 0,
                       employeesWithLoss = 0,
                       totalRemainingDays = 0,
                       totalCarryoverDays = 0,
                       totalLostDays = 0
                   },
                   details = new List<object>()
               }, "Year-end already processed. Please view the Process Result tab for details."));
           }

           var result = await _yearEndService.PreviewCarryoverCalculationsAsync(fiscalYearId, ct);

           var totalRemaining = result.Sum(r => r.RemainingBalance);
           var totalCarryover = result.Sum(r => r.CarryoverAmount);
           var totalLost = result.Sum(r => r.LostAmount);
           var employeesWithCarryover = result.Count(r => r.CarryoverAmount > 0);
           var employeesWithLoss = result.Count(r => r.LostAmount > 0);

           return Ok(ApiResponse<object>.Ok(new
           {
               fiscalYearId = fiscalYearId,
               fiscalYearName = result.FirstOrDefault()?.FiscalYearName ?? "Unknown",
               previewDate = DateTime.UtcNow,
               isAlreadyProcessed = false,
               summary = new
               {
                   totalEmployees = result.Count,
                   employeesWithCarryover = employeesWithCarryover,
                   employeesWithLoss = employeesWithLoss,
                   totalRemainingDays = totalRemaining,
                   totalCarryoverDays = totalCarryover,
                   totalLostDays = totalLost
               },
               details = result
           }, "Preview generated successfully."));
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, "Error generating preview");
           return StatusCode(500, ApiResponse<object>.Ok(null, $"Error generating preview: {ex.Message}"));
       }
   }
    /// <summary>
    /// Process year-end carryover
    /// </summary>
    [HttpPost("Process")]
    [PerAuth("hr.leave.manage")]
    public async Task<IActionResult> ProcessYearEnd([FromBody] YearEndProcessRequestDto request, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("=== PROCESS YEAR-END STARTED ===");
            _logger.LogInformation("Request: FiscalYearId={FiscalYearId}, ProcessedBy={ProcessedBy}",
                request.FiscalYearId, request.ProcessedBy);

            _logger.LogInformation("=== USER CLAIMS ===");
            foreach (var claim in User.Claims)
            {
                _logger.LogInformation("Claim: {Type} = {Value}", claim.Type, claim.Value);
            }

            var canProcess = await _yearEndService.CanProcessYearEnd(request.FiscalYearId, ct);
            _logger.LogInformation("CanProcess result: {CanProcess}", canProcess);

            if (!canProcess)
            {
                _logger.LogWarning("Cannot process year-end - validation failed");
                return BadRequest(ApiResponse<object>.Ok(null,
                    $"Cannot process year-end for this fiscal year as it hasn't ended yet."));
            }

            var userId = User.FindFirstValue("employeeId") ?? User.FindFirstValue("userId");
            _logger.LogInformation("User ID from token: {UserId}", userId);

            if (!string.IsNullOrEmpty(userId))
            {
                request.ProcessedBy = Guid.Parse(userId);
                _logger.LogInformation("Set ProcessedBy to: {ProcessedBy}", request.ProcessedBy);
            }
            else
            {
                _logger.LogWarning("No user ID found in token, ProcessedBy remains null");
            }

            _logger.LogInformation("Calling ProcessYearEndAsync...");
            var result = await _yearEndService.ProcessYearEndAsync(request, ct);

            _logger.LogInformation("ProcessYearEndAsync completed. Success: {Success}, Message: {Message}",
                result.Success, result.Message);
            _logger.LogInformation("Employees processed: {Employees}, Carryover records: {Carryover}, Encashment records: {Encashment}",
                result.EmployeesProcessed, result.CarryoverRecordsCreated, result.EncashmentRecordsCreated);

            if (result.Success)
            {
                _logger.LogInformation("=== PROCESS YEAR-END COMPLETED SUCCESSFULLY ===");
                return Ok(ApiResponse<object>.Ok(result, result.Message));
            }
            else
            {
                _logger.LogError("=== PROCESS YEAR-END FAILED ===");
                return BadRequest(ApiResponse<object>.Ok(null, result.Message));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing year-end");
            return StatusCode(500, ApiResponse<object>.Ok(null, $"Error processing year-end: {ex.Message}"));
        }
    }

    /// <summary>
    /// Get max carryover days for all leave types
    /// </summary>
    [HttpGet("MaxCarryoverDays")]
    [PerAuth("hr.leave.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMaxCarryoverDays(CancellationToken ct)
    {
        try
        {
            var result = await _yearEndService.GetMaxCarryoverDaysAsync(ct);

            return Ok(ApiResponse<object>.Ok(new
            {
                carryoverRules = result.Select(r => new
                {
                    leaveTypeId = r.Key,
                    maxCarryoverDays = r.Value
                }),
                message = $"Retrieved max carryover days for {result.Count} leave types"
            }, "Max carryover days retrieved successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting max carryover days");
            return StatusCode(500, ApiResponse<object>.Ok(null, $"Error: {ex.Message}"));
        }
    }

    // ========== ENCASHMENT ENDPOINTS ==========

    /// <summary>
    /// Get encashment configuration for leave types
    /// </summary>
    [HttpGet("Encashment/Config")]
    [PerAuth("hr.leave.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEncashmentConfigs(CancellationToken ct)
    {
        try
        {
            var result = await _yearEndService.GetEncashmentConfigsAsync(ct);
            return Ok(ApiResponse<object>.Ok(result, "Encashment configurations retrieved successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting encashment configs");
            return StatusCode(500, ApiResponse<object>.Ok(null, $"Error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Process encashment for an employee
    /// </summary>
    [HttpPost("Encashment/Process")]
    [PerAuth("hr.leave.manage")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ProcessEncashment([FromBody] EncashmentRequestDto request, CancellationToken ct)
    {
        try
        {
            var userId = User.FindFirstValue("employeeId") ?? User.FindFirstValue("userId");
            if (!string.IsNullOrEmpty(userId))
            {
                request.ProcessedBy = Guid.Parse(userId);
            }

            var result = await _yearEndService.ProcessEncashmentAsync(request, ct);

            if (result.Success)
                return Ok(ApiResponse<object>.Ok(result, result.Message));
            else
                return BadRequest(ApiResponse<object>.Ok(null, result.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing encashment");
            return StatusCode(500, ApiResponse<object>.Ok(null, $"Error processing encashment: {ex.Message}"));
        }
    }
// In your YearEndProcessingController.cs
// YearEndProcessingController.cs


[HttpGet("Encashment/History/All")]
[PerAuth("hr.leave.view")]
[ProducesResponseType(StatusCodes.Status200OK)]
public async Task<IActionResult> GetAllEncashmentHistory(
    [FromQuery] Guid? fiscalYearId = null,
    CancellationToken ct = default)
{
    try
    {
        var result = await _yearEndService.GetAllEncashmentHistoryAsync(fiscalYearId, ct);
        return Ok(ApiResponse<object>.Ok(result, "All encashment history retrieved successfully."));
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error getting all encashment history");
        return StatusCode(500, ApiResponse<object>.Ok(null, $"Error: {ex.Message}"));
    }
}
    /// <summary>
    /// Get encashment history for an employee
    /// </summary>
    [HttpGet("Encashment/History/{employeeId}")]
    [PerAuth("hr.leave.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEncashmentHistory(Guid employeeId, CancellationToken ct)
    {
        try
        {
            var result = await _yearEndService.GetEncashmentHistoryAsync(employeeId, ct);
            return Ok(ApiResponse<object>.Ok(result, "Encashment history retrieved successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting encashment history");
            return StatusCode(500, ApiResponse<object>.Ok(null, $"Error: {ex.Message}"));
        }
    }

    // ========== HISTORY ENDPOINTS ==========

    [HttpGet("Debug/CheckPolicies")]
    [PerAuth("hr.leave.view")]
    public async Task<IActionResult> CheckPolicies(CancellationToken ct)
    {
        var result = await _yearEndService.DebugCheckCarryoverPolicies(ct);
        return Ok(result);
    }

    [HttpGet("History/Audit/{year}")]
    [PerAuth("hr.leave.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAuditHistory(int year, CancellationToken ct)
    {
        var history = await _historyRepository.GetHistoryByYearAsync(year, ct);
        return Ok(ApiResponse<object>.Ok(history, "Audit history retrieved successfully."));
    }

    [HttpGet("History/Employee/{employeeId}")]
    [PerAuth("hr.leave.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmployeeHistory(Guid employeeId, CancellationToken ct)
    {
        var history = await _historyRepository.GetHistoryByEmployeeAsync(employeeId, ct);
        return Ok(ApiResponse<object>.Ok(history, "Employee history retrieved successfully."));
    }

    [HttpGet("History/{fiscalYearId}")]
    [PerAuth("hr.leave.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProcessingHistory(Guid fiscalYearId, CancellationToken ct)
    {
        try
        {
            var fiscalYears = await _yearEndService.GetAvailableFiscalYearsAsync(ct);
            var fiscalYear = fiscalYears.FirstOrDefault(fy => fy.Id == fiscalYearId);

            if (fiscalYear == null)
            {
                return Ok(ApiResponse<object>.Ok(new { records = new List<object>(), count = 0 }, "No history found"));
            }

            var yearMatch = System.Text.RegularExpressions.Regex.Match(fiscalYear.Name, @"\d{4}");
            var yearNumber = yearMatch.Success ? int.Parse(yearMatch.Value) : DateTime.UtcNow.Year;

            var nextYear = yearNumber + 1;
            var carryoverPolicies = await _yearEndService.GetCarryoverHistoryAsync(nextYear, ct);
            var historyRecords = await _historyRepository.GetHistoryByYearAsync(yearNumber, ct);

            var result = new
            {
                fiscalYearId = fiscalYearId,
                fiscalYear = fiscalYear.Name,
                processedYear = nextYear,
                records = carryoverPolicies,
                historyRecords = historyRecords,
                count = carryoverPolicies.Count
            };

            return Ok(ApiResponse<object>.Ok(result, "Processing history retrieved successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting processing history");
            return Ok(ApiResponse<object>.Ok(new { records = new List<object>(), count = 0 }, "No history found"));
        }
    }



    // Add to YearEndProcessingController.cs

    // ========== ENCASHMENT APPROVAL ENDPOINTS ==========

  // In YearEndProcessingController.cs, add these three endpoints after your existing endpoints

  // ========== ENCASHMENT APPROVAL ENDPOINTS ==========

  /// <summary>
  /// Get pending encashment approvals for the current user
  /// </summary>
  [HttpGet("Encashment/PendingApprovals")]
  [PerAuth("hr.leave.view")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public async Task<IActionResult> GetPendingEncashmentApprovals(
      [FromQuery] string? approverId = null,
      [FromQuery] string? approvalLevel = null,
      CancellationToken ct = default)
  {
      try
      {
          // Get current user ID from token if not provided
          if (string.IsNullOrEmpty(approverId))
          {
              approverId = User.FindFirstValue("employeeId") ?? User.FindFirstValue("userId");
          }

          // Get approval level from role if not provided
          if (string.IsNullOrEmpty(approvalLevel))
          {
              var role = User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirstValue("role");
              approvalLevel = role?.ToUpper() switch
              {
                  "MGR" => "MANAGER",
                  "CEO" => "CEO",
                  "ADMIN" => "ADMIN",
                  _ => "HR"
              };
          }

          var result = await _yearEndService.GetPendingEncashmentApprovalsAsync(approverId, approvalLevel, ct);
          return Ok(ApiResponse<object>.Ok(result, "Pending encashment approvals retrieved successfully."));
      }
      catch (Exception ex)
      {
          _logger.LogError(ex, "Error getting pending encashment approvals");
          return StatusCode(500, ApiResponse<object>.Ok(null, $"Error: {ex.Message}"));
      }
  }

  /// <summary>
  /// Approve or reject an encashment request
  /// </summary>
  [HttpPost("Encashment/{requestId}/Approve")]
  [PerAuth("hr.leave.manage")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> ApproveEncashment(
      Guid requestId,
      [FromBody] EncashmentApprovalDto approval,
      CancellationToken ct)
  {
      try
      {
          var approverId = User.FindFirstValue("employeeId") ?? User.FindFirstValue("userId");
          if (string.IsNullOrEmpty(approverId))
          {
              return Unauthorized(ApiResponse<object>.Ok(null, "User not authenticated"));
          }

          approval.ApproverId = Guid.Parse(approverId);
          approval.ApproverName = User.FindFirstValue("userName") ?? User.FindFirstValue("name") ?? approverId;

          var result = await _yearEndService.ProcessEncashmentApprovalAsync(requestId, approval, ct);

          if (result.Success)
              return Ok(ApiResponse<object>.Ok(result, result.Message));
          else
              return BadRequest(ApiResponse<object>.Ok(null, result.Message));
      }
      catch (Exception ex)
      {
          _logger.LogError(ex, "Error processing encashment approval for {RequestId}", requestId);
          return StatusCode(500, ApiResponse<object>.Ok(null, $"Error: {ex.Message}"));
      }
  }

  /// <summary>
  /// Get encashment approval details
  /// </summary>
  [HttpGet("Encashment/{requestId}/ApprovalDetails")]
  [PerAuth("hr.leave.view")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public async Task<IActionResult> GetEncashmentApprovalDetails(Guid requestId, CancellationToken ct)
  {
      try
      {
          var result = await _yearEndService.GetEncashmentApprovalDetailsAsync(requestId, ct);
          return Ok(ApiResponse<object>.Ok(result, "Approval details retrieved successfully."));
      }
      catch (Exception ex)
      {
          _logger.LogError(ex, "Error getting approval details for {RequestId}", requestId);
          return StatusCode(500, ApiResponse<object>.Ok(null, $"Error: {ex.Message}"));
      }
  }

// In YearEndProcessingController.cs
[HttpGet("Encashment/Total/{employeeId}")]
[PerAuth("hr.leave.view")]
[ProducesResponseType(StatusCodes.Status200OK)]
public async Task<IActionResult> GetTotalEncashedDays(Guid employeeId, [FromQuery] int fiscalYear, CancellationToken ct)
{
    var totalEncashed = await _yearEndService.GetTotalEncashedDaysAsync(employeeId, fiscalYear, ct);
    return Ok(ApiResponse<object>.Ok(new { totalEncashed }, "Total encashed days retrieved."));
}
    /// <summary>
    /// Revert year-end processing (Admin only - use with caution)
    /// </summary>
    [HttpPost("Revert/{fiscalYear}")]
    [PerAuth("hr.leave.manage")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RevertYearEnd(int fiscalYear, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("=== USER CLAIMS DEBUG ===");
            foreach (var claim in User.Claims)
            {
                _logger.LogInformation("Claim Type: {ClaimType}, Value: {ClaimValue}", claim.Type, claim.Value);
            }
            _logger.LogInformation("=== END USER CLAIMS DEBUG ===");

            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            var roleClaimShort = User.FindFirst("role")?.Value;

            var isAdmin = roleClaim == "admin" || roleClaimShort == "admin" || User.IsInRole("admin");
            var isHrManager = roleClaim == "mgr" || roleClaimShort == "mgr" ||
                              User.IsInRole("mgr") || User.IsInRole("HR Manager");

            var isAuthorized = isAdmin || isHrManager;

            _logger.LogInformation($"RoleClaim: {roleClaim}, IsAdmin: {isAdmin}, IsHrManager: {isHrManager}, Final: {isAuthorized}");

            if (!isAuthorized)
            {
                return Unauthorized(ApiResponse<object>.Ok(null, "Only administrators or HR managers can revert year-end processing."));
            }

            var result = await _yearEndService.RevertYearEndProcessingAsync(fiscalYear, ct);

            if (result.Success)
            {
                _logger.LogWarning("Year-end processing for fiscal year {FiscalYear} was reverted by user {UserId}",
                    fiscalYear, User.FindFirstValue("userId") ?? User.FindFirstValue("nameid"));

                return Ok(ApiResponse<object>.Ok(new
                {
                    success = true,
                    fiscalYear = fiscalYear,
                    revertedAt = DateTime.UtcNow,
                    message = result.Message,
                    employeesRestored = result.EmployeesProcessed,
                    carryoverRecordsDeleted = result.CarryoverRecordsCreated
                }, result.Message));
            }
            else
            {
                return BadRequest(ApiResponse<object>.Ok(new
                {
                    success = false,
                    message = result.Message,
                    errors = result.Errors
                }, result.Message));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reverting year-end processing for fiscal year {FiscalYear}", fiscalYear);
            return StatusCode(500, ApiResponse<object>.Ok(null, $"Error: {ex.Message}"));
        }
    }
}