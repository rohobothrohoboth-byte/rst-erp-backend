// Queries/PermissionStructureQry.cs
using Dapper;
using Helpers;
using MediatR;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Queries;
public class GetPermissionStructureQry : IRequest<PermissionStructureDto> { }

public class GetPermissionStructureHandler(IDapperHelper _dapper)
    : IRequestHandler<GetPermissionStructureQry, PermissionStructureDto>
{
    public async Task<PermissionStructureDto> Handle(GetPermissionStructureQry request, CancellationToken ct)
    {
        // Get all modules (PostgreSQL, case-sensitive quoted identifiers)
        const string moduleSql = @"
            SELECT ""Id"", ""Key"", ""Desc"" AS ""Name"", ""Icon"", ""Order""
            FROM ""PerModule""
            WHERE ""IsDeleted"" = false
            ORDER BY ""Order"", ""Key""";

        var modules = (await _dapper.QueryAsync<ModuleStructureDto>(moduleSql, null, ct)).ToList();

        // Get all menus with their actions
        const string menuSql = @"
            SELECT
                m.""Id"", m.""Key"", m.""Label"", m.""Path"", m.""Icon"", m.""IsChild"", m.""Order"", m.""ParentId"", m.""PerModuleId"",
                a.""Id"" AS ""ActionId"", a.""Key"" AS ""ActionKey"", a.""Desc"" AS ""ActionName""
            FROM ""PerMenu"" m
            LEFT JOIN ""PerApi"" a ON a.""PerMenuId"" = m.""Id"" AND a.""IsDeleted"" = false
            WHERE m.""IsDeleted"" = false
            ORDER BY m.""Order"", a.""Key""";

        var menuData = await _dapper.QueryAsync<dynamic>(menuSql, null, ct);

        // Build menu tree
        var menus = new Dictionary<Guid, MenuStructureDto>();
        // Track each menu's owning module so we can assign by FK, not by key prefix.
        var menuModuleMap = new Dictionary<Guid, Guid>();

        foreach (var row in menuData)
        {
            menuModuleMap[(Guid)row.Id] = (Guid)row.PerModuleId;

            if (!menus.ContainsKey(row.Id))
            {
                menus[row.Id] = new MenuStructureDto
                {
                    Id = row.Id,
                    Key = row.Key,
                    Label = row.Label,
                    Path = row.Path,
                    Icon = row.Icon,
                    IsChild = row.IsChild,
                    Order = row.Order,
                    ParentId = row.ParentId,
                    Children = new List<MenuStructureDto>(),
                    Actions = new List<ApiActionDto>()
                };
            }

            if (row.ActionId != null)
            {
                menus[row.Id].Actions.Add(new ApiActionDto
                {
                    Id = row.ActionId,
                    Key = row.ActionKey,
                    Name = row.ActionName
                });
            }
        }

        // Build hierarchy
        var rootMenus = new List<MenuStructureDto>();
        foreach (var menu in menus.Values)
        {
            if (menu.ParentId == null)
            {
                rootMenus.Add(menu);
            }
            else if (menus.ContainsKey(menu.ParentId.Value))
            {
                menus[menu.ParentId.Value].Children.Add(menu);
            }
        }

        // Assign menus to modules by their real PerModuleId FK.
        foreach (var module in modules)
        {
            module.Menus = rootMenus
                .Where(m => menuModuleMap.TryGetValue(m.Id, out var moduleId) && moduleId == module.Id)
                .ToList();
        }

        return new PermissionStructureDto { Modules = modules.ToList() };
    }
}