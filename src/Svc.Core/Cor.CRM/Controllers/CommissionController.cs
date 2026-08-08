// Cor.CRM/Controllers/CommissionController.cs

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
public class CommissionController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogService _logger;

    public CommissionController(IMediator mediator, ILogService logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all commissions
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? agentId,
        [FromQuery] Guid? transactionId,
        [FromQuery] int? status,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var query = new CommissionAllQry
            {
                AgentId = agentId,
                TransactionId = transactionId,
                Status = status,
                FromDate = fromDate,
                ToDate = toDate,
                Page = page,
                PageSize = pageSize
            };
            var response = await _mediator.Send(query);
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting commissions");
            throw;
        }
    }

    /// <summary>
    /// Get commission by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new CommissionByIdQry { Id = id });
            if (response == null)
                throw new DomainException($"Commission with id [{id}] NOT FOUND.");
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting commission: {CommissionId}", id);
            throw;
        }
    }

    /// <summary>
    /// Get commissions by agent
    /// </summary>
    [HttpGet("agent/{agentId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByAgent(
        Guid agentId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        try
        {
            var query = new CommissionByAgentQry
            {
                AgentId = agentId,
                FromDate = fromDate,
                ToDate = toDate
            };
            var response = await _mediator.Send(query);
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting commissions by agent: {AgentId}", agentId);
            throw;
        }
    }

    /// <summary>
    /// Create a new commission
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCommissionDto dto)
    {
        try
        {
            var command = new CommissionAddCmd { Dto = dto };
            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = response.Id },
                ApiResponse<object>.Ok(response, "Commission created successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating commission");
            throw;
        }
    }

    /// <summary>
    /// Update a commission
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCommissionDto dto)
    {
        try
        {
            var command = new CommissionUpdateCmd { Id = id, Dto = dto };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Commission updated successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating commission: {CommissionId}", id);
            throw;
        }
    }

    /// <summary>
    /// Approve a commission
    /// </summary>
    [HttpPost("{id:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Approve(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new CommissionApproveCmd { Id = id });
            return Ok(ApiResponse<object>.Ok(response, "Commission approved successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving commission: {CommissionId}", id);
            throw;
        }
    }

    /// <summary>
    /// Mark commission as paid
    /// </summary>
    [HttpPost("{id:guid}/pay")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Pay(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new CommissionPayCmd { Id = id });
            return Ok(ApiResponse<object>.Ok(response, "Commission paid successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error paying commission: {CommissionId}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete a commission
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _mediator.Send(new CommissionDeleteCmd { Id = id });
            return Ok(ApiResponse<string>.Ok(null!, $"Commission with Id {id} successfully deleted."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting commission: {CommissionId}", id);
            throw;
        }
    }
}