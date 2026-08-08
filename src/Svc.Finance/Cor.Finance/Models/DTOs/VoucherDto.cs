// Models/DTOs/VoucherDto.cs
using System;
using System.Collections.Generic;
namespace Cor.Finance.Models.DTOs;

public class VoucherDto
{
    public Guid Id { get; set; }
    public string VoucherNumber { get; set; } = string.Empty;
    public string VoucherType { get; set; } = string.Empty; // Payment, Receipt, Journal, Contra, Transfer
    public Guid? VendorId { get; set; }
    public string? VendorName { get; set; }
    public DateTime VoucherDate { get; set; }
    public string? Description { get; set; }
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public string Status { get; set; } = "Draft"; // Draft, Pending, Approved, Posted, Rejected, Void
    public Guid? PeriodId { get; set; }
    public string? PeriodName { get; set; }
    public List<VoucherLineDto> Lines { get; set; } = new();
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? PostedBy { get; set; }
    public DateTime? PostedAt { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public string? RowVersion { get; set; }
}

public class VoucherLineDto
{
    public Guid? Id { get; set; }
    public Guid AccountId { get; set; }
    public string? AccountName { get; set; }
    public string? AccountCode { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public Guid? PeriodId { get; set; }
}

public class AddVoucherDto
{
    public string VoucherType { get; set; } = "Journal";
    public Guid? VendorId { get; set; }
    public DateTime VoucherDate { get; set; } = DateTime.UtcNow;
    public string? Description { get; set; }
    public Guid PeriodId { get; set; }
    public List<AddVoucherLineDto> Lines { get; set; } = new();
}

public class EditVoucherDto
{
    public Guid Id { get; set; }
    public string VoucherType { get; set; } = "Journal";
    public Guid? VendorId { get; set; }
    public DateTime VoucherDate { get; set; }
    public string? Description { get; set; }
    public Guid PeriodId { get; set; }
    public List<EditVoucherLineDto> Lines { get; set; } = new();
    public string? RowVersion { get; set; }
}

public class AddVoucherLineDto
{
    public Guid AccountId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
}

public class EditVoucherLineDto
{
    public Guid? Id { get; set; }
    public Guid AccountId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
}

public class RejectVoucherDto
{
    public string Reason { get; set; } = string.Empty;
}