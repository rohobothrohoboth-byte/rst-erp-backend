using Cor.Inventory.Models.DTOs;
using Cor.Inventory.Models.Entities;
using Cor.Inventory.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cor.Inventory.Services;

public class CategoryService : ICategoryService
{
    private readonly InventoryDbContext _context;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(InventoryDbContext context, ILogger<CategoryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<CategoryDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Categories
            .OrderBy(c => c.Name)
            .Select(c => MapToDto(c))
            .ToListAsync(ct);
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);
        return category == null ? null : MapToDto(category);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto, CancellationToken ct = default)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Code = dto.Code,
            Description = dto.Description,
            ParentId = dto.ParentId,
            IsActive = dto.IsActive,
            DateAdd = DateTime.UtcNow
        };
        category.UpdateRowVersion();

        await _context.Categories.AddAsync(category, ct);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Created category: {CategoryName}", category.Name);
        return MapToDto(category);
    }

    public async Task<CategoryDto> UpdateAsync(UpdateCategoryDto dto, CancellationToken ct = default)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == dto.Id, ct);
        if (category == null)
            throw new KeyNotFoundException($"Category with ID '{dto.Id}' not found");

        if (!string.IsNullOrEmpty(dto.RowVersion) && dto.RowVersion != category.RowVersion)
            throw new DbUpdateConcurrencyException("The category was modified by another user");

        if (!string.IsNullOrEmpty(dto.Name)) category.Name = dto.Name;
        if (dto.Code != null) category.Code = dto.Code;
        if (dto.Description != null) category.Description = dto.Description;
        if (dto.ParentId.HasValue) category.ParentId = dto.ParentId;
        if (dto.IsActive.HasValue) category.IsActive = dto.IsActive.Value;

        category.DateMod = DateTime.UtcNow;
        category.UpdateRowVersion();

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Updated category: {CategoryName}", category.Name);
        return MapToDto(category);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (category == null)
            return false;

        category.IsDeleted = true;
        category.DateMod = DateTime.UtcNow;
        category.UpdateRowVersion();

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Deleted category: {CategoryName}", category.Name);
        return true;
    }

    private static CategoryDto MapToDto(Category c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Code = c.Code,
        Description = c.Description,
        ParentId = c.ParentId,
        IsActive = c.IsActive,
        DateAdd = c.DateAdd,
        DateMod = c.DateMod
    };
}
