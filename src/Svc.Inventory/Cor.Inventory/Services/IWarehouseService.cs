using Cor.Inventory.Models.DTOs;

namespace Cor.Inventory.Services;

public interface IWarehouseService
{
    Task<List<WarehouseDto>> GetAllWarehousesAsync(CancellationToken ct = default);
    Task<WarehouseDto?> GetWarehouseByIdAsync(Guid id, CancellationToken ct = default);
    Task<WarehouseDto> CreateWarehouseAsync(CreateWarehouseDto dto, CancellationToken ct = default);
    Task<WarehouseDto> UpdateWarehouseAsync(UpdateWarehouseDto dto, CancellationToken ct = default);
    Task<bool> DeleteWarehouseAsync(Guid id, CancellationToken ct = default);
}