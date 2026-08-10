// Cor.CRM/Controllers/LeadRoutingController.cs

using Asp.Versioning;
using Common;
using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Queries;
using Helpers;
using MediatR;
using Cor.CRM.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cor.CRM.Controllers;

[Authorize]
[ApiController]
[Route("api/core/crm/v{version:apiVersion}/Routing")]
[ApiVersion("1.0")]
public class LeadRoutingController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogService _logger;

    public LeadRoutingController(IMediator mediator, ILogService logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all routing rules
    /// </summary>
    [HttpGet("Rules")]
    [PerAuth("crm.settings.routing.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRules()
    {
        try
        {
            var response = await _mediator.Send(new RoutingRulesQry());
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting routing rules");
            throw;
        }
    }

    /// <summary>
    /// Get routing rule by ID
    /// </summary>
    [HttpGet("Rule/{id:guid}")]
    [PerAuth("crm.settings.routing.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRule(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new RoutingRuleByIdQry { Id = id });
            if (response == null)
                throw new DomainException($"Routing rule with id [{id}] NOT FOUND.");
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting routing rule: {RuleId}", id);
            throw;
        }
    }

    /// <summary>
    /// Create a routing rule
    /// </summary>
    [HttpPost("Rule")]
    [PerAuth("crm.settings.routing.add")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateRule([FromBody] CreateRoutingRuleDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        try
        {
            var command = new RoutingRuleAddCmd { Dto = dto };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Routing rule created successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating routing rule");
            throw;
        }
    }

    /// <summary>
    /// Update a routing rule
    /// </summary>
    [HttpPut("Rule/{id:guid}")]
    [PerAuth("crm.settings.routing.mod")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateRule(Guid id, [FromBody] UpdateRoutingRuleDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        try
        {
            var command = new RoutingRuleModCmd { Id = id, Dto = dto };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Routing rule updated successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating routing rule: {RuleId}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete a routing rule
    /// </summary>
    [HttpDelete("Rule/{id:guid}")]
    [PerAuth("crm.settings.routing.del")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRule(Guid id)
    {
        try
        {
            var command = new RoutingRuleDelCmd { Id = id };
            await _mediator.Send(command);
            return Ok(ApiResponse<string>.Ok(null!, "Routing rule deleted successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting routing rule: {RuleId}", id);
            throw;
        }
    }

    /// <summary>
    /// Get routing statistics
    /// </summary>
    [HttpGet("Stats")]
    [PerAuth("crm.settings.routing.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var response = await _mediator.Send(new RoutingStatsQry());
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting routing stats");
            throw;
        }
    }
}