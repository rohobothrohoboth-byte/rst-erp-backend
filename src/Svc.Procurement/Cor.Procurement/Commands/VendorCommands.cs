using MediatR;
using Cor.Procurement.Models.DTOs;

namespace Cor.Procurement.Commands;

public class CreateVendorCommand : IRequest<VendorDto>
{
    public CreateVendorDto CreateDto { get; set; } = new();
}

public class UpdateVendorCommand : IRequest<VendorDto>
{
    public UpdateVendorDto UpdateDto { get; set; } = new();
}

public class DeleteVendorCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class ToggleVendorStatusCommand : IRequest<VendorDto>
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
}