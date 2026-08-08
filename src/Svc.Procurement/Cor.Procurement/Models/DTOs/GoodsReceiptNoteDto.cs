using System;
using System.Collections.Generic;

namespace Cor.Procurement.Models.DTOs;

public class GoodsReceiptNoteDto
{
    public Guid Id { get; set; }
    public string GrnNumber { get; set; } = string.Empty;
    public Guid PurchaseOrderId { get; set; }
    public string? PurchaseOrderNumber { get; set; }
    public string? DeliveryNoteNumber { get; set; }
    public DateTime ReceivedDate { get; set; }
    public Guid WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public string ReceivedBy { get; set; } = string.Empty;
    public string? InspectedBy { get; set; }
    public string Status { get; set; } = "Draft";
    public decimal TotalReceived { get; set; }
    public decimal TotalAccepted { get; set; }
    public decimal TotalRejected { get; set; }
    public string? Notes { get; set; }
    public DateTime? CompletedDate { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public string? RowVersion { get; set; }
    public List<GoodsReceiptItemDto> Items { get; set; } = new();
}

public class GoodsReceiptItemDto
{
    public Guid? Id { get; set; }
    public Guid PurchaseOrderItemId { get; set; }
    public string? Description { get; set; }
    public int QuantityReceived { get; set; }
    public int QuantityAccepted { get; set; }
    public int QuantityRejected { get; set; }
    public string? Condition { get; set; }
    public string? RejectionReason { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? TotalAmount { get; set; }
}

public class CreateGoodsReceiptNoteDto
{
    public Guid PurchaseOrderId { get; set; }
    public string? DeliveryNoteNumber { get; set; }
    public DateTime ReceivedDate { get; set; }
    public Guid WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public string ReceivedBy { get; set; } = string.Empty;
    public string? InspectedBy { get; set; }
    public string? Notes { get; set; }
    public List<CreateGoodsReceiptItemDto> Items { get; set; } = new();
}

public class CreateGoodsReceiptItemDto
{
    public Guid PurchaseOrderItemId { get; set; }
    public string? Description { get; set; }
    public int QuantityReceived { get; set; }
    public int QuantityAccepted { get; set; }
    public int QuantityRejected { get; set; }
    public string? Condition { get; set; }
    public string? RejectionReason { get; set; }
    public decimal? UnitPrice { get; set; }
}

// ✅ ADD THIS - Update DTOs
public class UpdateGoodsReceiptNoteDto
{
    public Guid Id { get; set; }
    public string? DeliveryNoteNumber { get; set; }
    public DateTime ReceivedDate { get; set; }
    public Guid WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public string? ReceivedBy { get; set; }
    public string? InspectedBy { get; set; }
    public string? Notes { get; set; }
    public string? RowVersion { get; set; }
    public List<UpdateGoodsReceiptItemDto> Items { get; set; } = new();
}

public class UpdateGoodsReceiptItemDto
{
    public Guid? Id { get; set; }
    public Guid PurchaseOrderItemId { get; set; }
    public string? Description { get; set; }
    public int QuantityReceived { get; set; }
    public int QuantityAccepted { get; set; }
    public int QuantityRejected { get; set; }
    public string? Condition { get; set; }
    public string? RejectionReason { get; set; }
    public decimal? UnitPrice { get; set; }
}