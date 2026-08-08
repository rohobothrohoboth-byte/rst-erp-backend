using MediatR;
using Cor.Procurement.Models.DTOs;

namespace Cor.Procurement.Queries;

public class GetAllInspectionsQuery : IRequest<List<InspectionDto>>
{
    public string? Status { get; set; }
    public string? InspectorId { get; set; }
    public Guid? GrnId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class GetInspectionByIdQuery : IRequest<InspectionDto>
{
    public Guid Id { get; set; }
}

public class GetInspectionsByGrnQuery : IRequest<List<InspectionDto>>
{
    public Guid GrnId { get; set; }
}