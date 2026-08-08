using Cor.Procurement.Models.DTOs;

namespace Cor.Procurement.Services;

public interface IInventoryApiService
{
    Task<List<WarehouseDto>> GetAllWarehousesAsync(CancellationToken ct = default);
    Task<WarehouseDto?> GetWarehouseAsync(Guid id, CancellationToken ct = default);
}