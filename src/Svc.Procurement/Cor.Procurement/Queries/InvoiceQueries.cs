using MediatR;
using Cor.Procurement.Models.DTOs;

namespace Cor.Procurement.Queries;

public class GetAllInvoicesQuery : IRequest<List<InvoiceDto>>
{
    public string? Status { get; set; }
    public Guid? VendorId { get; set; }
    public Guid? PurchaseOrderId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class GetInvoiceByIdQuery : IRequest<InvoiceDto>
{
    public Guid Id { get; set; }
}

public class GetInvoicesByPurchaseOrderQuery : IRequest<List<InvoiceDto>>
{
    public Guid PurchaseOrderId { get; set; }
}

public class GetInvoicesByVendorQuery : IRequest<List<InvoiceDto>>
{
    public Guid VendorId { get; set; }
}
public class SearchInvoicesQuery : IRequest<List<InvoiceDto>>
{
    public string? SearchTerm { get; set; }
    public string? Status { get; set; }
}
