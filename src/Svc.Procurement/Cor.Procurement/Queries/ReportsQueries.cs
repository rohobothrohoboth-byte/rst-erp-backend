using MediatR;
using Cor.Procurement.Models.DTOs;

namespace Cor.Procurement.Queries;

public class GetReportsDashboardQuery : IRequest<ReportsDashboardDto>
{
    public string? Period { get; set; }
    public int TopVendorsCount { get; set; } = 5;
    public int RecentReportsCount { get; set; } = 10;
}