using Cor.Inventory.Models.DTOs;
using Cor.Inventory.Models.Entities;

namespace Cor.Inventory.Services;

public interface IStockService
{
    // Movements
    Task<List<StockMovementDto>> GetMovementsAsync(string? type, Guid? warehouseId, CancellationToken ct = default);
    Task<List<StockLevel>> GetStockLevelsAsync(Guid? warehouseId, CancellationToken ct = default);

    Task<StockMovementDto> InboundAsync(InboundStockDto dto, CancellationToken ct = default);
    Task<StockMovementDto> OutboundAsync(OutboundStockDto dto, CancellationToken ct = default);
    Task<StockMovementDto> TransferAsync(TransferStockDto dto, CancellationToken ct = default);
    Task<StockMovementDto> AdjustmentAsync(AdjustmentStockDto dto, CancellationToken ct = default);

    // Stock counts
    Task<StockCountDto> CreateCountAsync(CreateStockCountDto dto, CancellationToken ct = default);
    Task<List<StockCountDto>> GetCountsAsync(CancellationToken ct = default);
    Task<StockCountDto?> GetCountAsync(Guid id, CancellationToken ct = default);
    Task<StockCountDto> RecordCountAsync(Guid id, RecordStockCountDto dto, CancellationToken ct = default);
    Task<StockCountDto> ReconcileCountAsync(Guid id, CancellationToken ct = default);
}
