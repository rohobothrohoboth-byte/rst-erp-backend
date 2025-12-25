namespace Svc.Auth.Models.Dtos;

public class MenuPerApiListDto
{
    public Guid PerMenuId { get; set; } // PerMenu
    public string PerMenu { get; set; } = default; // PerMenu
    public List<NameList> PerApiList { get; set; } = default;
}

public class PerApiListDto
{
    public Guid Id { get; set; }
    public Guid PerMenuId { get; set; } // PerMenu
    public string PerMenuKey { get; set; } = default; // PerMenu
    public string Key { get; set; } = default;
    public string Name { get; set; } = default;
    public string PerMenu { get; set; } = default; // PerMenu
}

public class PerApiAddDto
{
    public string PerMenuKey { get; set; } = default!; // PerMenu
    public string Key { get; set; } = default!;
    public string Desc { get; set; } = default!;
}

public class PerApiModDto
{
    public string PerMenuKey { get; set; } = default!; // PerMenu
    public Guid Id { get; set; }
    public string Key { get; set; } = default!;
    public string Desc { get; set; } = default!;
}
