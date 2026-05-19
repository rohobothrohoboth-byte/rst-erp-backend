namespace Svc.Auth.Models.Dtos;

public class ModPerMenuListDto
{
    public Guid PerModuleId { get; set; }
    public string PerModule { get; set; } = default!; // PerModule
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

    // Parent menu
    public string? ParentLabel { get; set; }
    public string? ParentKey { get; set; }

    // Module
    public string ModuleDesc { get; set; } = default!;
}

public class PerMenuListDto : BaseDto
{
    public string Path { get; set; } = default!;
    public string Icon { get; set; } = default!;
    public string ParentKey { get; set; } = "";
    public Guid PerModuleId { get; set; } // PerModule
    public int Order { get; set; }
    public bool IsChild { get; set; } = false!;
    public string Key { get; set; } = default!;
    public string Label { get; set; } = default!;
    public string IsChildStr { get; set; } = default!;
    public string Parent { get; set; } = default!;
    public string Module { get; set; } = default!; // PerModule
}

public class PerMenuAddDto
{
    public Guid PerModuleId { get; set; } // PerModule
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
    public Guid PerModuleId { get; set; } // PerModule
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