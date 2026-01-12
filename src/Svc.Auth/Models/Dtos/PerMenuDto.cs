namespace Svc.Auth.Models.Dtos;

public class ModPerMenuListDto
{
    public Guid PerModuleId { get; set; }
    public string PerModule { get; set; } = default; // PerModule
    public List<NameList> PerMenuList { get; set; } = default;
}

public class PerMenuListDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = default;
    public string Name { get; set; } = default;
    public string Module { get; set; } = default; // PerModule
}

public class PerMenuAddDto
{
    public Guid PerModuleId { get; set; } // PerModule
    public string Key { get; set; } = default!;
    public string Label { get; set; } = default!;
    public string Path { get; set; } = default!;
    public string Icon { get; set; } = default!;
    public bool IsChild { get; set; } = false!;
    public string Parent { get; set; } = "";
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
    public string Parent { get; set; } = "";
    public int Order { get; set; }
}