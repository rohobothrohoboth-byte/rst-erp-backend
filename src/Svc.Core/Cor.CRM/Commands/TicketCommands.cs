// Cor.CRM/Commands/TicketCommands.cs
using Cor.CRM.Models.DTOs;
using MediatR;

namespace Cor.CRM.Commands;

public class TicketAddCmd : IRequest<TicketDto>
{
    public CreateTicketDto Dto { get; set; } = default!;
}

public class TicketUpdateCmd : IRequest<TicketDto>
{
    public Guid Id { get; set; }
    public UpdateTicketDto Dto { get; set; } = default!;
}

public class TicketUpdateStatusCmd : IRequest<TicketDto>
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class TicketAssignCmd : IRequest<TicketDto>
{
    public Guid Id { get; set; }
    public Guid AssignedToUserId { get; set; }
}

public class TicketResolveCmd : IRequest<TicketDto>
{
    public Guid Id { get; set; }
    public string? Resolution { get; set; }
}

public class TicketCloseCmd : IRequest<TicketDto>
{
    public Guid Id { get; set; }
    public int? SatisfactionScore { get; set; }
}

public class TicketDeleteCmd : IRequest
{
    public Guid Id { get; set; }
}