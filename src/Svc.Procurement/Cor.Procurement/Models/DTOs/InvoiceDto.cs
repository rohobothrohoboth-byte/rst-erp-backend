namespace Cor.Procurement.Models.DTOs;

public class InvoiceDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid PurchaseOrderId { get; set; }
    public string? PurchaseOrderNumber { get; set; }
    public Guid VendorId { get; set; }
    public string? VendorName { get; set; }
    public string? Title { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReceivedDate { get; set; }
    public decimal NetAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Draft";
    public string? PaymentTerms { get; set; }
    public string? Notes { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string? PaidBy { get; set; }
    public DateTime? PaidDate { get; set; }
    public int AttachmentCount { get; set; }
    public List<InvoiceLineItemDto> LineItems { get; set; } = new();
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public string? RowVersion { get; set; }
}

public class InvoiceLineItemDto
{
    public Guid? Id { get; set; }
    public Guid PurchaseOrderItemId { get; set; }
    public string? Description { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? Discount { get; set; }
    public decimal? TaxAmount { get; set; }
}

public class CreateInvoiceDto
{
    public Guid PurchaseOrderId { get; set; }
    public string? Title { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public string? PaymentTerms { get; set; }
    public string? Notes { get; set; }
    public List<CreateInvoiceLineItemDto> LineItems { get; set; } = new();
}

public class CreateInvoiceLineItemDto
{
    public Guid PurchaseOrderItemId { get; set; }
    public string? Description { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? Discount { get; set; }
    public decimal? TaxAmount { get; set; }
}

public class UpdateInvoiceDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public DateTime? DueDate { get; set; }
    public string? PaymentTerms { get; set; }
    public string? Notes { get; set; }
    public string? RowVersion { get; set; }
}

public class ApproveInvoiceDto
{
    public Guid Id { get; set; }
    public string? ApprovedBy { get; set; }
    public string? Notes { get; set; }
}

public class InvoiceStatusUpdateDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}