using Cor.Inventory.Models.DTOs;

namespace Cor.Inventory.Services;

public interface IWarehouseZoneService
{
    Task<List<WarehouseZoneDto>> GetZonesAsync(Guid? warehouseId, CancellationToken ct = default);
    Task<WarehouseZoneDto?> GetZoneByIdAsync(Guid id, CancellationToken ct = default);
    Task<WarehouseZoneDto> CreateZoneAsync(CreateWarehouseZoneDto dto, CancellationToken ct = default);
    Task<WarehouseZoneDto> UpdateZoneAsync(UpdateWarehouseZoneDto dto, CancellationToken ct = default);
    Task<bool> DeleteZoneAsync(Guid id, CancellationToken ct = default);

    Task<List<BinDto>> GetBinsAsync(Guid zoneId, CancellationToken ct = default);
    Task<BinDto> CreateBinAsync(Guid zoneId, CreateBinDto dto, CancellationToken ct = default);

    Task<WarehouseLayoutDto> GetLayoutAsync(Guid warehouseId, CancellationToken ct = default);
}
