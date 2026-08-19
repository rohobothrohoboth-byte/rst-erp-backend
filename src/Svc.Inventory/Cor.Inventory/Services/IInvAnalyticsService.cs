using Cor.Inventory.Models.DTOs;

namespace Cor.Inventory.Services;

public interface IInvAnalyticsService
{
    Task<StockSummaryDto> GetStockSummaryAsync(CancellationToken ct = default);
    Task<MovementAnalysisDto> GetMovementAnalysisAsync(int top = 5, CancellationToken ct = default);
    Task<ForecastDto> GetForecastAsync(int months = 3, CancellationToken ct = default);
}
