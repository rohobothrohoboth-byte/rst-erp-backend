// Cor.Finance.Queries - AmendmentQueries.cs

using MediatR;
using Cor.Finance.Models.DTOs;

namespace Cor.Finance.Queries;

public class GetAmendmentsQuery : IRequest<List<AmendmentDto>>
{
    public Guid InvoiceId { get; set; }
}