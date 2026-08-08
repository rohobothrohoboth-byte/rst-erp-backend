using Asp.Versioning;
using Cor.CRM.Commands;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Queries;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cor.CRM.Interfaces;
using Microsoft.AspNetCore.ResponseCaching;

namespace Cor.CRM.Controllers;

/// <summary>
/// Lead Management Endpoints
/// </summary>
[Authorize]
[ApiController]
[Route("api/core/crm/v{version:apiVersion}/Lead")]
[ApiVersion("1.0")]
public class LeadController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogService _logger;

    public LeadController(IMediator mediator, ILogService logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all leads with optional filters
    /// </summary>
    [HttpGet("AllLeads")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "page", "pageSize", "status", "searchTerm" })]
    public async Task<IActionResult> GetAllLeads([FromQuery] LeadFilterDto? filter)
    {
        try
        {
            var response = await _mediator.Send(new LeadAllQry { Filter = filter });
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all leads");
            throw;
        }
    }

    /// <summary>
    /// Get a single lead by ID
    /// </summary>
    [HttpGet("GetLead/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(Duration = 120)]
    public async Task<IActionResult> GetLead(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new LeadByIdQry { Id = id });
            if (response == null)
            {
                throw new DomainException($"LEAD with id [{id}] NOT FOUND.");
            }
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting lead: {LeadId}", id);
            throw;
        }
    }

    /// <summary>
    /// Create a new lead
    /// </summary>
    [HttpPost("AddLead")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ResponseCache(NoStore = true)]
    public async Task<IActionResult> CreateLead([FromBody] CreateLeadDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            throw new ValException(errors);
        }

        try
        {
            var command = new LeadAddCmd { AddDto = addDto };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "New LEAD successfully created."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating lead");
            throw;
        }
    }

    /// <summary>
    /// Update an existing lead
    /// </summary>
    [HttpPut("ModLead/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ResponseCache(NoStore = true)]
    public async Task<IActionResult> UpdateLead(Guid id, [FromBody] UpdateLeadDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            throw new ValException(errors);
        }

        try
        {
            var command = new LeadModCmd { ModDto = modDto };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Selected LEAD successfully updated."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating lead: {LeadId}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete a lead (soft delete)
    /// </summary>
    [HttpDelete("DelLead/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(NoStore = true)]
    public async Task<IActionResult> DeleteLead(Guid id)
    {
        try
        {
            var command = new LeadDelCmd { Id = id };
            await _mediator.Send(command);
            return Ok(ApiResponse<string>.Ok(null!, $"LEAD with Id {id} successfully deleted."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting lead: {LeadId}", id);
            throw;
        }
    }

    /// <summary>
    /// Convert a lead to customer
    /// </summary>
    [HttpPost("ConvertLead/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ResponseCache(NoStore = true)]
    public async Task<IActionResult> ConvertLead(Guid id)
    {
        try
        {
            var command = new LeadConvertCmd { Id = id };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "LEAD successfully converted to customer."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting lead: {LeadId}", id);
            throw;
        }
    }

    /// <summary>
    /// Assign a lead to a user
    /// </summary>
    [HttpPost("AssignLead/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(NoStore = true)]
    public async Task<IActionResult> AssignLead(Guid id, [FromBody] Guid userId)
    {
        try
        {
            var command = new LeadAssignCmd { Id = id, UserId = userId };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "LEAD successfully assigned."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning lead: {LeadId}", id);
            throw;
        }
    }

    /// <summary>
    /// Bulk action on leads (assign, change status, add tags, delete)
    /// </summary>
    [HttpPost("BulkAction")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ResponseCache(NoStore = true)]
    public async Task<IActionResult> BulkAction([FromBody] LeadBulkActionDto bulkDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            throw new ValException(errors);
        }

        try
        {
            var command = new LeadBulkActionCmd { BulkDto = bulkDto };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<bool>.Ok(response, "Bulk action completed successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing bulk action");
            throw;
        }
    }

    /// <summary>
    /// Bulk assign leads to a user
    /// </summary>
    [HttpPost("BulkAssign")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ResponseCache(NoStore = true)]
    public async Task<IActionResult> BulkAssign([FromBody] LeadBulkAssignDto assignDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            throw new ValException(errors);
        }

        try
        {
            var command = new LeadBulkAssignCmd
            {
                LeadIds = assignDto.LeadIds,
                UserId = assignDto.UserId
            };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<bool>.Ok(response, "Bulk assign completed successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing bulk assign");
            throw;
        }
    }

    /// <summary>
    /// Get lead statistics
    /// </summary>
    [HttpGet("Stats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ResponseCache(Duration = 30)]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var response = await _mediator.Send(new LeadStatsQry());
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting lead stats");
            throw;
        }
    }

    /// <summary>
    /// Get leads by status
    /// </summary>
    [HttpGet("ByStatus/{status}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ResponseCache(Duration = 60)]
    public async Task<IActionResult> GetLeadsByStatus(string status)
    {
        try
        {
            var response = await _mediator.Send(new LeadByStatusQry { Status = status });
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting leads by status: {Status}", status);
            throw;
        }
    }

    /// <summary>
    /// Get leads assigned to a specific user
    /// </summary>
    [HttpGet("ByAssignedUser/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ResponseCache(Duration = 60)]
    public async Task<IActionResult> GetLeadsByAssignedUser(Guid userId)
    {
        try
        {
            var response = await _mediator.Send(new LeadByAssignedUserQry { UserId = userId });
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting leads by assigned user: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Get leads for routing (unassigned high priority leads)
    /// </summary>
    [HttpGet("ForRouting")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ResponseCache(Duration = 30)]
    public async Task<IActionResult> GetLeadsForRouting()
    {
        try
        {
            var response = await _mediator.Send(new LeadForRoutingQry());
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting leads for routing");
            throw;
        }
    }

    /// <summary>
    /// Get total lead count
    /// </summary>
    [HttpGet("Count")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ResponseCache(Duration = 30)]
    public async Task<IActionResult> GetLeadCount([FromQuery] string? status)
    {
        try
        {
            var response = await _mediator.Send(new LeadCountQry { Status = status });
            return Ok(ApiResponse<int>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting lead count");
            throw;
        }
    }


}

/// <summary>
/// Bulk assign DTO
/// </summary>
public class LeadBulkAssignDto
{
    public List<Guid> LeadIds { get; set; } = new();
    public Guid UserId { get; set; }
}