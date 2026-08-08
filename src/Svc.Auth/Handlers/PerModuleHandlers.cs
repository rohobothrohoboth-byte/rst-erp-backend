using MediatR;
using Microsoft.EntityFrameworkCore;
using Svc.Auth.Persistence;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;
using Svc.Auth.Queries;
using Svc.Auth.Commands;
using Helpers;
using Svc.Auth.Interfaces;

namespace Svc.Auth.Handlers;

// ==================== QUERY HANDLERS ====================

public class ModuleNameByIdQryHandler : IRequestHandler<ModuleNameByIdQry, NameList?>
{
    private readonly IDapperHelper _dapper;
    public ModuleNameByIdQryHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<NameList?> Handle(ModuleNameByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<PerModule>(v, x => x.Id)
            .SelectAs<PerModule, NameList>(v, x => x.Desc, d => d.Name)
            .Select<PerModule>(v, x => x.Key)
            .Select<PerModule>(v, x => x.Icon!)
            .Select<PerModule>(v, x => x.Order)
            .From<PerModule>(v)
            .Where<PerModule>(v, p => p.Id == request.Id)
            .Where<PerModule>(v, x => x.IsDeleted == false)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<NameList>(sql, parameters, ct);
        return data;
    }
}

public class PerModuleAllQryHandler(AuthDbContext db) : IRequestHandler<PerModuleAllQry, List<PerModuleListDto>>
{
    public async Task<List<PerModuleListDto>> Handle(PerModuleAllQry request, CancellationToken cancellationToken)
    {
        return await db.PerModule
            .Where(m => !m.IsDeleted)
            .OrderBy(m => m.Order)
            .Select(m => new PerModuleListDto
            {
                Id = m.Id,
                Key = m.Key,
                Desc = m.Desc,
                Icon = m.Icon,
                Order = m.Order,
                DateAdd = m.DateAdd,
                DateMod = m.DateMod,
                IsDeleted = m.IsDeleted
            })
            .ToListAsync(cancellationToken);
    }
}

public class PerModuleByIdQryHandler(AuthDbContext db) : IRequestHandler<PerModuleByIdQry, PerModuleListDto>
{
    public async Task<PerModuleListDto> Handle(PerModuleByIdQry request, CancellationToken cancellationToken)
    {
        var module = await db.PerModule
            .Where(m => m.Id == request.Id && !m.IsDeleted)
            .Select(m => new PerModuleListDto
            {
                Id = m.Id,
                Key = m.Key,
                Desc = m.Desc,
                Icon = m.Icon,
                Order = m.Order,
                DateAdd = m.DateAdd,
                DateMod = null,
                IsDeleted = m.IsDeleted
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (module == null)
            throw new DomainException($"MODULE with id [{request.Id}] NOT FOUND.");

        return module;
    }
}

// ==================== COMMAND HANDLERS ====================

public class PerModuleAddCmdHandler(AuthDbContext db) : IRequestHandler<PerModuleAddCmd, PerModuleListDto>
{
    public async Task<PerModuleListDto> Handle(PerModuleAddCmd request, CancellationToken cancellationToken)
    {
        var exists = await db.PerModule
            .AnyAsync(m => m.Key == request.AddDto.Key && !m.IsDeleted, cancellationToken);

        if (exists)
            throw new DomainException($"Module with key '{request.AddDto.Key}' already exists.");

        var module = new PerModule
        {
            Id = Guid.NewGuid(),
            Key = request.AddDto.Key,
            Desc = request.AddDto.Desc,
            Icon = request.AddDto.Icon,
            Order = request.AddDto.Order ?? 0,
            DateAdd = DateTime.UtcNow,
            IsDeleted = false
        };

        db.PerModule.Add(module);
        await db.SaveChangesAsync(cancellationToken);

        return new PerModuleListDto
        {
            Id = module.Id,
            Key = module.Key,
            Desc = module.Desc,
            Icon = module.Icon,
            Order = module.Order,
            DateAdd = module.DateAdd,
            DateMod = null,
            IsDeleted = module.IsDeleted
        };
    }
}

public class PerModuleModCmdHandler(AuthDbContext db) : IRequestHandler<PerModuleModCmd, PerModuleListDto>
{
    public async Task<PerModuleListDto> Handle(PerModuleModCmd request, CancellationToken cancellationToken)
    {
        var module = await db.PerModule
            .FirstOrDefaultAsync(m => m.Id == request.ModDto.Id && !m.IsDeleted, cancellationToken);

        if (module == null)
            throw new DomainException($"Module with id '{request.ModDto.Id}' not found.");

        if (module.Key != request.ModDto.Key)
        {
            var keyExists = await db.PerModule
                .AnyAsync(m => m.Key == request.ModDto.Key && m.Id != request.ModDto.Id && !m.IsDeleted, cancellationToken);

           if (keyExists)
               throw new DomainException($"Module with key '{request.ModDto.Key}' already exists.");
        }

        module.Key = request.ModDto.Key;
        module.Desc = request.ModDto.Desc;
        module.Icon = request.ModDto.Icon;
        module.Order = request.ModDto.Order ?? 0;
        module.DateMod = DateTime.UtcNow;

        db.Entry(module).State = EntityState.Modified;
        await db.SaveChangesAsync(cancellationToken);

        return new PerModuleListDto
        {
            Id = module.Id,
            Key = module.Key,
            Desc = module.Desc,
            Icon = module.Icon,
            Order = module.Order,
            DateAdd = module.DateAdd,
            DateMod = module.DateMod,
            IsDeleted = module.IsDeleted
        };
    }
}

public class PerModuleDelCmdHandler(AuthDbContext db) : IRequestHandler<PerModuleDelCmd, Unit>
{
    public async Task<Unit> Handle(PerModuleDelCmd request, CancellationToken cancellationToken)
    {
        var module = await db.PerModule
            .FirstOrDefaultAsync(m => m.Id == request.Id && !m.IsDeleted, cancellationToken);

        if (module == null)
            throw new DomainException($"Module with id '{request.Id}' not found.");

        module.IsDeleted = true;
        module.DateMod = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}