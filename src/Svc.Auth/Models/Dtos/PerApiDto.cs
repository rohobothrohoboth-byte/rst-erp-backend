namespace Svc.Auth.Models.Dtos;

public class MenuPerApiListDto
{
    public Guid PerMenuId { get; set; }
    public string PerMenu { get; set; } = default!;
    public List<NameList> PerApiList { get; set; } = default!;
}
public class PerApiDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string PerMenu { get; set; } = default!;
    public string Description { get; set; } = default!;
}
public class PerApiListDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string PerMenu { get; set; } = default!;
}

public class PerApiAddDto
{
    public string PerMenuKey { get; set; } = default!;
    public string Key { get; set; } = default!;
    public string Desc { get; set; } = default!;
}

public class PerApiModDto
{
    public string PerMenuKey { get; set; } = default!;
    public Guid Id { get; set; }
    public string Key { get; set; } = default!;
    public string Desc { get; set; } = default!;
}

