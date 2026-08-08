namespace Svc.Auth.Models.Dtos;

// Row classes (used for Dapper mapping) - KEEP THESE HERE
public class PerMenuApiRow
{
    public Guid ApiId { get; set; }
    public string Desc { get; set; } = default!;
    public Guid PerMenuId { get; set; }
    public string Label { get; set; } = default!;
}

public class UserMenuApiRow
{
    public Guid PerMenuId { get; set; }
    public string Label { get; set; } = default!;
    public Guid ApiId { get; set; }
    public string Desc { get; set; } = default!;
}

public class ModPerMenuListDto
{
    public Guid PerModuleId { get; set; }
    public string PerModule { get; set; } = default!;
    public List<NameList> PerMenuList { get; set; } = default!;
}

public class ModuleMenuRow
{
    public Guid PerModuleId { get; set; }
    public string ModuleDesc { get; set; } = default!;
    public Guid MenuId { get; set; }
    public string MenuLabel { get; set; } = default!;
}

public class UserModuleMenuRow
{
    public Guid ModuleId { get; set; }
    public string ModuleDesc { get; set; } = default!;
    public Guid MenuId { get; set; }
    public string MenuLabel { get; set; } = default!;
}

public class PerMenuJoinRow
{
    public Guid Id { get; set; }
    public Guid PerModuleId { get; set; }
    public Guid? ParentId { get; set; }
    public int Order { get; set; }
    public bool IsChild { get; set; }
    public string Key { get; set; } = default!;
    public string Label { get; set; } = default!;
    public string Path { get; set; } = default!;
    public string Icon { get; set; } = default!;
    public bool IsDeleted { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public string? ParentLabel { get; set; }
    public string? ParentKey { get; set; }
    public string ModuleDesc { get; set; } = default!;
}

public class PerMenuListDto : BaseDto
{
    public string Path { get; set; } = default!;
    public string Icon { get; set; } = default!;
    public string ParentKey { get; set; } = "";
    public Guid PerModuleId { get; set; }
    public int Order { get; set; }
    public bool IsChild { get; set; } = false!;
    public string Key { get; set; } = default!;
    public string Label { get; set; } = default!;
    public string IsChildStr { get; set; } = default!;
    public string Parent { get; set; } = default!;
    public string Module { get; set; } = default!;
}

public class PerMenuAddDto
{
    public Guid PerModuleId { get; set; }
    public string Key { get; set; } = default!;
    public string Label { get; set; } = default!;
    public string Path { get; set; } = default!;
    public string Icon { get; set; } = default!;
    public bool IsChild { get; set; } = false!;
    public string ParentKey { get; set; } = "";
    public int Order { get; set; }
}

public class PerMenuModDto
{
    public Guid PerModuleId { get; set; }
    public Guid Id { get; set; }
    public string Key { get; set; } = default!;
    public string Label { get; set; } = default!;
    public string Path { get; set; } = default!;
    public string Icon { get; set; } = default!;
    public bool IsChild { get; set; } = false!;
    public string ParentKey { get; set; } = "";
    public int Order { get; set; }
}

public class PerMenuDto
{
    public string Module { get; set; } = default!;
    public Guid Id { get; set; }
    public string Key { get; set; } = default!;
    public string Label { get; set; } = default!;
    public string Path { get; set; } = default!;
    public string Icon { get; set; } = default!;
    public int Order { get; set; }
    public Guid PerModuleId { get; set; }
    public Guid? ParentId { get; set; }
    public List<PerMenuDto> Children { get; set; } = [];
}

// In your DTOs
public class UpdateMenuPermissionDto
{
    public Guid Id { get; set; }
    public Guid PerModuleId { get; set; }
    public string Key { get; set; } = default!;
    public string Label { get; set; } = default!;
    public string Path { get; set; } = default!;
    public string Icon { get; set; } = default!;
    public bool IsChild { get; set; }
    public int Order { get; set; }
    public string? ParentKey { get; set; }  // Add this
    public Guid? ParentId { get; set; }      // Or keep this for direct ID

}

public class MenuHierarchyDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? Path { get; set; }
    public string? Icon { get; set; }
    public bool IsChild { get; set; }
    public int Order { get; set; }
    public Guid? ParentId { get; set; }
    public Guid? PerModuleId { get; set; }
    public string? ModuleKey { get; set; }
    public string? ModuleName { get; set; }
}

public class MenuTreeDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? Path { get; set; }
    public string? Icon { get; set; }
    public bool IsChild { get; set; }
    public int Order { get; set; }
    public Guid? ParentId { get; set; }
    public Guid? PerModuleId { get; set; }
    public string? ModuleKey { get; set; }
    public string? ModuleName { get; set; }
    public List<MenuTreeDto> Children { get; set; } = new();
}

// Svc.Auth.Models.Dtos - Update your menu DTO

public class MenuPermissionDto
{
    public string K { get; set; } = string.Empty; // Key/ID
    public string L { get; set; } = string.Empty; // Label
    public string? P { get; set; } // Path
    public string? I { get; set; } // Icon
    public int O { get; set; } // Order
    public string[] A { get; set; } = Array.Empty<string>(); // Actions
    public List<MenuPermissionDto> C { get; set; } = new(); // Children

    // ? ADD THIS - ParentId is required for hierarchy
    public string? ParentId { get; set; }
}
public class MenuResponseDto
{
    public string K { get; set; } = string.Empty;  // Key
    public string L { get; set; } = string.Empty;  // Label
    public string? P { get; set; }  // Path
    public string? I { get; set; }  // Icon
    public int O { get; set; }  // Order
    public Guid? ParentId { get; set; }  // ? CHANGE TO Guid? (not string)
    public Guid? ModuleId { get; set; }  // ? CHANGE TO Guid?
    public string[] A { get; set; } = Array.Empty<string>();  // Actions
    public List<MenuResponseDto> C { get; set; } = new();  // Children
}
