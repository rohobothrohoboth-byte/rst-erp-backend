using MediatR;
using Cor.Procurement.Models.DTOs;

namespace Cor.Procurement.Commands;

public class CreateGoodsReceiptNoteCommand : IRequest<GoodsReceiptNoteDto>
{
    public CreateGoodsReceiptNoteDto CreateDto { get; set; } = new();
}

public class UpdateGoodsReceiptNoteCommand : IRequest<GoodsReceiptNoteDto>
{
    public UpdateGoodsReceiptNoteDto UpdateDto { get; set; } = new();
}

public class DeleteGoodsReceiptNoteCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class CompleteGoodsReceiptNoteCommand : IRequest<GoodsReceiptNoteDto>
{
    public Guid Id { get; set; }
}