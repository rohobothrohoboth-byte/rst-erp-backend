// Cor.CRM/Controllers/ContractController.cs

using Asp.Versioning;
using Common;
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
public class ContractController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogService _logger;

    public ContractController(IMediator mediator, ILogService logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Create a new contract
    /// </summary>
    [HttpPost]
    [PerAuth("crm.sales.contracts.add")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateContractDto dto)
    {
        try
        {
            var command = new ContractAddCmd { Dto = dto };
            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = response.Id },
                ApiResponse<object>.Ok(response, "Contract created successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating contract");
            throw;
        }
    }

    /// <summary>
    /// Get contract by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [PerAuth("crm.sales.contracts.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new ContractByIdQry { Id = id });
            if (response == null)
                throw new DomainException($"Contract with id [{id}] NOT FOUND.");

            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting contract: {ContractId}", id);
            throw;
        }
    }

    /// <summary>
    /// Get all contracts with filters
    /// </summary>
    [HttpGet]
    [PerAuth("crm.sales.contracts.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? customerId,
        [FromQuery] Guid? opportunityId,
        [FromQuery] Guid? quoteId,
        [FromQuery] string? status,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = "CreatedAt",
        [FromQuery] bool sortDescending = true)
    {
        try
        {
            var query = new ContractAllQry
            {
                CustomerId = customerId,
                OpportunityId = opportunityId,
                QuoteId = quoteId,
                Status = status,
                FromDate = fromDate,
                ToDate = toDate,
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
            _logger.LogError(ex, "Error getting contracts");
            throw;
        }
    }

    /// <summary>
    /// Update an existing contract
    /// </summary>
    [HttpPut("{id:guid}")]
    [PerAuth("crm.sales.contracts.mod")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateContractDto dto)
    {
        try
        {
            var command = new ContractUpdateCmd { Id = id, Dto = dto };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Contract updated successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating contract: {ContractId}", id);
            throw;
        }
    }

    /// <summary>
    /// Sign a contract
    /// </summary>
    [HttpPost("{id:guid}/sign")]
    [PerAuth("crm.sales.contracts.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Sign(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new ContractSignCmd { Id = id });
            return Ok(ApiResponse<object>.Ok(response, "Contract signed successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error signing contract: {ContractId}", id);
            throw;
        }
    }

    /// <summary>
    /// Activate a contract
    /// </summary>
    [HttpPost("{id:guid}/activate")]
    [PerAuth("crm.sales.contracts.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Activate(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new ContractActivateCmd { Id = id });
            return Ok(ApiResponse<object>.Ok(response, "Contract activated successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating contract: {ContractId}", id);
            throw;
        }
    }

    /// <summary>
    /// Terminate a contract
    /// </summary>
    [HttpPost("{id:guid}/terminate")]
    [PerAuth("crm.sales.contracts.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Terminate(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new ContractTerminateCmd { Id = id });
            return Ok(ApiResponse<object>.Ok(response, "Contract terminated successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error terminating contract: {ContractId}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete a contract (soft delete)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [PerAuth("crm.sales.contracts.del")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _mediator.Send(new ContractDelCmd { Id = id });
            return Ok(ApiResponse<string>.Ok(null!, $"Contract with Id {id} successfully deleted."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting contract: {ContractId}", id);
            throw;
        }
    }

    /// <summary>
    /// Get contract statistics
    /// </summary>
    [HttpGet("stats")]
    [PerAuth("crm.sales.contracts.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var response = await _mediator.Send(new ContractStatsQry());
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting contract stats");
            throw;
        }
    }
}