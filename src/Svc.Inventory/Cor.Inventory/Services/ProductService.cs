using Cor.Inventory.Models.DTOs;
using Cor.Inventory.Models.Entities;
using Cor.Inventory.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cor.Inventory.Services;

public class ProductService : IProductService
{
    private readonly InventoryDbContext _context;
    private readonly ILogger<ProductService> _logger;

    public ProductService(InventoryDbContext context, ILogger<ProductService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<ProductDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Products
            .OrderBy(p => p.Name)
            .Select(p => MapToDto(p))
            .ToListAsync(ct);
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id, ct);
        return product == null ? null : MapToDto(product);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken ct = default)
    {
        var exists = await _context.Products.AnyAsync(p => p.Sku == dto.Sku, ct);
        if (exists)
            throw new InvalidOperationException($"Product with SKU '{dto.Sku}' already exists");

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Sku = dto.Sku,
            Name = dto.Name,
            Description = dto.Description,
            CategoryId = dto.CategoryId,
            UnitId = dto.UnitId,
            UnitPrice = dto.UnitPrice,
            ReorderLevel = dto.ReorderLevel,
            Barcode = dto.Barcode,
            IsActive = dto.IsActive,
            DateAdd = DateTime.UtcNow
        };
        product.UpdateRowVersion();

        await _context.Products.AddAsync(product, ct);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Created product: {ProductName} ({Sku})", product.Name, product.Sku);
        return MapToDto(product);
    }

    public async Task<ProductDto> UpdateAsync(UpdateProductDto dto, CancellationToken ct = default)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == dto.Id, ct);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID '{dto.Id}' not found");

        if (!string.IsNullOrEmpty(dto.RowVersion) && dto.RowVersion != product.RowVersion)
            throw new DbUpdateConcurrencyException("The product was modified by another user");

        if (!string.IsNullOrEmpty(dto.Sku))
        {
            var exists = await _context.Products.AnyAsync(p => p.Sku == dto.Sku && p.Id != dto.Id, ct);
            if (exists)
                throw new InvalidOperationException($"Product with SKU '{dto.Sku}' already exists");
            product.Sku = dto.Sku;
        }
        if (!string.IsNullOrEmpty(dto.Name)) product.Name = dto.Name;
        if (dto.Description != null) product.Description = dto.Description;
        if (dto.CategoryId.HasValue) product.CategoryId = dto.CategoryId.Value;
        if (dto.UnitId.HasValue) product.UnitId = dto.UnitId.Value;
        if (dto.UnitPrice.HasValue) product.UnitPrice = dto.UnitPrice.Value;
        if (dto.ReorderLevel.HasValue) product.ReorderLevel = dto.ReorderLevel;
        if (dto.Barcode != null) product.Barcode = dto.Barcode;
        if (dto.IsActive.HasValue) product.IsActive = dto.IsActive.Value;

        product.DateMod = DateTime.UtcNow;
        product.UpdateRowVersion();

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Updated product: {ProductName} ({Sku})", product.Name, product.Sku);
        return MapToDto(product);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (product == null)
            return false;

        product.IsDeleted = true;
        product.DateMod = DateTime.UtcNow;
        product.UpdateRowVersion();

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Deleted product: {ProductName} ({Sku})", product.Name, product.Sku);
        return true;
    }

    private static ProductDto MapToDto(Product p) => new()
    {
        Id = p.Id,
        Sku = p.Sku,
        Name = p.Name,
        Description = p.Description,
        CategoryId = p.CategoryId,
        UnitId = p.UnitId,
        UnitPrice = p.UnitPrice,
        ReorderLevel = p.ReorderLevel,
        Barcode = p.Barcode,
        IsActive = p.IsActive,
        DateAdd = p.DateAdd,
        DateMod = p.DateMod
    };
}
