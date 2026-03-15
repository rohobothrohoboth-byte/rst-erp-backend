namespace Svc.Auth.Models.Dtos;

public class PerMenuApiRow
{
    public Guid PerMenuId { get; set; }
    public string Label { get; set; } = default!;

    public Guid ApiId { get; set; }
    public string Desc { get; set; } = default!;
}

public class UserMenuApiRow
{
    public Guid PerMenuId { get; set; }
    public string Label { get; set; } = default!;

    public Guid ApiId { get; set; }
    public string Desc { get; set; } = default!;
}

public class MenuPerApiListDto
{
    public Guid PerMenuId { get; set; }
    public string PerMenu { get; set; } = default!; // PerMenu
    public List<NameList> PerApiList { get; set; } = default!;
}

public class PerApiListDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string PerMenu { get; set; } = default!; // PerMenu
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
