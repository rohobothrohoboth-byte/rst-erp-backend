using Cor.Inventory.Models.DTOs;

namespace Cor.Inventory.Services;

public interface IInvDashboardService
{
    Task<DashboardStatsDto> GetStatsAsync(CancellationToken ct = default);
}
