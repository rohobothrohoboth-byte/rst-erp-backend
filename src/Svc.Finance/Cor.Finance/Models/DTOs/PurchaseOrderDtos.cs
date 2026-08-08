// Svc.Finance.Models.DTOs - PurchaseOrderDtos.cs

using System;
using System.Collections.Generic;

namespace Cor.Finance.Models.DTOs;

public class PurchaseOrderDto
{
    public Guid Id { get; set; }
    public string PurchaseOrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public Guid VendorId { get; set; }
    public string? VendorName { get; set; }
    public string? Description { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Draft";
    public string? Currency { get; set; }
    public DateTime? ReceivedDate { get; set; }
    public Guid? ReceivedBy { get; set; }

    // ✅ ADDED - PeriodId is REQUIRED
    public Guid? PeriodId { get; set; }
    public string? PeriodName { get; set; }  // For display

    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public List<PurchaseOrderLineDto> Lines { get; set; } = new();
}

public class PurchaseOrderLineDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? Discount { get; set; }
    public decimal? TaxRate { get; set; }

    // ✅ ADDED - PeriodId is REQUIRED (denormalized for performance)
    public Guid? PeriodId { get; set; }
    public string? PeriodName { get; set; }
}

public class AddPurchaseOrderDto
{
    public string PurchaseOrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public Guid VendorId { get; set; }
    public string? VendorName { get; set; }
    public string? Description { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Draft";
    public string? Currency { get; set; }

    // ✅ ADDED - PeriodId is REQUIRED
    public Guid? PeriodId { get; set; }

    public List<AddPurchaseOrderLineDto> Lines { get; set; } = new();
}

public class AddPurchaseOrderLineDto
{
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? Discount { get; set; }
    public decimal? TaxRate { get; set; }

    // ✅ ADDED - PeriodId is REQUIRED
    public Guid? PeriodId { get; set; }
}

public class EditPurchaseOrderDto
{
    public Guid Id { get; set; }
    public string PurchaseOrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public Guid VendorId { get; set; }
    public string? VendorName { get; set; }
    public string? Description { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Draft";
    public string? Currency { get; set; }

    // ✅ ADDED - PeriodId is REQUIRED
    public Guid? PeriodId { get; set; }

    public List<EditPurchaseOrderLineDto> Lines { get; set; } = new();
    public string RowVersion { get; set; } = string.Empty;
}
// Models/DTOs/PurchaseOrderDtos.cs - Add this class

public class ReceivePurchaseOrderDto
{
    public DateTime? ReceivedDate { get; set; }
    public Guid? ReceivedBy { get; set; }
}
public class EditPurchaseOrderLineDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? Discount { get; set; }
    public decimal? TaxRate { get; set; }

    // ✅ ADDED - PeriodId is REQUIRED
    public Guid? PeriodId { get; set; }
}