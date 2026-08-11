using Cor.Inventory.Models.DTOs;

namespace Cor.Inventory.Services;

public interface IProductService
{
    Task<List<ProductDto>> GetAllAsync(CancellationToken ct = default);
    Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken ct = default);
    Task<ProductDto> UpdateAsync(UpdateProductDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
