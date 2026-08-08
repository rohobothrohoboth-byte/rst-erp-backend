// Cor.CRM/Queries/TicketQueries.cs
using Cor.CRM.Models.DTOs;
using MediatR;

namespace Cor.CRM.Queries;

public class TicketAllQry : IRequest<List<TicketDto>>
{
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public string? Source { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public string? Category { get; set; }
    public string? SubCategory { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class TicketByIdQry : IRequest<TicketDto?>
{
    public Guid Id { get; set; }
}

public class TicketStatsQry : IRequest<TicketStatsResponse>
{
}