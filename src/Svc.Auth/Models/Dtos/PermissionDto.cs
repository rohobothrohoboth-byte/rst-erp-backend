// Models/Dtos/PermissionStructureDto.cs

using System;
using System.Collections.Generic;

namespace Svc.Auth.Models.Dtos;

public class PermissionStructureDto
{
    public List<ModuleStructureDto> Modules { get; set; } = new();
}

public class ModuleStructureDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public int Order { get; set; }
    public List<MenuStructureDto> Menus { get; set; } = new();
}

public class MenuStructureDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? Path { get; set; }
    public string? Icon { get; set; }
    public bool IsChild { get; set; }
    public int Order { get; set; }
    public Guid? ParentId { get; set; }
    public List<MenuStructureDto> Children { get; set; } = new();
    public List<ApiActionDto> Actions { get; set; } = new();
}

public class ApiActionDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Order { get; set; }
}

public class FilteredPermissionsReq
{
    public string UserId { get; set; } = string.Empty;
    public List<Guid> ModuleIds { get; set; } = new();
}

public class FilteredApiPermissionsReq
{
    public string UserId { get; set; } = string.Empty;
    public List<Guid> MenuIds { get; set; } = new();
}
// DTO

public class SaveUserPermissionsReq
{
    public string UserId { get; set; } = string.Empty;
    public List<string>? ModuleIds { get; set; }
    public List<string>? MenuIds { get; set; }
    public List<string>? ApiActionIds { get; set; }
}