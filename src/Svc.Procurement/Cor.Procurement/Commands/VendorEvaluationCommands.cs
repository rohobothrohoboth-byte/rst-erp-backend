using MediatR;
using Cor.Procurement.Models.DTOs;

namespace Cor.Procurement.Commands;

public class CreateVendorEvaluationCommand : IRequest<VendorEvaluationDto>
{
    public CreateVendorEvaluationDto CreateDto { get; set; } = new();
}

public class UpdateVendorEvaluationCommand : IRequest<VendorEvaluationDto>
{
    public UpdateVendorEvaluationDto UpdateDto { get; set; } = new();
}

public class DeleteVendorEvaluationCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}