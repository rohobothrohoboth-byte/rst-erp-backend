using MediatR;
using Cor.Procurement.Models.DTOs;

namespace Cor.Procurement.Queries;

public class GetAllVendorEvaluationsQuery : IRequest<List<VendorEvaluationDto>>
{
    public Guid? VendorId { get; set; }
    public string? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class GetVendorEvaluationByIdQuery : IRequest<VendorEvaluationDto>
{
    public Guid Id { get; set; }
}

public class GetVendorEvaluationsByVendorQuery : IRequest<List<VendorEvaluationDto>>
{
    public Guid VendorId { get; set; }
}