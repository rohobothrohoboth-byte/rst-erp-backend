// Cor.Finance.Controllers - AmendmentController.cs

using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Cor.Finance.Commands;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Queries;

namespace Cor.Finance.Controllers;

[ApiController]
[Route("api/finance/v{version:apiVersion}/Invoice/Amendment")]
[ApiVersion("1.0")]
public class AmendmentController : ControllerBase
{
    private readonly IMediator _mediator;

    public AmendmentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Request an amendment for an invoice
    /// </summary>
    [HttpPost("Request")]
    [ProducesResponseType(typeof(ApiResponse<AmendmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RequestAmendment([FromBody] AmendmentRequestDto dto)
    {
        var amendment = await _mediator.Send(new RequestAmendmentCommand { Dto = dto });

        return Ok(new ApiResponse<AmendmentDto>
        {
            Success = true,
            Message = "Amendment request submitted successfully",
            Data = amendment
        });
    }

    /// <summary>
    /// Approve an amendment
    /// </summary>
    [HttpPost("{id}/approve")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ApproveAmendment(Guid id, [FromBody] AmendmentApproveDto dto)
    {
        var isApproved = await _mediator.Send(new ApproveAmendmentCommand { Id = id, Dto = dto });

        return Ok(new ApiResponse<bool>
        {
            Success = true,
            Message = "Amendment approved successfully",
            Data = isApproved
        });
    }

    /// <summary>
    /// Reject an amendment
    /// </summary>
    [HttpPost("{id}/reject")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RejectAmendment(Guid id, [FromBody] AmendmentRejectDto dto)
    {
        var isRejected = await _mediator.Send(new RejectAmendmentCommand { Id = id, Dto = dto });

        return Ok(new ApiResponse<bool>
        {
            Success = true,
            Message = "Amendment rejected",
            Data = isRejected
        });
    }

    /// <summary>
    /// Get all amendments for an invoice
    /// </summary>
    [HttpGet("{invoiceId}")]
    [ProducesResponseType(typeof(ApiResponse<List<AmendmentDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAmendments(Guid invoiceId)
    {
        var amendments = await _mediator.Send(new GetAmendmentsQuery { InvoiceId = invoiceId });

        return Ok(new ApiResponse<List<AmendmentDto>>
        {
            Success = true,
            Message = "Amendments retrieved successfully",
            Data = amendments
        });
    }
}