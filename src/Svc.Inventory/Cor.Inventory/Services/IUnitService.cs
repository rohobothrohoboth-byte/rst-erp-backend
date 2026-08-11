using Cor.Inventory.Models.DTOs;

namespace Cor.Inventory.Services;

public interface IUnitService
{
    Task<List<UnitDto>> GetAllAsync(CancellationToken ct = default);
    Task<UnitDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<UnitDto> CreateAsync(CreateUnitDto dto, CancellationToken ct = default);
    Task<UnitDto> UpdateAsync(UpdateUnitDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
