using Cor.Inventory.Models.DTOs;

namespace Cor.Inventory.Services;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync(CancellationToken ct = default);
    Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<CategoryDto> CreateAsync(CreateCategoryDto dto, CancellationToken ct = default);
    Task<CategoryDto> UpdateAsync(UpdateCategoryDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
