// Models/DTOs/PurchaseOrderDto.cs
using System;
using System.Collections.Generic;

namespace Cor.Procurement.Models.DTOs;

// ============================================================
// PURCHASE ORDER DTO
// ============================================================

public class PurchaseOrderDto
{
    public Guid Id { get; set; }
    public string PurchaseOrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public Guid? VendorId { get; set; }
    public string? VendorName { get; set; }
    public string? Description { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Draft";
    public string Currency { get; set; } = "USD";
    public DateTime? ReceivedDate { get; set; }
    public string? ReceivedBy { get; set; }
    public Guid? RequisitionId { get; set; }
    public string? RequisitionNumber { get; set; }
    public string? PaymentTerms { get; set; }
    public string? ShippingAddress { get; set; }
    public DateTime? SentDate { get; set; }
    public Guid? SentBy { get; set; }
    public DateTime? ConfirmedDate { get; set; }
    public Guid? ConfirmedBy { get; set; }
    public Guid? PeriodId { get; set; }
    public string? PeriodName { get; set; }

    // Audit fields
    public Guid? CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public Guid? UpdatedByUserId { get; set; }
    public string? UpdatedByUserName { get; set; }

    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public string? RowVersion { get; set; }

    // Navigation
    public List<PurchaseOrderLineDto> Lines { get; set; } = new();
}

// ============================================================
// PURCHASE ORDER LINE DTO
// ============================================================

public class PurchaseOrderLineDto
{
    public Guid Id { get; set; }
    public Guid? PurchaseOrderId { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? Discount { get; set; }
    public decimal? TaxRate { get; set; }
    public decimal? TaxAmount { get; set; }
    public string? UnitOfMeasure { get; set; }
    public Guid? RequisitionLineId { get; set; }
    public Guid? PeriodId { get; set; }
    public string? PeriodName { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}

// ============================================================
// CREATE DTOs
// ============================================================

public class CreatePurchaseOrderDto
{
    public string PurchaseOrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public Guid? VendorId { get; set; }
    public string? VendorName { get; set; }
    public string? Description { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Status { get; set; } = "Draft";
    public string? Currency { get; set; } = "USD";
    public string? PaymentTerms { get; set; }
    public string? ShippingAddress { get; set; }
    public Guid? RequisitionId { get; set; }
    public string? RequisitionNumber { get; set; }
    public Guid? PeriodId { get; set; }

    // Audit fields (will be set by backend from token)
    public Guid? CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }

    public List<CreatePurchaseOrderLineDto> Lines { get; set; } = new();
}

public class CreatePurchaseOrderLineDto
{
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? Discount { get; set; }
    public decimal? TaxRate { get; set; }
    public decimal? TaxAmount { get; set; }
    public string? UnitOfMeasure { get; set; }
    public Guid? RequisitionLineId { get; set; }
    public Guid? PeriodId { get; set; }
}

// ============================================================
// UPDATE DTOs
// ============================================================

public class UpdatePurchaseOrderDto
{
    public Guid Id { get; set; }
    public string PurchaseOrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public Guid? VendorId { get; set; }
    public string? VendorName { get; set; }
    public string? Description { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Status { get; set; }
    public string? Currency { get; set; }
    public string? PaymentTerms { get; set; }
    public string? ShippingAddress { get; set; }
    public Guid? RequisitionId { get; set; }
    public string? RequisitionNumber { get; set; }
    public Guid? PeriodId { get; set; }

    // Audit fields (will be set by backend from token)
    public Guid? UpdatedByUserId { get; set; }
    public string? UpdatedByUserName { get; set; }

    public string? RowVersion { get; set; }
    public List<UpdatePurchaseOrderLineDto> Lines { get; set; } = new();
}

public class UpdatePurchaseOrderLineDto
{
    public Guid? Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? Discount { get; set; }
    public decimal? TaxRate { get; set; }
    public decimal? TaxAmount { get; set; }
    public string? UnitOfMeasure { get; set; }
    public Guid? RequisitionLineId { get; set; }
    public Guid? PeriodId { get; set; }
}

// ============================================================
// STATUS UPDATE DTO
// ============================================================

public class PurchaseOrderStatusUpdateDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

// ============================================================
// RECEIVE DTO
// ============================================================

public class ReceivePurchaseOrderDto
{
    public DateTime? ReceivedDate { get; set; }
    public string? ReceivedBy { get; set; }
    public string? Notes { get; set; }
}

// ============================================================
// FILTER DTO
// ============================================================

public class PurchaseOrderFilterDto
{
    public string? Status { get; set; }
    public Guid? VendorId { get; set; }
    public Guid? PeriodId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; } = "OrderDate";
    public string? SortOrder { get; set; } = "DESC";
}