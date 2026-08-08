// Controllers/PaymentApprovalChainController.cs
using Cor.Finance.Commands;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace Cor.Finance.Controllers;

[ApiController]
[Route("api/finance/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class PaymentApprovalChainController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ILogger<PaymentApprovalChainController> _logger;

    public PaymentApprovalChainController(IMediator mediator, ILogger<PaymentApprovalChainController> logger)
        : base(mediator, logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all payment approval chains
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<PaymentApprovalChainDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool? isActive,
        [FromQuery] string? paymentType)
    {
        try
        {
            var result = await _mediator.Send(new GetAllPaymentApprovalChainsQry
            {
                IsActive = isActive,
                PaymentType = paymentType
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetAllPaymentApprovalChains");
        }
    }

    /// <summary>
    /// Get payment approval chain by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PaymentApprovalChainDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetPaymentApprovalChainByIdQry { Id = id });
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetPaymentApprovalChainById", id);
        }
    }

    /// <summary>
    /// Create a new payment approval chain
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PaymentApprovalChainDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AddPaymentApprovalChainDto dto)
    {
        try
        {
            var result = await _mediator.Send(new AddPaymentApprovalChainCmd { AddDto = dto });
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "CreatePaymentApprovalChain");
        }
    }

    /// <summary>
    /// Update a payment approval chain
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(PaymentApprovalChainDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] EditPaymentApprovalChainDto dto)
    {
        try
        {
            var result = await _mediator.Send(new EditPaymentApprovalChainCmd { EditDto = dto });
            return Ok(result);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "UpdatePaymentApprovalChain", dto.Id);
        }
    }

    /// <summary>
    /// Delete a payment approval chain
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new DeletePaymentApprovalChainCmd { Id = id });
            if (!result)
                return NotFound(new { message = $"Payment approval chain with ID '{id}' not found" });
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex, "DeletePaymentApprovalChain", id);
        }
    }
}