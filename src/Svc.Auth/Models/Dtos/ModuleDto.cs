namespace Svc.Auth.Models.Dtos;

public class PerModuleAddDto
{
    public string Key { get; set; } = string.Empty;
    public string Desc { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public int? Order { get; set; }
}

public class PerModuleModDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Desc { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public int? Order { get; set; }
}

public class PerModuleListDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Desc { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public int? Order { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public bool IsDeleted { get; set; }
}