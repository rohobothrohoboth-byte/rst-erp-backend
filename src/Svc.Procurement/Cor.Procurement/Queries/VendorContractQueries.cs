using MediatR;
using Cor.Procurement.Models.DTOs;

namespace Cor.Procurement.Queries;

public class GetAllVendorContractsQuery : IRequest<List<VendorContractDto>>
{
    public Guid? VendorId { get; set; }
    public string? Status { get; set; }
    public string? Type { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class GetVendorContractByIdQuery : IRequest<VendorContractDto>
{
    public Guid Id { get; set; }
}

public class GetVendorContractsByVendorQuery : IRequest<List<VendorContractDto>>
{
    public Guid VendorId { get; set; }
}