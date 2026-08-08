using MediatR;
using Cor.Procurement.Models.DTOs;

namespace Cor.Procurement.Commands;

public class GenerateReportCommand : IRequest<GenerateReportResponseDto>
{
    public CreateReportDto CreateDto { get; set; } = new();
}
public class DeleteReportCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}