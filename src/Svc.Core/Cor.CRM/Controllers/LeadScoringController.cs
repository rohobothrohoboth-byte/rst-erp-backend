// Cor.CRM/Controllers/LeadScoringController.cs

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
[Route("api/core/crm/v{version:apiVersion}/Scoring")]
[ApiVersion("1.0")]
public class LeadScoringController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogService _logger;

    public LeadScoringController(IMediator mediator, ILogService logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all score rules
    /// </summary>
    [HttpGet("Rules")]
    [PerAuth("crm.settings.scoring.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRules()
    {
        try
        {
            var response = await _mediator.Send(new ScoreRulesQry());
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting score rules");
            throw;
        }
    }

    /// <summary>
    /// Get score rule by ID
    /// </summary>
    [HttpGet("Rule/{id:guid}")]
    [PerAuth("crm.settings.scoring.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRule(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new ScoreRuleByIdQry { Id = id });
            if (response == null)
                throw new DomainException($"Score rule with id [{id}] NOT FOUND.");
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting score rule: {RuleId}", id);
            throw;
        }
    }

    /// <summary>
    /// Create a score rule
    /// </summary>
    [HttpPost("Rule")]
    [PerAuth("crm.settings.scoring.add")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateRule([FromBody] CreateScoreRuleDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        try
        {
            var command = new ScoreRuleAddCmd { Dto = dto };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Score rule created successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating score rule");
            throw;
        }
    }

    /// <summary>
    /// Update a score rule
    /// </summary>
    [HttpPut("Rule/{id:guid}")]
    [PerAuth("crm.settings.scoring.mod")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateRule(Guid id, [FromBody] UpdateScoreRuleDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        try
        {
            var command = new ScoreRuleModCmd { Id = id, Dto = dto };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Score rule updated successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating score rule: {RuleId}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete a score rule
    /// </summary>
    [HttpDelete("Rule/{id:guid}")]
    [PerAuth("crm.settings.scoring.del")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRule(Guid id)
    {
        try
        {
            var command = new ScoreRuleDelCmd { Id = id };
            await _mediator.Send(command);
            return Ok(ApiResponse<string>.Ok(null!, "Score rule deleted successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting score rule: {RuleId}", id);
            throw;
        }
    }

    /// <summary>
    /// Calculate score for a lead
    /// </summary>
    [HttpPost("Calculate/{leadId:guid}")]
    [PerAuth("crm.settings.scoring.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CalculateScore(Guid leadId)
    {
        try
        {
            var response = await _mediator.Send(new CalculateScoreCmd { LeadId = leadId });
            return Ok(ApiResponse<object>.Ok(response, "Score calculated successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating score for lead: {LeadId}", leadId);
            throw;
        }
    }
}