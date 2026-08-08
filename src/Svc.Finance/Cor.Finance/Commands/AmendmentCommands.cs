// Cor.Finance.Commands - AmendmentCommands.cs

using MediatR;
using Cor.Finance.Models.DTOs;

namespace Cor.Finance.Commands;

public class RequestAmendmentCommand : IRequest<AmendmentDto>
{
    public AmendmentRequestDto Dto { get; set; } = default!;
}

public class ApproveAmendmentCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public AmendmentApproveDto Dto { get; set; } = default!;
}

public class RejectAmendmentCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public AmendmentRejectDto Dto { get; set; } = default!;
}