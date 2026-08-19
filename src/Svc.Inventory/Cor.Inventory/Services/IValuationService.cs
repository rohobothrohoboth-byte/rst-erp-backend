using Cor.Inventory.Models.DTOs;

namespace Cor.Inventory.Services;

public interface IValuationService
{
    Task<ValuationMethodDto> GetMethodAsync(CancellationToken ct = default);
    Task<ValuationMethodDto> SetMethodAsync(ValuationMethodDto dto, CancellationToken ct = default);
    Task<ValuationReportDto> GetReportAsync(Guid? warehouseId, CancellationToken ct = default);
}
