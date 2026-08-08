using MediatR;
using Cor.Procurement.Models.DTOs;

namespace Cor.Procurement.Commands;

public class CreateInvoiceCommand : IRequest<InvoiceDto>
{
    public CreateInvoiceDto CreateDto { get; set; } = new();
}

public class UpdateInvoiceStatusCommand : IRequest<InvoiceDto>
{
    public InvoiceStatusUpdateDto StatusDto { get; set; } = new();
}