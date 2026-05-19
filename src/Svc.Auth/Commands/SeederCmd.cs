using MediatR;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;
using Svc.Auth.Queries;

namespace Svc.Auth.Commands;

public sealed class PerMenuSeedCmd : IRequest { public List<PerMenuSeedDto> AddDto { get; set; } = default!; }
public sealed class PerAccessSeedCmd : IRequest { public List<PerAccessSeedDto> AddDto { get; set; } = default!; }



public sealed class PerMenuSeed(IUnitOfWork _uow, IMediator _med) : IRequestHandler<PerMenuSeedCmd>
{
    public async Task Handle(PerMenuSeedCmd request, CancellationToken ct)
    {
        if (request.AddDto == null || request.AddDto.Count == 0) { return; }
        await _uow.Begin(ct);
        try
        {
            var moduleKeys = request.AddDto.Select(x => x.ModKey).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            var modules = await _med.Send(new ModIdsBykeysQry { Keys = moduleKeys }, ct);
            var moduleDict = modules.ToDictionary(x => x.Key, x => x.Id, StringComparer.OrdinalIgnoreCase);
            var parKeys = request.AddDto.Where(x => x.IsChild && !string.IsNullOrWhiteSpace(x.ParKey)).Select(x => x.ParKey).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            var existingParents = parKeys.Count > 0 ? await _med.Send(new MenuIdsByKeysQry { Keys = parKeys }, ct) : [];
            var menuDict = existingParents.ToDictionary(x => x.Key, x => x.Id, StringComparer.OrdinalIgnoreCase);
            var orderedDtos = request.AddDto.OrderBy(x => x.IsChild).ThenBy(x => x.Order).ToList();

            foreach (var dto in orderedDtos)
            {
                if (string.IsNullOrWhiteSpace(dto.ModKey)) { continue; }
                if (!moduleDict.TryGetValue(dto.ModKey, out var moduleId)) { continue; }

                Guid? parentId = null;
                if (dto.IsChild)
                {
                    if (string.IsNullOrWhiteSpace(dto.ParKey) || !menuDict.TryGetValue(dto.ParKey, out var parentMenuId)) { continue; }
                    parentId = parentMenuId;
                }

                var entity = new PerMenu
                {
                    PerModuleId = moduleId,
                    Key = dto.Key,
                    Label = dto.Label,
                    Path = dto.Path,
                    Icon = dto.Icon,
                    IsChild = dto.IsChild,
                    ParentId = parentId,
                    Order = dto.Order
                };

                await _uow.Add(entity, ct);
                if (!menuDict.ContainsKey(entity.Key)) { menuDict[entity.Key] = entity.Id; }
            }

            await _uow.Commit(ct);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public sealed class PerAccessSeed(IUnitOfWork _uow, IMediator _med) : IRequestHandler<PerAccessSeedCmd>
{
    public async Task Handle(PerAccessSeedCmd request, CancellationToken ct)
    {
        if (request.AddDto == null || request.AddDto.Count == 0) { return; }
        await _uow.Begin(ct);
        try
        {
            var menuKeys = request.AddDto.Select(x => x.MenuKey).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            var menus = await _med.Send(new MenuIdsByKeysQry { Keys = menuKeys }, ct);
            var menuDict = menus.ToDictionary(x => x.Key, x => x.Id, StringComparer.OrdinalIgnoreCase);            
            var orderedDtos = request.AddDto.OrderBy(x => x.Key).ThenBy(x => x.MenuKey).ToList();

            foreach (var dto in orderedDtos)
            {
                if (string.IsNullOrWhiteSpace(dto.MenuKey)) { continue; }
                if (!menuDict.TryGetValue(dto.MenuKey, out var menuId)) { continue; }       

                var entity = new PerApi
                {
                    PerMenuId = menuId,
                    Key = dto.Key,
                    Desc = dto.Desc
                };
                await _uow.Add(entity, ct);
                if (!menuDict.ContainsKey(entity.Key)) { menuDict[entity.Key] = entity.Id; }
            }

            await _uow.Commit(ct);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}