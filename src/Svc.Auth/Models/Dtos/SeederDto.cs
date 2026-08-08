namespace Svc.Auth.Models.Dtos;
public class PerModuleSeedDto
{
    public string Key { get; set; } = default!;
    public string Desc { get; set; } = default!;
    public string Icon { get; set; } = default!;
    public int Order { get; set; }
}
public class PerMenuSeedDto
{
    public string ModKey { get; set; } = ""; // PerModule
    public string Key { get; set; } = default!;
    public string Label { get; set; } = default!;
    public string Path { get; set; } = default!;
    public string Icon { get; set; } = default!;
    public bool IsChild { get; set; } = false!;
    public string ParKey { get; set; } = "";
    public int Order { get; set; }
}

public class PerAccessSeedDto
{
    public string MenuKey { get; set; } = default!; // PerMenu
    public string Key { get; set; } = default!;
    public string Desc { get; set; } = default!;
}

public class IdDto
{
    public Guid Id { get; set; }
}

public sealed class KeyIdDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
}