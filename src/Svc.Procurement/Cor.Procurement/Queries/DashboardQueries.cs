using MediatR;
using Cor.Procurement.Models.DTOs;

namespace Cor.Procurement.Queries;

public class GetDashboardDataQuery : IRequest<DashboardDto>
{
    public int RecentActivitiesCount { get; set; } = 5;
    public int TopVendorsCount { get; set; } = 5;
}