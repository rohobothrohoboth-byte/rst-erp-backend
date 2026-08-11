using Cor.Inventory.Models.DTOs;
using Cor.Inventory.Models.Entities;
using Cor.Inventory.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cor.Inventory.Services;

public class UnitService : IUnitService
{
    private readonly InventoryDbContext _context;
    private readonly ILogger<UnitService> _logger;

    public UnitService(InventoryDbContext context, ILogger<UnitService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<UnitDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Units
            .OrderBy(u => u.Name)
            .Select(u => MapToDto(u))
            .ToListAsync(ct);
    }

    public async Task<UnitDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var unit = await _context.Units.FirstOrDefaultAsync(u => u.Id == id, ct);
        return unit == null ? null : MapToDto(unit);
    }

    public async Task<UnitDto> CreateAsync(CreateUnitDto dto, CancellationToken ct = default)
    {
        var unit = new Unit
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Symbol = dto.Symbol,
            Description = dto.Description,
            IsActive = dto.IsActive,
            DateAdd = DateTime.UtcNow
        };
        unit.UpdateRowVersion();

        await _context.Units.AddAsync(unit, ct);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Created unit: {UnitName}", unit.Name);
        return MapToDto(unit);
    }

    public async Task<UnitDto> UpdateAsync(UpdateUnitDto dto, CancellationToken ct = default)
    {
        var unit = await _context.Units.FirstOrDefaultAsync(u => u.Id == dto.Id, ct);
        if (unit == null)
            throw new KeyNotFoundException($"Unit with ID '{dto.Id}' not found");

        if (!string.IsNullOrEmpty(dto.RowVersion) && dto.RowVersion != unit.RowVersion)
            throw new DbUpdateConcurrencyException("The unit was modified by another user");

        if (!string.IsNullOrEmpty(dto.Name)) unit.Name = dto.Name;
        if (!string.IsNullOrEmpty(dto.Symbol)) unit.Symbol = dto.Symbol;
        if (dto.Description != null) unit.Description = dto.Description;
        if (dto.IsActive.HasValue) unit.IsActive = dto.IsActive.Value;

        unit.DateMod = DateTime.UtcNow;
        unit.UpdateRowVersion();

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Updated unit: {UnitName}", unit.Name);
        return MapToDto(unit);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var unit = await _context.Units.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (unit == null)
            return false;

        unit.IsDeleted = true;
        unit.DateMod = DateTime.UtcNow;
        unit.UpdateRowVersion();

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Deleted unit: {UnitName}", unit.Name);
        return true;
    }

    private static UnitDto MapToDto(Unit u) => new()
    {
        Id = u.Id,
        Name = u.Name,
        Symbol = u.Symbol,
        Description = u.Description,
        IsActive = u.IsActive,
        DateAdd = u.DateAdd,
        DateMod = u.DateMod
    };
}
