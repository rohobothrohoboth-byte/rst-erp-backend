using MediatR;
using Cor.Procurement.Models.DTOs;

namespace Cor.Procurement.Commands;

public class CreateVendorContractCommand : IRequest<VendorContractDto>
{
    public CreateVendorContractDto CreateDto { get; set; } = new();
}

public class UpdateVendorContractCommand : IRequest<VendorContractDto>
{
    public UpdateVendorContractDto UpdateDto { get; set; } = new();
}

public class DeleteVendorContractCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}