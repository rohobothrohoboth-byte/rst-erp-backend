using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Commands;

// ============ ENTITY COMMANDS ============
public class AddEntityCmd : IRequest<EntityDto>
{
    public AddEntityDto AddDto { get; set; } = new();
}

public class EditEntityCmd : IRequest<EntityDto>
{
    public EditEntityDto EditDto { get; set; } = new();
}

public class DeleteEntityCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

// ============ ENTITY HANDLERS ============
public class AddEntityHandler : IRequestHandler<AddEntityCmd, EntityDto>
{
    private readonly FinanceDbContext _context;

    public AddEntityHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<EntityDto> Handle(AddEntityCmd request, CancellationToken ct)
    {
        // Check for duplicate code
        var exists = await _context.Entities
            .AnyAsync(x => x.Code == request.AddDto.Code && !x.IsDeleted, ct);
        if (exists)
            throw new InvalidOperationException($"Entity with code '{request.AddDto.Code}' already exists");

        var entity = new Entity
        {
            Id = Guid.NewGuid(),
            Code = request.AddDto.Code,
            Name = request.AddDto.Name,
            LegalName = request.AddDto.LegalName ?? request.AddDto.Name,
            Type = request.AddDto.Type,
            Country = request.AddDto.Country,
            Currency = request.AddDto.Currency,
            FiscalYearStart = request.AddDto.FiscalYearStart,
            FiscalYearEnd = request.AddDto.FiscalYearEnd,
            IsActive = request.AddDto.IsActive,
            ConsolidationMethod = request.AddDto.ConsolidationMethod,
            RegistrationNumber = request.AddDto.RegistrationNumber,
            TaxId = request.AddDto.TaxId,
            Address = request.AddDto.Address,
            Phone = request.AddDto.Phone,
            Email = request.AddDto.Email,
            Website = request.AddDto.Website,
            ParentEntityId = request.AddDto.ParentEntityId,
            OwnershipPercentage = request.AddDto.OwnershipPercentage,
            IsConsolidated = request.AddDto.IsConsolidated,
            DateAdd = DateTime.UtcNow
        };

        _context.Entities.Add(entity);
        await _context.SaveChangesAsync(ct);

        return MapToDto(entity);
    }

    private EntityDto MapToDto(Entity entity)
    {
        return new EntityDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            LegalName = entity.LegalName,
            Type = entity.Type,
            Country = entity.Country,
            Currency = entity.Currency,
            FiscalYearStart = entity.FiscalYearStart,
            FiscalYearEnd = entity.FiscalYearEnd,
            IsActive = entity.IsActive,
            ConsolidationMethod = entity.ConsolidationMethod,
            RegistrationNumber = entity.RegistrationNumber,
            TaxId = entity.TaxId,
            Address = entity.Address,
            Phone = entity.Phone,
            Email = entity.Email,
            Website = entity.Website,
            ParentEntityId = entity.ParentEntityId,
            ParentEntityName = entity.ParentEntity != null ? entity.ParentEntity.Name : null,
            OwnershipPercentage = entity.OwnershipPercentage ?? 0,
            IsConsolidated = entity.IsConsolidated,
            DateAdd = entity.DateAdd,
            DateMod = entity.DateMod
        };
    }
}

public class EditEntityHandler : IRequestHandler<EditEntityCmd, EntityDto>
{
    private readonly FinanceDbContext _context;

    public EditEntityHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<EntityDto> Handle(EditEntityCmd request, CancellationToken ct)
    {
        var entity = await _context.Entities
            .FirstOrDefaultAsync(x => x.Id == request.EditDto.Id && !x.IsDeleted, ct);

        if (entity == null)
            throw new InvalidOperationException($"Entity with ID '{request.EditDto.Id}' not found");

        // Check for duplicate code (excluding this entity)
        var exists = await _context.Entities
            .AnyAsync(x => x.Code == request.EditDto.Code && x.Id != request.EditDto.Id && !x.IsDeleted, ct);
        if (exists)
            throw new InvalidOperationException($"Entity with code '{request.EditDto.Code}' already exists");

        // Update all properties
        entity.Code = request.EditDto.Code;
        entity.Name = request.EditDto.Name;
        entity.LegalName = request.EditDto.LegalName ?? request.EditDto.Name;
        entity.Type = request.EditDto.Type;
        entity.Country = request.EditDto.Country;
        entity.Currency = request.EditDto.Currency;
        entity.FiscalYearStart = request.EditDto.FiscalYearStart;
        entity.FiscalYearEnd = request.EditDto.FiscalYearEnd;
        entity.OwnershipPercentage = request.EditDto.OwnershipPercentage;
        entity.ConsolidationMethod = request.EditDto.ConsolidationMethod;
        entity.RegistrationNumber = request.EditDto.RegistrationNumber;
        entity.TaxId = request.EditDto.TaxId;
        entity.Address = request.EditDto.Address;
        entity.Phone = request.EditDto.Phone;
        entity.Email = request.EditDto.Email;
        entity.Website = request.EditDto.Website;
        entity.ParentEntityId = request.EditDto.ParentEntityId;
        entity.IsConsolidated = request.EditDto.IsConsolidated;
        entity.IsActive = request.EditDto.IsActive;

        entity.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return MapToDto(entity);
    }

    private EntityDto MapToDto(Entity entity)
    {
        return new EntityDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            LegalName = entity.LegalName,
            Type = entity.Type,
            Country = entity.Country,
            Currency = entity.Currency,
            FiscalYearStart = entity.FiscalYearStart,
            FiscalYearEnd = entity.FiscalYearEnd,
            IsActive = entity.IsActive,
            ConsolidationMethod = entity.ConsolidationMethod,
            RegistrationNumber = entity.RegistrationNumber,
            TaxId = entity.TaxId,
            Address = entity.Address,
            Phone = entity.Phone,
            Email = entity.Email,
            Website = entity.Website,
            ParentEntityId = entity.ParentEntityId,
            ParentEntityName = entity.ParentEntity != null ? entity.ParentEntity.Name : null,
            OwnershipPercentage = entity.OwnershipPercentage ?? 0,
            IsConsolidated = entity.IsConsolidated,
            DateAdd = entity.DateAdd,
            DateMod = entity.DateMod
        };
    }
}

public class DeleteEntityHandler : IRequestHandler<DeleteEntityCmd, bool>
{
    private readonly FinanceDbContext _context;

    public DeleteEntityHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteEntityCmd request, CancellationToken ct)
    {
        var entity = await _context.Entities
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (entity == null)
            return false;

        entity.IsDeleted = true;
        entity.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}