// Cor.Finance.Models.DTOs - AmendmentDto.cs

using System;

namespace Cor.Finance.Models.DTOs;

public class AmendmentDto
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public decimal OriginalSubTotal { get; set; }
    public decimal OriginalTaxAmount { get; set; }
    public decimal OriginalTotalAmount { get; set; }
    public decimal RequestedSubTotal { get; set; }
    public decimal RequestedTaxAmount { get; set; }
    public decimal RequestedTotalAmount { get; set; }
    public string? Comment { get; set; }
    public string Status { get; set; } = string.Empty;
    public string RequestedBy { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? RejectionReason { get; set; }
}

public class AmendmentRequestDto
{
    public Guid InvoiceId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public decimal RequestedSubTotal { get; set; }
    public decimal RequestedTaxAmount { get; set; }
    public decimal RequestedTotalAmount { get; set; }
    public string? Comment { get; set; }
}

public class AmendmentApproveDto
{
    public string? Comment { get; set; }
}

public class AmendmentRejectDto
{
    public string Reason { get; set; } = string.Empty;
}

public class AmendmentResponseDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime RequestedDate { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedDate { get; set; }
    public string? RejectionReason { get; set; }
}