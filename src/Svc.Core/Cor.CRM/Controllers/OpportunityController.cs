// Cor.CRM/Controllers/OpportunityController.cs

using Asp.Versioning;
using Cor.CRM.Commands;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Queries;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cor.CRM.Interfaces;
namespace Cor.CRM.Controllers;

[Authorize]
[ApiController]
[Route("api/core/crm/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class OpportunityController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogService _logger;

    public OpportunityController(IMediator mediator, ILogService logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all opportunities with filters
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? searchTerm,
        [FromQuery] string? stage,
        [FromQuery] Guid? customerId,
        [FromQuery] Guid? leadId,
        [FromQuery] Guid? assignedToUserId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] decimal? minAmount,
        [FromQuery] decimal? maxAmount,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = "CreatedAt",
        [FromQuery] bool sortDescending = false)
    {
        try
        {
            var query = new OpportunityAllQry
            {
                SearchTerm = searchTerm,
                Stage = stage,
                CustomerId = customerId,
                LeadId = leadId,
                AssignedToUserId = assignedToUserId,
                FromDate = fromDate,
                ToDate = toDate,
                MinAmount = minAmount,
                MaxAmount = maxAmount,
                Page = page,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDescending = sortDescending
            };

            var response = await _mediator.Send(query);
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all opportunities");
            throw;
        }
    }

    /// <summary>
    /// Get opportunity by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new OpportunityByIdQry { Id = id });
            if (response == null)
                throw new DomainException($"Opportunity with id [{id}] NOT FOUND.");

            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting opportunity: {OpportunityId}", id);
            throw;
        }
    }

    /// <summary>
    /// Create a new opportunity
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateOpportunityDto dto)
    {
        try
        {
            var command = new OpportunityAddCmd { Dto = dto };
            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = response.Id },
                ApiResponse<object>.Ok(response, "Opportunity created successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating opportunity");
            throw;
        }
    }

    /// <summary>
    /// Update an existing opportunity
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOpportunityDto dto)
    {
        try
        {
            var command = new OpportunityModCmd { Id = id, Dto = dto };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Opportunity updated successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating opportunity: {OpportunityId}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete an opportunity
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _mediator.Send(new OpportunityDelCmd { Id = id });
            return Ok(ApiResponse<string>.Ok(null!, $"Opportunity with Id {id} successfully deleted."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting opportunity: {OpportunityId}", id);
            throw;
        }
    }

    /// <summary>
    /// Update opportunity stage
    /// </summary>
    [HttpPatch("{id:guid}/stage")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateStage(Guid id, [FromBody] UpdateStageDto dto)
    {
        try
        {
            var command = new OpportunityUpdateStageCmd { Id = id, Stage = dto.Stage };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Stage updated successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating opportunity stage: {OpportunityId}", id);
            throw;
        }
    }

    /// <summary>
    /// Get opportunity statistics
    /// </summary>
    [HttpGet("stats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats([FromQuery] Guid? customerId, [FromQuery] Guid? leadId)
    {
        try
        {
            var query = new OpportunityStatsQry { CustomerId = customerId, LeadId = leadId };
            var response = await _mediator.Send(query);
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting opportunity stats");
            throw;
        }
    }

    /// <summary>
    /// Get opportunity pipeline
    /// </summary>
    [HttpGet("pipeline")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPipeline([FromQuery] Guid? customerId, [FromQuery] Guid? leadId)
    {
        try
        {
            var query = new OpportunityPipelineQry { CustomerId = customerId, LeadId = leadId };
            var response = await _mediator.Send(query);
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting opportunity pipeline");
            throw;
        }
    }
}

public class UpdateStageDto
{
    public string Stage { get; set; } = string.Empty;
}