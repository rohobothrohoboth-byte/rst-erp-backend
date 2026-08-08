// Cor.CRM/Models/DTOs/OrderDtos.cs

namespace Cor.CRM.Models.DTOs;

public class CreateOrderDto
{
    public Guid CustomerId { get; set; }
    public Guid? OpportunityId { get; set; }
    public Guid? QuoteId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal TotalAmount { get; set; }
    public string? ShippingAddress { get; set; }
    public string? BillingAddress { get; set; }
    public string? Terms { get; set; }
    public string? Notes { get; set; }
    public string? Currency { get; set; }
    public List<CreateOrderLineDto> OrderLines { get; set; } = new();
}

public class CreateOrderLineDto
{
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal? Discount { get; set; }
    public decimal? TaxRate { get; set; }
    public Guid? ProductId { get; set; }
    public string? Notes { get; set; }
}

public class OrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public Guid? OpportunityId { get; set; }
    public string? OpportunityName { get; set; }
    public Guid? QuoteId { get; set; }
    public string? QuoteNumber { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal TotalAmount { get; set; }
    public string? ShippingAddress { get; set; }
    public string? BillingAddress { get; set; }
    public string? Terms { get; set; }
    public string? Notes { get; set; }
    public string? Currency { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<OrderLineDto> OrderLines { get; set; } = new();
}

public class OrderLineDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TotalPrice { get; set; }
    public int SortOrder { get; set; }
    public Guid? ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? Notes { get; set; }
}
public class UpdateOrderLineDto
{
    public Guid? Id { get; set; }
    public Guid? ProductId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? Discount { get; set; }
    public decimal? TaxRate { get; set; }
    public string? Notes { get; set; }
}
public class UpdateOrderDto
{
    public DateTime? DueDate { get; set; }
    public DateTime? ShippedDate { get; set; }
    public DateTime? DeliveredDate { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal? ShippingCost { get; set; }
    public string? ShippingAddress { get; set; }
    public string? BillingAddress { get; set; }
    public string? Terms { get; set; }
    public string? Notes { get; set; }
    public string? TrackingNumber { get; set; }
    public string? Carrier { get; set; }
    public List<UpdateOrderLineDto>? OrderLines { get; set; }
}
