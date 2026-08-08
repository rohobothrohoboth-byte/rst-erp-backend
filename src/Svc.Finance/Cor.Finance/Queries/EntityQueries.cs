using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Queries;

// ============ ENTITY QUERIES ============
public class GetAllEntitiesQry : IRequest<List<EntityDto>>
{
    public bool? IsActive { get; set; }
}

public class GetEntityByIdQry : IRequest<EntityDto>
{
    public Guid Id { get; set; }
}

// ============ ENTITY QUERY HANDLERS ============
public class GetAllEntitiesHandler : IRequestHandler<GetAllEntitiesQry, List<EntityDto>>
{
    private readonly FinanceDbContext _context;

    public GetAllEntitiesHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<EntityDto>> Handle(GetAllEntitiesQry request, CancellationToken ct)
    {
        var query = _context.Entities
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);

        return await query
            .Select(x => new EntityDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                LegalName = x.LegalName,
                Type = x.Type,
                Country = x.Country,
                Currency = x.Currency,
                FiscalYearStart = x.FiscalYearStart,
                FiscalYearEnd = x.FiscalYearEnd,
                IsActive = x.IsActive,
                ConsolidationMethod = x.ConsolidationMethod,
                RegistrationNumber = x.RegistrationNumber,
                TaxId = x.TaxId,
                Address = x.Address,
                Phone = x.Phone,
                Email = x.Email,
                Website = x.Website,
                ParentEntityId = x.ParentEntityId,
                ParentEntityName = x.ParentEntity != null ? x.ParentEntity.Name : null,
                OwnershipPercentage = x.OwnershipPercentage ?? 0,
                IsConsolidated = x.IsConsolidated,
                DateAdd = x.DateAdd,
                DateMod = x.DateMod
            })
            .ToListAsync(ct);
    }
}

public class GetEntityByIdHandler : IRequestHandler<GetEntityByIdQry, EntityDto>
{
    private readonly FinanceDbContext _context;

    public GetEntityByIdHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<EntityDto> Handle(GetEntityByIdQry request, CancellationToken ct)
    {
        var entity = await _context.Entities
            .Include(x => x.ParentEntity)
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (entity == null)
            throw new InvalidOperationException($"Entity with ID '{request.Id}' not found");

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
            ParentEntityName = entity.ParentEntity?.Name,
            OwnershipPercentage = entity.OwnershipPercentage ?? 0,
            IsConsolidated = entity.IsConsolidated,
            DateAdd = entity.DateAdd,
            DateMod = entity.DateMod
        };
    }
}