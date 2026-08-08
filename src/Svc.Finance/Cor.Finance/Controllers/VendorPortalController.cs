// Controllers/VendorPortalController.cs
using Cor.Finance.Commands;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Cor.Finance.Helpers;

namespace Cor.Finance.Controllers;

[ApiController]
[Route("api/finance/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class VendorPortalController : BaseApiController
{
    private readonly IMediator _mediator;

    public VendorPortalController(IMediator mediator, ILogger<VendorPortalController> logger)
        : base(mediator, logger)
    {
        _mediator = mediator;
    }

    // ============================================================
    // VENDOR USERS
    // ============================================================

    [HttpGet("Vendors")]
    [ProducesResponseType(typeof(List<PortalVendorUserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllVendors([FromQuery] string? status, [FromQuery] Guid? vendorId)
    {
        try
        {
            var result = await _mediator.Send(new GetAllPortalVendorsQry { Status = status, VendorId = vendorId });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetAllPortalVendors");
        }
    }

    [HttpGet("Vendors/{id}")]
    [ProducesResponseType(typeof(PortalVendorUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVendorById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetPortalVendorByIdQry { Id = id });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetPortalVendorById", id);
        }
    }

    [HttpPost("Vendors")]
    [ProducesResponseType(typeof(PortalVendorUserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateVendor([FromBody] AddPortalVendorUserDto dto)
    {
        try
        {
            var result = await _mediator.Send(new AddPortalVendorUserCmd { AddDto = dto });
            return CreatedAtAction(nameof(GetVendorById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "CreatePortalVendor");
        }
    }

    [HttpPut("Vendors")]
    [ProducesResponseType(typeof(PortalVendorUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateVendor([FromBody] EditPortalVendorUserDto dto)
    {
        try
        {
            var result = await _mediator.Send(new EditPortalVendorUserCmd { EditDto = dto });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "UpdatePortalVendor", dto.Id);
        }
    }

    [HttpDelete("Vendors/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteVendor(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new DeletePortalVendorUserCmd { Id = id });
            if (!result)
                return NotFound(new { message = $"Portal vendor user with ID '{id}' not found" });
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex, "DeletePortalVendor", id);
        }
    }

    // ============================================================
    // PORTAL INVOICES
    // ============================================================

    [HttpGet("Invoices")]
    [ProducesResponseType(typeof(List<PortalInvoiceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllInvoices(
        [FromQuery] string? status,
        [FromQuery] Guid? vendorId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        try
        {
            var result = await _mediator.Send(new GetAllPortalInvoicesQry
            {
                Status = status,
                VendorId = vendorId,
                FromDate = fromDate,
                ToDate = toDate
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetAllPortalInvoices");
        }
    }

    [HttpGet("Invoices/{id}")]
    [ProducesResponseType(typeof(PortalInvoiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInvoiceById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetPortalInvoiceByIdQry { Id = id });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetPortalInvoiceById", id);
        }
    }

    [HttpGet("Invoices/{id}/Tracking")]
    [ProducesResponseType(typeof(PortalInvoiceTrackingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInvoiceTracking(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetPortalInvoiceTrackingQry { Id = id });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetPortalInvoiceTracking", id);
        }
    }

    [HttpPost("Invoices")]
    [ProducesResponseType(typeof(PortalInvoiceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SubmitInvoice([FromBody] AddPortalInvoiceDto dto)
    {
        try
        {
            var result = await _mediator.Send(new AddPortalInvoiceCmd { AddDto = dto });
            return CreatedAtAction(nameof(GetInvoiceById), new { id = result.Id }, new
            {
                success = true,
                message = "Invoice submitted successfully",
                data = result
            });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "SubmitPortalInvoice");
        }
    }

    [HttpPut("Invoices")]
    [ProducesResponseType(typeof(PortalInvoiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateInvoice([FromBody] EditPortalInvoiceDto dto)
    {
        try
        {
            var result = await _mediator.Send(new EditPortalInvoiceCmd { EditDto = dto });
            return Ok(new { success = true, message = "Invoice updated successfully", data = result });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "UpdatePortalInvoice", dto.Id);
        }
    }

    [HttpPost("Invoices/{id}/Approve")]
    [ProducesResponseType(typeof(PortalInvoiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveInvoice(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new ApprovePortalInvoiceCmd { Id = id });
            return Ok(new { success = true, message = "Invoice approved successfully", data = result });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "ApprovePortalInvoice", id);
        }
    }

    [HttpPost("Invoices/{id}/Reject")]
    [ProducesResponseType(typeof(PortalInvoiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RejectInvoice(Guid id, [FromBody] RejectPortalInvoiceDto dto)
    {
        try
        {
            var result = await _mediator.Send(new RejectPortalInvoiceCmd { Id = id, Reason = dto.Reason });
            return Ok(new { success = true, message = "Invoice rejected successfully", data = result });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "RejectPortalInvoice", id);
        }
    }

    [HttpDelete("Invoices/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteInvoice(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new DeletePortalInvoiceCmd { Id = id });
            if (!result)
                return NotFound(new { message = $"Portal invoice with ID '{id}' not found" });
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex, "DeletePortalInvoice", id);
        }
    }

    // ============================================================
    // PORTAL PAYMENTS (NEW - Payment Tracking)
    // ============================================================

    [HttpGet("Payments")]
    [ProducesResponseType(typeof(List<PortalPaymentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPayments(
        [FromQuery] string? status,
        [FromQuery] Guid? vendorId,
        [FromQuery] Guid? invoiceId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        try
        {
            var fromDateUtc = fromDate.HasValue ? DateTimeHelper.EnsureUtc(fromDate.Value) : (DateTime?)null;
            var toDateUtc = toDate.HasValue ? DateTimeHelper.EnsureUtc(toDate.Value) : (DateTime?)null;

            var result = await _mediator.Send(new GetAllPortalPaymentsQry
            {
                Status = status,
                VendorId = vendorId,
                InvoiceId = invoiceId,
                FromDate = fromDateUtc,
                ToDate = toDateUtc
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetAllPortalPayments");
        }
    }

    [HttpGet("Payments/{id}")]
    [ProducesResponseType(typeof(PortalPaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPaymentById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetPortalPaymentByIdQry { Id = id });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetPortalPaymentById", id);
        }
    }

    [HttpGet("Payments/Invoice/{invoiceId}")]
    [ProducesResponseType(typeof(List<PortalPaymentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPaymentsByInvoice(Guid invoiceId)
    {
        try
        {
            var result = await _mediator.Send(new GetPortalPaymentsByInvoiceQry { InvoiceId = invoiceId });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetPortalPaymentsByInvoice", invoiceId);
        }
    }

    [HttpPost("Payments")]
    [ProducesResponseType(typeof(PortalPaymentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePayment([FromBody] AddPortalPaymentDto dto)
    {
        try
        {
            var result = await _mediator.Send(new AddPortalPaymentCmd { AddDto = dto });
            return CreatedAtAction(nameof(GetPaymentById), new { id = result.Id }, new
            {
                success = true,
                message = "Payment recorded successfully",
                data = result
            });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "CreatePortalPayment");
        }
    }

    [HttpPut("Payments")]
    [ProducesResponseType(typeof(PortalPaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePayment([FromBody] EditPortalPaymentDto dto)
    {
        try
        {
            var result = await _mediator.Send(new EditPortalPaymentCmd { EditDto = dto });
            return Ok(new { success = true, message = "Payment updated successfully", data = result });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "UpdatePortalPayment", dto.Id);
        }
    }

    [HttpPatch("Payments/{id}/Status")]
    [ProducesResponseType(typeof(PortalPaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePaymentStatus(Guid id, [FromBody] UpdatePaymentStatusDto dto)
    {
        try
        {
            var result = await _mediator.Send(new UpdatePortalPaymentStatusCmd { Id = id, Status = dto.Status });
            return Ok(new { success = true, message = "Payment status updated successfully", data = result });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "UpdatePortalPaymentStatus", id);
        }
    }

    [HttpPost("Payments/{id}/Process")]
    [ProducesResponseType(typeof(PortalPaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ProcessPayment(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new ProcessPortalPaymentCmd { Id = id });
            return Ok(new { success = true, message = "Payment processed successfully", data = result });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "ProcessPortalPayment", id);
        }
    }

    [HttpDelete("Payments/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePayment(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new DeletePortalPaymentCmd { Id = id });
            if (!result)
                return NotFound(new { message = $"Portal payment with ID '{id}' not found" });
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex, "DeletePortalPayment", id);
        }
    }

    // ============================================================
    // PORTAL PAYMENT SUMMARY / DASHBOARD
    // ============================================================

    [HttpGet("Payments/Summary")]
    [ProducesResponseType(typeof(PortalPaymentSummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaymentSummary([FromQuery] Guid? vendorId)
    {
        try
        {
            var result = await _mediator.Send(new GetPortalPaymentSummaryQry { VendorId = vendorId });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetPortalPaymentSummary");
        }
    }

    // ============================================================
    // PORTAL NOTIFICATIONS
    // ============================================================

    [HttpGet("Notifications")]
    [ProducesResponseType(typeof(List<PortalNotificationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNotifications(
        [FromQuery] Guid? vendorId,
        [FromQuery] bool? isRead,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        try
        {
            var fromDateUtc = fromDate.HasValue ? DateTimeHelper.EnsureUtc(fromDate.Value) : (DateTime?)null;
            var toDateUtc = toDate.HasValue ? DateTimeHelper.EnsureUtc(toDate.Value) : (DateTime?)null;

            var result = await _mediator.Send(new GetAllPortalNotificationsQry
            {
                VendorId = vendorId,
                IsRead = isRead,
                FromDate = fromDateUtc,
                ToDate = toDateUtc
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetAllPortalNotifications");
        }
    }

    [HttpGet("Notifications/{id}")]
    [ProducesResponseType(typeof(PortalNotificationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetNotificationById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetPortalNotificationByIdQry { Id = id });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetPortalNotificationById", id);
        }
    }

    [HttpPost("Notifications/Send")]
    [ProducesResponseType(typeof(PortalNotificationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendNotification([FromBody] SendPortalNotificationDto dto)
    {
        try
        {
            var result = await _mediator.Send(new SendPortalNotificationCmd { SendDto = dto });
            return Ok(new { success = true, message = "Notification sent successfully", data = result });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "SendPortalNotification");
        }
    }

    [HttpPut("Notifications/{id}/Read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkNotificationRead(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new MarkPortalNotificationReadCmd { Id = id });
            if (!result)
                return NotFound(new { message = $"Notification with ID '{id}' not found" });
            return Ok(new { success = true, message = "Notification marked as read" });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "MarkPortalNotificationRead", id);
        }
    }

    [HttpPost("Notifications/MarkAllRead")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> MarkAllNotificationsRead([FromBody] MarkAllPortalNotificationsReadDto dto)
    {
        try
        {
            var result = await _mediator.Send(new MarkAllPortalNotificationsReadCmd { VendorId = dto.VendorId });
            return Ok(new { success = true, message = "All notifications marked as read" });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "MarkAllPortalNotificationsRead");
        }
    }
}

// ============================================================
// ADDITIONAL DTOs
// ============================================================

