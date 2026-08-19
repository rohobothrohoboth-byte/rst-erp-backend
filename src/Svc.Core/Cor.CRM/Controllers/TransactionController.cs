// Cor.CRM/Controllers/TransactionController.cs

using Asp.Versioning;
using Common;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Queries;
using Cor.CRM.Commands;
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
public class TransactionController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogService _logger;

    public TransactionController(IMediator mediator, ILogService logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [PerAuth("crm.realestate.transactions.view")]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? propertyId,
        [FromQuery] Guid? buyerId,
        [FromQuery] Guid? sellerId,
        [FromQuery] int? status,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var query = new TransactionAllQry
            {
                PropertyId = propertyId,
                BuyerId = buyerId,
                SellerId = sellerId,
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
            _logger.LogError(ex, "Error getting transactions");
            throw;
        }
    }

    [HttpGet("{id:guid}")]
    [PerAuth("crm.realestate.transactions.view")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new TransactionByIdQry { Id = id });
            if (response == null)
                throw new DomainException($"Transaction with id [{id}] NOT FOUND.");
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transaction: {TransactionId}", id);
            throw;
        }
    }

    [HttpPost]
    [PerAuth("crm.realestate.transactions.add")]
    public async Task<IActionResult> Create([FromBody] CreateTransactionDto dto)
    {
        try
        {
            var command = new TransactionAddCmd { Dto = dto };
            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = response.Id },
                ApiResponse<object>.Ok(response, "Transaction created successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating transaction");
            throw;
        }
    }

    [HttpPut("{id:guid}")]
    [PerAuth("crm.realestate.transactions.mod")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTransactionDto dto)
    {
        try
        {
            var command = new TransactionUpdateCmd { Id = id, Dto = dto };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Transaction updated successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating transaction: {TransactionId}", id);
            throw;
        }
    }

    [HttpPost("{id:guid}/accept")]
    [PerAuth("crm.realestate.transactions.view")]
    public async Task<IActionResult> Accept(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new TransactionAcceptCmd { Id = id });
            return Ok(ApiResponse<object>.Ok(response, "Transaction accepted successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accepting transaction: {TransactionId}", id);
            throw;
        }
    }

    [HttpPost("{id:guid}/close")]
    [PerAuth("crm.realestate.transactions.view")]
    public async Task<IActionResult> Close(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new TransactionCloseCmd { Id = id });
            return Ok(ApiResponse<object>.Ok(response, "Transaction closed successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing transaction: {TransactionId}", id);
            throw;
        }
    }

    [HttpDelete("{id:guid}")]
    [PerAuth("crm.realestate.transactions.del")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _mediator.Send(new TransactionDelCmd { Id = id });
            return Ok(ApiResponse<string>.Ok(null!, $"Transaction with Id {id} successfully deleted."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting transaction: {TransactionId}", id);
            throw;
        }
    }

    [HttpGet("stats")]
        [PerAuth("crm.realestate.transactions.view")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStats(
            [FromQuery] Guid? propertyId,
            [FromQuery] Guid? agentId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            try
            {
                var query = new TransactionStatsQry
                {
                    PropertyId = propertyId,
                    AgentId = agentId,
                    FromDate = fromDate,
                    ToDate = toDate
                };

                var response = await _mediator.Send(query);
                return Ok(ApiResponse<object>.Ok(response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transaction stats");
                throw;
            }
        }

}