using MediatR;
using Cor.Procurement.Models.DTOs;

namespace Cor.Procurement.Queries;

public class GetAllVendorsQuery : IRequest<List<VendorDto>>
{
    public string? Status { get; set; }
    public string? VendorType { get; set; }
    public string? SearchTerm { get; set; }
}

public class GetVendorByIdQuery : IRequest<VendorDto>
{
    public Guid Id { get; set; }
}

public class GetVendorByCodeQuery : IRequest<VendorDto>
{
    public string Code { get; set; } = string.Empty;
}