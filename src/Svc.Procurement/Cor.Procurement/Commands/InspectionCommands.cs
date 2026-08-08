using MediatR;
using Cor.Procurement.Models.DTOs;

namespace Cor.Procurement.Commands;

public class CreateInspectionCommand : IRequest<InspectionDto>
{
    public CreateInspectionDto CreateDto { get; set; } = new();
}

public class CompleteInspectionCommand : IRequest<InspectionDto>
{
    public CompleteInspectionDto CompleteDto { get; set; } = new();
}

public class UpdateInspectionItemCommand : IRequest<InspectionItemDto>
{
    public Guid InspectionId { get; set; }
    public Guid ItemId { get; set; }
    public UpdateInspectionItemDto UpdateDto { get; set; } = new();
}

public class DeleteInspectionCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class UpdateInspectionItemDto
{
    public int QuantityAccepted { get; set; }
    public int QuantityRejected { get; set; }
    public string? Condition { get; set; }
    public string? RejectionReason { get; set; }
}